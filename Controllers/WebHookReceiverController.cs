using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using bot_webhooks.Models;
using System.Net.Http;
using System.Net;

namespace bot_webhooks.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebHookReceiverController : ControllerBase
    {
        
        private readonly ILogger<WebHookReceiverController> _logger;
        private readonly WebHookContext Db;
        private readonly string token = "1339387459:AAG8KH3duliEhV6cuQv8WHQVr4EGFnP0tig", channel = "1001336600906";

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
            using (var httpClient = new HttpClient())
            {
                var res = httpClient.GetAsync($"https://api.telegram.org/bot{token}/sendMessage?chat_id={channel}&text=ololo").Result;
                if (res.StatusCode == HttpStatusCode.OK)
                { /* done, go check your channel */ }
                else
                { /* something went wrong */ }
            }

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
