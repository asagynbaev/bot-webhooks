using Microsoft.AspNetCore.Mvc;
using bot_webhooks.Models;
using System.Net.Http;
using System.Text.Json;

namespace bot_webhooks.Controllers
{
    public class TestSignal
    {
        public string Symbol {get;set;}
        public string Direction {get;set;}
        public decimal Price {get;set;}
    }
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly string token = "bot1339387459:AAG8KH3duliEhV6cuQv8WHQVr4EGFnP0tig", channel = "-1001336600906";

        public TestController()
        {
        }

        [HttpGet]
        public string Get(string symbol)
        {
            return "Test controller";
        }

        [HttpPost]
        public void Post([FromBody]TestSignal signal)
        {
            //string asdf = signal.ToString();
            using (var httpClient = new HttpClient())
            {
                string jsonString = JsonSerializer.Serialize(signal);
                var res3 = httpClient.GetAsync($"https://api.telegram.org/{token}/sendMessage?chat_id={channel}&text={jsonString}").Result;
            }
        }
    }
}
