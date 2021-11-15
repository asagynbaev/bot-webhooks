using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Binance.Net;
using Binance.Net.Enums;
using bot_webhooks.Data;
using bot_webhooks.Helpers;
using bot_webhooks.Models;

namespace bot_webhooks.Services
{
    public class TradeService : ITradeRepo
    {
        private readonly WebHookContext _db;

        public TradeService(WebHookContext db) => (_db) = (db);
        
        public Trade GetTrade(string symbol, int direction) => 
            _db.Trades.Where(x => x.Symbol == symbol)
            .Where(x => x.Direction == direction)
            .FirstOrDefault();

        public async Task<int> InsertTrade(Signal signal, int signalId)
        {
            var newTrade = new Trade()
            {
                Price = signal.Price,
                SignalID = signalId,
                Symbol = signal.Symbol,
                Direction = signal.Direction,
                SignalType = signal.SignalType
            };
            decimal _myBalance = 0;
            decimal _tokensAmount = 0;
            
            if(signal.Direction == 0)
                _myBalance = await Balance.GetBalance();
            else
                _tokensAmount = await Balance.GetTokensAmount(signal.Symbol);

            using (var client = new BinanceClient())
            {
                var result = await client.Spot.Order.PlaceOrderAsync(
                    signal.Symbol, 
                    (OrderSide)signal.Direction,
                    OrderType.Market, 
                    _tokensAmount == 0 ? null : _tokensAmount, // SymbolAmount
                    _myBalance == 0 ? null : _myBalance, // quoteSymbolAmount
                    null, null, null, null, null, null, null, default // Unnecessary parameters
                );

                if(result.Success)
                {
                    foreach (var item in result.Data.Fills)
                    {
                        newTrade.BoughtPrice = item.Price;
                        newTrade.BuyQuoteCommission = item.Commission;
                        newTrade.Amount = item.Quantity;
                    }
                }
                else
                {
                    await TelegramMessenger.SendMessage($"Error");
                }
            }

            await _db.AddAsync(newTrade);
            await _db.SaveChangesAsync();
            int id = newTrade.ID;

            await TelegramMessenger.SendMessage($"Bought: {newTrade.Symbol}, Price: {newTrade.BoughtPrice}");

            return id;
        }

        public Trade UpdateTrade(int ID)
        {
            throw new System.NotImplementedException();
        }
    }
}