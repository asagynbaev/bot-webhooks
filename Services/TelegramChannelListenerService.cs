using bot_webhooks.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TL;
using WTelegram;

namespace bot_webhooks.Services;

/// <summary>
/// Background service that connects to Telegram as a user (MTProto), joins the configured channel
/// and pushes received messages into <see cref="ITelegramChannelMessageStore"/>.
/// Requires api_id/api_hash from https://my.telegram.org/apps and the account phone number.
/// </summary>
public sealed class TelegramChannelListenerService : BackgroundService
{
	private readonly ITelegramChannelMessageStore _store;
	private readonly ILogger<TelegramChannelListenerService> _logger;
	private readonly Func<string, string?> _config;
	private long? _targetChannelId;

	public TelegramChannelListenerService(
		ITelegramChannelMessageStore store,
		ILogger<TelegramChannelListenerService> logger)
	{
		_store = store;
		_logger = logger;
		_config = key => key switch
		{
			"api_id" => Environment.GetEnvironmentVariable("TELEGRAM_API_ID"),
			"api_hash" => Environment.GetEnvironmentVariable("TELEGRAM_API_HASH"),
			"phone_number" => Environment.GetEnvironmentVariable("TELEGRAM_PHONE_NUMBER"),
			"verification_code" => Environment.GetEnvironmentVariable("TELEGRAM_VERIFICATION_CODE"),
			"password" => Environment.GetEnvironmentVariable("TELEGRAM_2FA_PASSWORD"),
			"session_pathname" => Path.Combine(Path.GetTempPath(), "bot_webhooks_telegram.session"),
			_ => null
		};
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		if (string.IsNullOrEmpty(_config("api_id")) || string.IsNullOrEmpty(_config("api_hash")) || string.IsNullOrEmpty(_config("phone_number")))
		{
			_logger.LogWarning("Telegram channel listener skipped: set TELEGRAM_API_ID, TELEGRAM_API_HASH, TELEGRAM_PHONE_NUMBER to receive channel messages.");
			return;
		}

		var channelUsername = Environment.GetEnvironmentVariable("TELEGRAM_SIGNAL_CHANNEL_USERNAME")?.Trim().TrimStart('@');
		var channelIdRaw = Environment.GetEnvironmentVariable("TELEGRAM_SIGNAL_CHANNEL_ID");
		if (string.IsNullOrEmpty(channelUsername) && string.IsNullOrEmpty(channelIdRaw))
		{
			_logger.LogWarning("Telegram channel listener skipped: set TELEGRAM_SIGNAL_CHANNEL_USERNAME or TELEGRAM_SIGNAL_CHANNEL_ID to filter messages.");
			return;
		}

		if (!string.IsNullOrEmpty(channelIdRaw) && long.TryParse(channelIdRaw, out var id))
			_targetChannelId = id;

		WTelegram.Helpers.Log = (level, msg) => _logger.Log((LogLevel)level, msg);

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				using var client = new Client(_config);
				var manager = client.WithUpdateManager(OnUpdate);
				var user = await client.LoginUserIfNeeded();
				_logger.LogInformation("Telegram user session: {User}", user.username ?? user.first_name);

				if (_targetChannelId == null && !string.IsNullOrEmpty(channelUsername))
				{
					var resolved = await client.Contacts_ResolveUsername(channelUsername);
					if (resolved.peer is PeerChannel pc)
					{
						_targetChannelId = pc.channel_id;
						_logger.LogInformation("Resolved channel @{Name} to id {ChannelId}", channelUsername, _targetChannelId);
					}
					else
					{
						_logger.LogWarning("TELEGRAM_SIGNAL_CHANNEL_USERNAME did not resolve to a channel.");
						return;
					}
				}

				var dialogs = await client.Messages_GetAllDialogs();
				dialogs.CollectUsersChats(manager.Users, manager.Chats);

				_logger.LogInformation("Telegram channel listener connected; waiting for messages (channel id: {ChannelId}).", _targetChannelId);

				await Task.Delay(Timeout.Infinite, stoppingToken);
			}
			catch (OperationCanceledException)
			{
				break;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Telegram channel listener error; reconnecting in 60s.");
				await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
			}
		}
	}

	private Task OnUpdate(Update update)
	{
		switch (update)
		{
			case UpdateNewMessage unm:
				HandleMessage(unm.message, false);
				break;
			case UpdateEditMessage uem:
				HandleMessage(uem.message, true);
				break;
			default:
				break;
		}
		return Task.CompletedTask;
	}

	private void HandleMessage(MessageBase messageBase, bool isEdit)
	{
		if (messageBase is not Message msg || string.IsNullOrWhiteSpace(msg.message))
			return;

		long? channelId = null;
		string? channelTitle = null;

		if (messageBase.Peer is PeerChannel pc)
		{
			channelId = pc.channel_id;
			if (_targetChannelId.HasValue && pc.channel_id != _targetChannelId.Value)
				return;
		}
		else
			return;

		_store.Add(new TelegramChannelMessage
		{
			MessageId = msg.ID,
			ChannelId = channelId ?? 0,
			ChannelTitle = channelTitle,
			Text = msg.message,
			ReceivedAtUtc = DateTime.UtcNow,
			IsEdit = isEdit
		});
		_logger.LogInformation("Telegram channel message: {Text}", msg.message.Trim());
	}
}
