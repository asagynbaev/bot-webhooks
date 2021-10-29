using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using bot_webhooks.Models;
using System.Net.Http;
using System.Text.Json;
using Binance.Net;
using System;
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
        public string Get()
        {
            return "Hello from WebHookListener!";
        }

        [HttpPost]
        public void Post([FromBody]Position signal)
        {
            //OpenPositions(signal);
            using (var httpClient = new HttpClient())
            {
                string jsonString = JsonSerializer.Serialize(signal);
                var res = httpClient.GetAsync($"https://api.telegram.org/{token}/sendMessage?chat_id={channel}&text={jsonString}").Result;
            }
        }

        public async void OpenPositions(Position signal)
        {
            var client = new BinanceClient();

            //decimal price = BinanceHelpers.FloorPrice(token.TickSize, position.EntryPrice);
            //decimal quantity = (position.Quantity < 0) ? Math.Round(Math.Abs(position.Quantity), Convert.ToInt32(token.QuoteAssetPrecision)) : position.Quantity;
            // var res = await client.Spot.Order.PlaceOrderAsync(
            //     signal.Symbol, 
            //     OrderSide side, 
            //     OrderType type, decimal? quantity = null, decimal? quoteOrderQuantity = null, string? newClientOrderId = null, decimal? price = null, TimeInForce? timeInForce = null, decimal? stopPrice = null, decimal? icebergQty = null, OrderResponseType? orderResponseType = null, int? receiveWindow = null, CancellationToken ct = default
                
            // );

            // if(!res.Success)
            //     System.Console.WriteLine(res.Error.Message);
            // else
            //     System.Console.WriteLine($"Order has been opened: {message}");
        }
    }
}
