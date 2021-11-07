using System;
using System.Threading.Tasks;
using Binance.Net;
using Binance.Net.Enums;
using bot_webhooks.Helpers;
using CryptoExchange.Net.Authentication;

namespace bot_webhooks.Models
{
    class Futures : Position
    {
        internal WebHookContext Db { get; set; }
        public Futures()
        {
        }

        public Futures(WebHookContext db)
        {
            Db = db;
        }
        
        private decimal Balance { get; set; }
        private decimal TokensAmount { get; set; }
        
        public override async Task<bool> OpenPosition(Signal signal)
        {
            throw new NotImplementedException();
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
            // using (var client = new BinanceClient())
            // {
            //     var getFuturesBalance = await client.FuturesUsdt.Account.GetBalanceAsync();
            //     foreach (var item in getFuturesBalance.Data)
            //     {
            //         if(item.Asset == "USDT")
            //             //TODO: Need to think about amount for trade, now it's getting half of deposit
            //             futuresBalance = item.AvailableBalance / 2;
            //     }
            //     var getSymbolPrice = await client.Spot.Market.GetPriceAsync(signal.Symbol);
            //     var symbolPrice = getSymbolPrice.Data.Price;
            //     // TODO: 10X leverage used, should be flexible
            //     var quantity = (futuresBalance / symbolPrice) * 10;
                
            //     // FIXME: Need to get symbol precission from binance exchache (0.001m)
            //     // TODO: Get ticksize from signal
            //     quantity = Math.Round(Math.Abs(quantity), Convert.ToInt32(0.001m));

            //     var result = await client.FuturesUsdt.Order.PlaceOrderAsync(
            //         signal.Symbol, 
            //         OrderSide.Buy, 
            //         OrderType.Market, 
            //         quantity, //decimal? quantity, 
            //         signal.Direction == 0 ? PositionSide.Long : PositionSide.Short, 
            //         null, null, null, null, null, null, null,null, null, null, null, null, default
            //     );

            //     if(!result.Success)
            //     {
            //         return true;
            //     }
            //     else
            //     {
            //         // TODO: log error message
            //         TelegramMessenger.SendMessage($"Error: {result.Error.Message}");
            //         return false;
            //     } 
            // }
        }

        public override async Task<bool> ClosePosition(Signal signal)
        {
            using (var client = new BinanceClient())
            {
                var result = await client.Spot.Order.PlaceOrderAsync(
                    signal.Symbol, 
                    OrderSide.Sell,
                    OrderType.Market, 
                    TokensAmount == 0 ? null : TokensAmount, // SymbolAmount
                    Balance == 0 ? null : Balance, // quoteSymbolAmount
                    null, null, null, null, null, null, null, default // Unnecessary parameters
                );
            }
            return false;
        }

        public async override void GetBalance(string apiKey, string secret)
        {
            BinanceClient.SetDefaultOptions(new Binance.Net.Objects.BinanceClientOptions()
            {
                ApiCredentials = new ApiCredentials(apiKey, secret)
            });

            using(var client = new BinanceClient())
            {
                var data = await client.General.GetAccountInfoAsync();
                if(!data)
                {
                    // TODO: lor error message
                    TelegramMessenger.SendMessage($"Error: {data.Error.Message}");
                }

                foreach (var item in data.Data.Balances)
                {
                    if(item.Asset == "USDT")
                        Balance = item.Free;
                }
            }
        }

        public async override void GetCurrentPosition(string symbol, string apiKey, string secret)
        {
            string[] collection = symbol.Split('U');

            BinanceClient.SetDefaultOptions(new Binance.Net.Objects.BinanceClientOptions()
            {
                ApiCredentials = new ApiCredentials(apiKey, secret)
            });

            // FIXME: Decrease time to get balance information
            using(var client = new BinanceClient())
            {
                var data = await client.General.GetAccountInfoAsync();
                if(!data)
                {
                    // TODO: lor error message
                    TelegramMessenger.SendMessage($"Error: {data.Error.Message}");
                }

                foreach (var item in data.Data.Balances)
                {
                    if(item.Asset == collection[0])
                        TokensAmount = item.Free;
                }
            }
        }
    }
}