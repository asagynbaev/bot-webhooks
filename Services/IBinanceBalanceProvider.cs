namespace bot_webhooks.Services;

public interface IBinanceBalanceProvider
{
	Task<decimal> GetBalanceAsync(CancellationToken cancellationToken = default);
	Task<decimal> GetTokensAmountAsync(string symbol, CancellationToken cancellationToken = default);
}
