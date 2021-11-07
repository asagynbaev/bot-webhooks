using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using bot_webhooks.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using bot_webhooks.Helpers;

namespace bot_webhooks.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebHookReceiverController : ControllerBase
    {
        private readonly ILogger<WebHookReceiverController> _logger;
        private readonly WebHookContext Db;

        public WebHookReceiverController(ILogger<WebHookReceiverController> logger, WebHookContext db)
        {
            _logger = logger;
            Db = db;
        }

        [HttpGet]
        public string Get(string symbol)
        {
            return "Hello from Bot WebHooks!!!";
        }

        [HttpPost]
        public void Post([FromBody]Spot signal)
        {
            // var position = new Spot(Db);

            // var query = new Signal(Db, _signalLogger);
            // var statement = await query.GetDataFromDBAsync(signal.Symbol);
            
            // if(statement != null)
            //     await query.UpdateDB(signal.Symbol, signal.Level);
            
            // // Open position ONLY if 3 of BUY or SELL indicators signals
            // // FIXME: Use only one variable to measure level of signal
            // if(statement.BuySignalLevel1 == 1 && statement.BuySignalLevel2 == 1 && statement.BuySignalLevel3 == 1)
            // {
            //     bool positionIsOpen = await position.OpenSpotPosition(signal);
            //     if(positionIsOpen)
            //     {
            //         await query.UpdateDB(signal.Symbol);
            //         TelegramMessenger.SendMessage($"Success: {signal.Symbol} has been bought");
            //     }
            //     else
            //         TelegramMessenger.SendMessage($"Error: failed to buy {signal.Symbol}");
            // } 
            // // FIXME: Use only one variable to measure level of signal
            // if(statement.SellSignalLevel1 == 1 && statement.SellSignalLevel2 == 1 && statement.SellSignalLevel3 == 1)
            // {
            //     bool positionIsOpen = await position.OpenSpotPosition(signal);
            //     if(positionIsOpen)
            //     {
            //         await query.UpdateDB(signal.Symbol);
            //         TelegramMessenger.SendMessage($"Success: {signal.Symbol} has been sold");
            //     }
            //     else
            //         TelegramMessenger.SendMessage($"Error: failed to sell {signal.Symbol}");
            // }
        }
    }
}
