using Microsoft.AspNetCore.Mvc;
using bot_webhooks.Models;
using bot_webhooks.Data;

namespace bot_webhooks.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebHookReceiverController : ControllerBase
    {
        ISignalRepo _signal;
        ITradeRepo _trade;

        public WebHookReceiverController(ISignalRepo signalDb, ITradeRepo trade) => (_signal, _trade) = (signalDb, trade);

        [HttpGet]
        public string Get(string symbol) => "Hello from Bot WebHooks!!!";

        [HttpPost]
        public void Post([FromBody] Signal signal)
        {
            // if(signal.Direction == 0)
            // {
            //     var result = _signal.GetSignal(signal.Symbol, signal.Direction);
            //     if(result == null)
            //     {
            //         signal.IsActive = 1;
            //         int signalId = _signal.InsertSignal(signal).Result;
            //         int tradeId = _trade.InsertTrade(signal, signalId).Result;
            //     }
            // }
            // else
            // {
            //     var result = _signal.GetSignal(signal.Symbol, signal.Direction);
            //     if(result == null)
            //     {
            //         signal.IsActive = 1;
            //         int signalId = _signal.InsertSignal(signal).Result;
                    
            //     }
            // }
        }
    }
}
