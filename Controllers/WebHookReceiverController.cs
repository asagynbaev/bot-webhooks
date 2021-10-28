using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using bot_webhooks.Models;
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
        public void Post([FromBody]string signal)
        {
            using (var httpClient = new HttpClient())
            {
                //string jsonString = JsonSerializer.Serialize(signal);
                var res = httpClient.GetAsync($"https://api.telegram.org/{token}/sendMessage?chat_id={channel}&text={signal}").Result;
            }

            // signal.Db = Db;
            // await signal.InsertAsync();

            // Position position = new Position();

            // position.Symbol = signal.Symbol;
            // position.PositionSide = signal.PositionSide;
            // position.EntryPrice = signal.EntryPrice;

            //return position;
        }
    }
}
