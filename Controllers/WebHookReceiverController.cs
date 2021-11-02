using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using bot_webhooks.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace bot_webhooks.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebHookReceiverController : ControllerBase
    {
        private readonly ILogger<WebHookReceiverController> _logger;
        private readonly ILogger<Signal> _signalLogger;
        private readonly WebHookContext Db;

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
            _logger.LogInformation("I've just got information from database");
            return $"Buy level is {_buySignalLevel.ToString()} and sell level is {_sellSignalLevel.ToString()}";
        }

        [HttpPost]
        public async Task Post([FromBody]Position signal)
        {
            var position = new Position(Db);
            // TODO: After implementing Futures, replace this method
            //bool futuresIsOpen = await OpenFuturesPosition(signal);

            var query = new Signal(Db, _signalLogger);
            var statement = await query.GetDataFromDBAsync(signal.Symbol);
            
            if(statement != null)
                await query.UpdateDB(signal.Symbol, signal.Level);
            
            // Open position ONLY if 3 of BUY or SELL indicators signals
            // FIXME: Use only one variable to measure level of signal
            if(statement.BuySignalLevel1 == 1 && statement.BuySignalLevel2 == 1 && statement.BuySignalLevel3 == 1)
            {
                bool positionIsOpen = await position.OpenSpotPosition(signal);
                if(positionIsOpen)
                {
                    await query.UpdateDB(signal.Symbol);
                    TelegramMessenger.SendMessage($"Success: {signal.Symbol} has been bought");
                }
                else
                    TelegramMessenger.SendMessage($"Error: failed to buy {signal.Symbol}");
            } 
            // FIXME: Use only one variable to measure level of signal
            if(statement.SellSignalLevel1 == 1 && statement.SellSignalLevel2 == 1 && statement.SellSignalLevel3 == 1)
            {
                bool positionIsOpen = await position.OpenSpotPosition(signal);
                if(positionIsOpen)
                {
                    await query.UpdateDB(signal.Symbol);
                    TelegramMessenger.SendMessage($"Success: {signal.Symbol} has been sold");
                }
                else
                    TelegramMessenger.SendMessage($"Error: failed to sell {signal.Symbol}");
            }
        }
    }
}
