using Microsoft.AspNetCore.Mvc;
using bot_webhooks.Models;
using bot_webhooks.Data;
using System.Threading.Tasks;

namespace bot_webhooks.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebHookReceiverController : ControllerBase
    {
        ISignalRepo _signal;
        ITradeRepo _trade;

        public WebHookReceiverController(ISignalRepo signalDb, ITradeRepo trade) => (_signal, _trade) = (signalDb, trade);

        [HttpGet("Info")]
        public async Task<ActionResult> GetInfoAsync() => Ok(await _trade.BinanceAccountInfo());
    }
}
