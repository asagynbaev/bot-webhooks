using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using bot_webhooks.Models;

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
        public string Get()
        {
            return "Hello from WebHookListener!";
        }

        [HttpPost]
        public async Task<Position> Post([FromBody]Position signal)
        {
            await Db.Connection.OpenAsync();
            signal.Db = Db;
            await signal.InsertAsync();

            Position position = new Position();

            position.Symbol = signal.Symbol;
            position.PositionSide = signal.PositionSide;
            position.EntryPrice = signal.EntryPrice;

            return position;
        }
    }
}
