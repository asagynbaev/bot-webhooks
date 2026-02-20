using bot_webhooks.Models;

namespace bot_webhooks.Data;

public interface ITradeRepo
{
	Task<int> InsertTrade(Signal signal, int signalId);
	Trade? GetTrade(string symbol, int direction);
	Trade? UpdateTrade(int ID);
	Task<BinanceFutureAccountInfo> BinanceAccountInfoAsync(CancellationToken cancellationToken = default);
}
