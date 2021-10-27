using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace bot_webhooks.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebHookReceiverController : ControllerBase
    {

        private readonly ILogger<WebHookReceiverController> _logger;

        public WebHookReceiverController(ILogger<WebHookReceiverController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public void Post(Position signal)
        {
            Position position = new Position();

            position.Symbol = signal.Symbol;
            position.EntryPrice = signal.EntryPrice;
            position.Leverage = 10;
        }
    }
}
