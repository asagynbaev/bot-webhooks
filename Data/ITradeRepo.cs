using System.Collections.Generic;
using System.Threading.Tasks;
using Binance.Net.Objects.Futures.FuturesData;
using bot_webhooks.Models;

namespace bot_webhooks.Data
{
    public interface ITradeRepo
    {
        Task<int> InsertTrade(Signal signal, int signalId);
        Trade GetTrade(string symbol, int direction);
        Trade UpdateTrade(int ID);

        Task<BinanceFutureAccountInfo> BinanceAccountInfo();
        Task<BinanceFuturesPlacedOrder> BinanceFutureOrder();
    }
}