namespace bot_webhooks.Models;

/// <summary>
/// Message received from the configured Telegram channel (userbot).
/// </summary>
public class TelegramChannelMessage
{
	public long MessageId { get; init; }
	public long ChannelId { get; init; }
	public string? ChannelTitle { get; init; }
	public string Text { get; init; } = string.Empty;
	public DateTime ReceivedAtUtc { get; init; }
	public bool IsEdit { get; init; }
}
