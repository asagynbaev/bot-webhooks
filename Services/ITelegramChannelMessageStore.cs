using bot_webhooks.Models;

namespace bot_webhooks.Services;

public interface ITelegramChannelMessageStore
{
	void Add(TelegramChannelMessage message);
	IReadOnlyList<TelegramChannelMessage> GetLast(int count = 50);
}
