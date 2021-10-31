using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using bot_webhooks.Models;
using Binance.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Binance.Net.Enums;

namespace bot_webhooks.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebHookReceiverController : ControllerBase
    {
        private readonly ILogger<WebHookReceiverController> _logger;
        private readonly WebHookContext Db;
        private readonly string token = "bot1339387459:AAG8KH3duliEhV6cuQv8WHQVr4EGFnP0tig", channel = "-1001336600906";

        public WebHookReceiverController(ILogger<WebHookReceiverController> logger, WebHookContext db)
        {
            _logger = logger;
            Db = db;
        }

        [HttpGet]
        public async Task<string> Get(string symbol)
        {
            var res = await Db.Statements.Where(x => x.Symbol == symbol).ToListAsync();
            int _buySignalLevel = res[0].BuySignalLevel1 + res[0].BuySignalLevel2 + res[0].BuySignalLevel3;
            int _sellSignalLevel = res[0].SellSignalLevel1 + res[0].SellSignalLevel2 + res[0].SellSignalLevel3;
            return $"Buy level is {_buySignalLevel.ToString()} and sell level is {_sellSignalLevel.ToString()}";
        }

        [HttpPost]
        public async Task Post([FromBody]Position signal)
        {
            var query = new Statement(Db);
            // Getting actual statement of position
            // TODO: send log to TG if something went wrong
            var statement = await query.GetDataFromDBAsync(signal.Symbol);
            // Changing level of current statement
            // TODO: send log to TG if something went wrong
            await query.UpdateDB(signal.Symbol, signal.Level);
            
            // Open position ONLY if 3 of BUY or SELL indicators signals
            // FIXME: Use only one variable to measure level of signal
            if(statement.BuySignalLevel1 == 1 && statement.BuySignalLevel2 == 1 && statement.BuySignalLevel3 == 1)
            {
                bool positionIsOpen = await OpenSpotPosition(signal);
                if(positionIsOpen)
                    await query.UpdateDB(signal.Symbol);
                // TODO: send log to TG if something went wrong
            } 
            // FIXME: Use only one variable to measure level of signal
            else if(statement.SellSignalLevel1 == 1 && statement.SellSignalLevel2 == 1 && statement.SellSignalLevel3 == 1)
            {
                bool positionIsOpen = await OpenSpotPosition(signal);
                if(positionIsOpen)
                    await query.UpdateDB(signal.Symbol);
                // TODO: send log to TG if something went wrong
            }
            else
            {
                // TODO: Update if else statement
                // using (var httpClient = new HttpClient())
                // {
                //     string jsonString = JsonSerializer.Serialize(signal);
                //     var res3 = httpClient.GetAsync($"https://api.telegram.org/{token}/sendMessage?chat_id={channel}&text={signal.Symbol} spot position will be opened soon!").Result;
                // }
            }
        }

        #region SPOT Trading
        public async Task<bool> OpenSpotPosition(Position signal)
        {
            decimal balance = 0;
            decimal symbolAmount = 0;

            // Get users balance or tokens amount to close position
            if(signal.PositionSide == 0)
                balance = await GetBalanceOrSymbolAmount(signal.Symbol, signal.PositionSide);
            else
                symbolAmount =  await GetBalanceOrSymbolAmount(signal.Symbol, signal.PositionSide);
            
            using (var client = new BinanceClient())
            {
                 var result = await client.Spot.Order.PlaceOrderAsync(
                    signal.Symbol, 
                    signal.PositionSide == 0 ? OrderSide.Buy : OrderSide.Sell,
                    OrderType.Market, 
                    symbolAmount == 0 ? null : symbolAmount, // SymbolAmount
                    balance == 0 ? null : balance, // quoteSymbolAmount
                    null, null, null, null, null, null, null, default // Unnecessary parameters
                );

                if(!result.Success)
                    // TODO: Add logging and troubleshouting logic in case if result = Fail
                    System.Console.WriteLine(result.Error.Message);
                else
                {
                    using (var httpClient = new HttpClient())
                    {
                        string jsonString = JsonSerializer.Serialize(signal);
                        var res3 = httpClient.GetAsync($"https://api.telegram.org/{token}/sendMessage?chat_id={channel}&text={signal.Symbol} spot position has been opened!").Result;
                    }
                }
            }            

            return true;
        }
        #endregion

        #region USDT-M Futures
        public async void OpenFuturesPosition(Position signal)
        {
            // TODO: finish USDT-M positions functionality
            decimal FuturesBalance = 0;
            using (var client = new BinanceClient())
            {
                var futuresRes = await client.FuturesUsdt.Account.GetBalanceAsync();
                var quantity = ((double) FuturesBalance / 100)* 10;
                var SymbolPrice = await client.Spot.Market.GetPriceAsync(signal.Symbol);

                var result = await client.FuturesUsdt.Order.PlaceOrderAsync(
                    signal.Symbol, 
                    signal.PositionSide == 0 ? OrderSide.Buy : OrderSide.Sell, 
                    OrderType.Market, 
                    1000, //decimal? quantity, 
                    signal.PositionSide == 0 ? PositionSide.Long : PositionSide.Short, 
                    TimeInForce.GoodTillCancel,
                    null, null, null, null, null, null,null, null, null, null, null, default
                );

                if(!result.Success)
                    System.Console.WriteLine(result.Error.Message); // TODO: log error message
                else
                {
                    using (var httpClient = new HttpClient())
                    {
                        string jsonString = JsonSerializer.Serialize(signal);
                        var res3 = httpClient.GetAsync($"https://api.telegram.org/{token}/sendMessage?chat_id={channel}&text={signal.Symbol} spot position has been opened!").Result;
                    }
                } 
            }
        }
        #endregion

        #region Helper
        public async Task<decimal> GetBalanceOrSymbolAmount(string symbol, int positionSide)
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
