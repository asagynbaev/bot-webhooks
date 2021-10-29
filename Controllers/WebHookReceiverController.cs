﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using bot_webhooks.Models;
using Binance.Net;
using Binance.Net.Enums;
using System.Net.Http;
using System.Text.Json;

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
        public string Get()
        {
            return "Hello from WebHookListener!";
        }

        [HttpPost]
        public async void Post([FromBody]Position signal)
        {
            decimal USDT = 0;
            decimal SymbolAmount = 0;

            var client = new BinanceClient();
            var data = await client.General.GetAccountInfoAsync();
            if(signal.PositionSide == 0)
            {
                foreach (var item in data.Data.Balances)
                {
                    if(item.Asset == "USDT")
                        USDT = item.Free;
                }
            }
            else
            {
                string[] collection = signal.Symbol.Split('U');
                foreach (var item in data.Data.Balances)
                {
                    if(item.Asset == collection[0])
                        SymbolAmount = item.Free;
                }
            }
            if(signal.PositionSide == 1 && SymbolAmount < 1)
            {
                // написать логирование для ложных сигналов
            }
            else if(signal.PositionSide == 0 && USDT < 1)
            {
                // повторная покупка актива, который сработал ложно
            }
            else
            {
                var res = await client.Spot.Order.PlaceOrderAsync(
                    signal.Symbol, 
                    signal.PositionSide == 0 ? OrderSide.Buy : OrderSide.Sell,
                    OrderType.Market, 
                    SymbolAmount == 0 ? null : SymbolAmount, // SymbolAmount
                    USDT == 0 ? null : USDT, // quoteSymbolAmount
                    null, null, null, null, null, null, null, default
                );

                // if(!res.Success)
                //     System.Console.WriteLine(res.Error.Message);
                // else
                //     System.Console.WriteLine($"Order has been opened: {message}");
            }
            using (var httpClient = new HttpClient())
            {
                string jsonString = JsonSerializer.Serialize(signal);
                var res2 = httpClient.GetAsync($"https://api.telegram.org/{token}/sendMessage?chat_id={channel}&text={jsonString}").Result;
            }
        }
    }
}
