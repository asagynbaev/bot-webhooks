using System;
using System.Threading.Tasks;
using Binance.Net;
using Binance.Net.Enums;
using CryptoExchange.Net.Authentication;

namespace bot_webhooks.Models
{
    public class Position
    {
        internal WebHookContext Db { get; set; }
        public Position()
        {
        }

        public Position(WebHookContext db)
        {
            Db = db;
        }

        public string Symbol { get; set; }
        public int Direction { get; set; }
        public int Level { get; set; }

        #region SPOT Trading
        public async Task<bool> OpenSpotPosition(Position signal)
        {
            bool taskResult = false;
            User users = new User(Db);
            // FIXME: if user is active, and Spot only
            var allUsers = await users.GetAllusers();

            foreach (var item in allUsers)
            {
                decimal balance = 0;
                decimal symbolAmount = 0;

                BinanceClient.SetDefaultOptions(new Binance.Net.Objects.BinanceClientOptions()
                {
                    ApiCredentials = new ApiCredentials(item.ApiKey, item.Secret)
                });

                if(signal.Direction == 0)
                    balance = await GetBalanceOrSymbolAmount(signal.Symbol, signal.Direction);
                else
                    symbolAmount =  await GetBalanceOrSymbolAmount(signal.Symbol, signal.Direction);
                
                using (var client = new BinanceClient())
                {
                    var result = await client.Spot.Order.PlaceOrderAsync(
                        signal.Symbol, 
                        signal.Direction == 0 ? OrderSide.Buy : OrderSide.Sell,
                        OrderType.Market, 
                        symbolAmount == 0 ? null : symbolAmount, // SymbolAmount
                        balance == 0 ? null : balance, // quoteSymbolAmount
                        null, null, null, null, null, null, null, default // Unnecessary parameters
                    );

                    if(result.Success)
                        taskResult = true;
                    else
                    {
                        // TODO: log error message
                        TelegramMessenger.SendMessage($"Error: {result.Error.Message}");
                        taskResult = false;
                    }
                }
            }

            // FIXME: Modify logic, because for some users we can get error message
            return taskResult;
            
        }
        #endregion

        #region USDT-M Futures
        public async Task<bool> OpenFuturesPosition(Position signal)
        {
            User users = new User(Db);
            // FIXME: if user active, and Futures only
            var allUsers = await users.GetAllusers();

            // TODO: Implement method to check open positions
            // CheckOpenPosition();

            // TODO: Implement method to close position
            // ClosePosition();

            // TODO: finish USDT-M positions functionality

            // TODO: foreach(var item in allUsers)
            decimal futuresBalance = 0;
            using (var client = new BinanceClient())
            {
                var getFuturesBalance = await client.FuturesUsdt.Account.GetBalanceAsync();
                foreach (var item in getFuturesBalance.Data)
                {
                    if(item.Asset == "USDT")
                        //TODO: Need to think about amount for trade, now it's getting half of deposit
                        futuresBalance = item.AvailableBalance / 2;
                }
                var getSymbolPrice = await client.Spot.Market.GetPriceAsync(signal.Symbol);
                var symbolPrice = getSymbolPrice.Data.Price;
                // TODO: 10X leverage used, should be flexible
                var quantity = (futuresBalance / symbolPrice) * 10;
                
                // FIXME: Need to get symbol precission from binance exchache (0.001m)
                quantity = Math.Round(Math.Abs(quantity), Convert.ToInt32(0.001m));

                var result = await client.FuturesUsdt.Order.PlaceOrderAsync(
                    signal.Symbol, 
                    signal.Direction == 0 ? OrderSide.Buy : OrderSide.Sell, 
                    OrderType.Market, 
                    quantity, //decimal? quantity, 
                    signal.Direction == 0 ? PositionSide.Long : PositionSide.Short, 
                    null, null, null, null, null, null, null,null, null, null, null, null, default
                );

                if(!result.Success)
                {
                    return true;
                }
                else
                {
                    // TODO: log error message
                    TelegramMessenger.SendMessage($"Error: {result.Error.Message}");
                    return false;
                } 
            }
        }
        #endregion

        #region Helper
        private async Task<decimal> GetBalanceOrSymbolAmount(string symbol, int positionSide)
        {
            string[] collection = symbol.Split('U');
            decimal result = 0;

            // FIXME: Decrease time to get balance information
            using(var client = new BinanceClient())
            {
                var data = await client.General.GetAccountInfoAsync();
                if(!data)
                {
                    // TODO: lor error message
                    TelegramMessenger.SendMessage($"Error: {data.Error.Message}");
                }
                
                // positionSide 0 = Buy
                if(positionSide == 0)
                    foreach (var item in data.Data.Balances)
                    {
                        if(item.Asset == "USDT")
                            result = item.Free;
                    }
                else
                    foreach (var item in data.Data.Balances)
                    {
                        if(item.Asset == collection[0])
                            result = item.Free;
                    }

                return result;
            }
        }
        #endregion
    }
}