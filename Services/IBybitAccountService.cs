using bot_webhooks.Models;

namespace bot_webhooks.Services;

public interface IBybitAccountService
{
	Task<BybitAccountInfo?> GetAccountInfoAsync(CancellationToken cancellationToken = default);
}
