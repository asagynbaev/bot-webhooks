using System.Collections.Concurrent;
using bot_webhooks.Models;

namespace bot_webhooks.Services;

public sealed class TelegramChannelMessageStore : ITelegramChannelMessageStore
{
	private readonly ConcurrentQueue<TelegramChannelMessage> _messages = new();
	private const int MaxStored = 500;

	public void Add(TelegramChannelMessage message)
	{
		_messages.Enqueue(message);
		while (_messages.Count > MaxStored && _messages.TryDequeue(out _)) { }
	}

	public IReadOnlyList<TelegramChannelMessage> GetLast(int count = 50)
	{
		var list = _messages.ToArray();
		if (list.Length == 0) return Array.Empty<TelegramChannelMessage>();
		count = Math.Min(count, list.Length);
		var result = new TelegramChannelMessage[count];
		Array.Copy(list, list.Length - count, result, 0, count);
		return result;
	}
}
