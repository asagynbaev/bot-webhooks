using System.Threading.Tasks;
using Binance.Net;
using Binance.Net.Enums;
using bot_webhooks.Helpers;
using CryptoExchange.Net.Authentication;

namespace bot_webhooks.Models
{
    public class Spot : Position
    {
        internal WebHookContext Db { get; set; }
        public Spot()
        {
        }

        public Spot(WebHookContext db)
        {
            Db = db;
        }

        private decimal Balance { get; set; }
        private decimal TokensAmount { get; set; }

        public override async Task<bool> OpenPosition(Signal signal)
        {
            bool taskResult = false;
            User users = new User(Db);
            // FIXME: if user is active, and Spot only
            var allUsers = await users.GetAllusers();

            try
            {
                 foreach (var item in allUsers)
                {
                    BinanceClient.SetDefaultOptions(new Binance.Net.Objects.BinanceClientOptions()
                    {
                        ApiCredentials = new ApiCredentials(item.ApiKey, item.Secret)
                    });

                    // if(signal.Direction == 0)
                    // {
                    //     balance = await GetBalanceOrSymbolAmount(signal.Symbol, signal.Direction, item.ApiKey, item.Secret);
                    //     if(balance < 1)
                    //         TelegramMessenger.SendMessage($"False alarm: {signal.Symbol} already bought");
                    // }
                    // else
                    // {
                    //     symbolAmount =  await GetBalanceOrSymbolAmount(signal.Symbol, signal.Direction, item.ApiKey, item.Secret);
                    //         if(symbolAmount < 1)
                    //             TelegramMessenger.SendMessage($"False alarm: {signal.Symbol} already sold");
                    // }

                    using (var client = new BinanceClient())
                    {
                        var result = await client.Spot.Order.PlaceOrderAsync(
                            signal.Symbol, 
                            OrderSide.Buy,
                            OrderType.Market, 
                            TokensAmount == 0 ? null : TokensAmount, // SymbolAmount
                            Balance == 0 ? null : Balance, // quoteSymbolAmount
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
            catch (System.Exception ex)
            {
                TelegramMessenger.SendMessage($"Error: {ex.Message}");
                return false;
            }
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
            // TODO: Get currency from signal "USDT"
            string[] collection = symbol.Split('U');

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
                    if(item.Asset == collection[0])
                        TokensAmount = item.Free;
                }
            }
        }
    }
}