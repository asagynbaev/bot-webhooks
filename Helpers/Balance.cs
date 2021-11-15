using System.Threading.Tasks;
using Binance.Net;

namespace bot_webhooks.Helpers
{
    public static class Balance
    {
        public static async Task<decimal> GetBalance()
        {
            decimal balance = 0;
            using(var client = new BinanceClient())
            {
                var data = await client.General.GetAccountInfoAsync();
                
                if(data.Data.Balances != null)
                    foreach (var item in data.Data.Balances)
                    {
                        if(item.Asset == "USDT")
                            balance = item.Free;
                    }
                else
                    await TelegramMessenger.SendMessage($"Error: {data.Error.Message}");
            }
            return balance;
        }

        public static async Task<decimal> GetTokensAmount(string symbol)
        {
            decimal tokensAmount = 0;        
            using(var client = new BinanceClient())
            {
                var data = await client.General.GetAccountInfoAsync();
                if(data.Data.Balances != null)
                    foreach (var item in data.Data.Balances)
                    {
                        if(item.Asset == symbol)
                            tokensAmount = item.Free;
                    }
                else
                    await TelegramMessenger.SendMessage($"Error: {data.Error.Message}");
            }
            return tokensAmount;
        }
    }
}