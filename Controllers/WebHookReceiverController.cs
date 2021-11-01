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
using System;
using CryptoExchange.Net.Authentication;

namespace bot_webhooks.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebHookReceiverController : ControllerBase
    {
        private readonly ILogger<WebHookReceiverController> _logger; // TODO: use this amazing feature
        private readonly WebHookContext Db;

        // TODO: Implement class for Telegram messaging
        private readonly string token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_ID");
        private readonly string channel = Environment.GetEnvironmentVariable("TELEGRAM_CHANNEL_ID");

        public WebHookReceiverController(ILogger<WebHookReceiverController> logger, WebHookContext db)
        {
            _logger = logger;
            Db = db;
        }

        [HttpGet]
        public async Task<string> Get(string symbol)
        {
            // TODO: Make this message more informative
            var res = await Db.Signals.Where(x => x.Symbol == symbol).ToListAsync();
            int _buySignalLevel = res[0].BuySignalLevel1 + res[0].BuySignalLevel2 + res[0].BuySignalLevel3;
            int _sellSignalLevel = res[0].SellSignalLevel1 + res[0].SellSignalLevel2 + res[0].SellSignalLevel3;
            return $"Buy level is {_buySignalLevel.ToString()} and sell level is {_sellSignalLevel.ToString()}";
        }

        [HttpPost]
        public async Task Post([FromBody]Position signal) // FIXME: Don't forget to rename POsition to Signal
        {
            // TODO: After implementing Futures, replace this method
            //bool futuresIsOpen = await OpenFuturesPosition(signal);

            var query = new Signal(Db);
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
            if(statement.SellSignalLevel1 == 1 && statement.SellSignalLevel2 == 1 && statement.SellSignalLevel3 == 1)
            {
                bool positionIsOpen = await OpenSpotPosition(signal);
                if(positionIsOpen)
                    await query.UpdateDB(signal.Symbol);
                // TODO: send log to TG if something went wrong
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
            
            BinanceClient.SetDefaultOptions(new Binance.Net.Objects.BinanceClientOptions()
            {
                ApiCredentials = new ApiCredentials(
                    "cmz4W8IeYpbKx8W4jZZNS3jPE3gCf4mkYh8FH007Y078IsGyplV3XC7mMhAQWUA7", 
                    "09LGxQZnoEr1N0TIIfPNWJP4YDSCta682hblibY2JAMIwrTxn22UXkNm7OoDQ3NB"
                )
            });
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
        public async Task<bool> OpenFuturesPosition(Position signal)
        {
            // TODO: Implement method to check open positions
            // CheckOpenPosition();

            // TODO: Implement method to close position
            // ClosePosition();

            // TODO: finish USDT-M positions functionality
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
                    signal.PositionSide == 0 ? OrderSide.Buy : OrderSide.Sell, 
                    OrderType.Market, 
                    quantity, //decimal? quantity, 
                    signal.PositionSide == 0 ? PositionSide.Long : PositionSide.Short, 
                    null,
                    null, null, null, null, null, null,null, null, null, null, null, default
                );

                if(!result.Success)
                {
                    System.Console.WriteLine(result.Error.Message); // TODO: log error message
                    return false;
                }
                else
                {
                    using (var httpClient = new HttpClient())
                    {
                        string jsonString = JsonSerializer.Serialize(signal);
                        var res3 = httpClient.GetAsync($"https://api.telegram.org/{token}/sendMessage?chat_id={channel}&text={signal.Symbol} USDT-M Futures position has been opened!").Result;
                    }
                    return true;
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
