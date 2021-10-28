﻿using System.Threading.Tasks;
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
        public async void Post([FromBody]Position signal)
        {
            using (var httpClient = new HttpClient())
            {
                string jsonString = JsonSerializer.Serialize(signal);
                var res = httpClient.GetAsync($"https://api.telegram.org/{token}/sendMessage?chat_id={channel}&text={jsonString}").Result;
            }

            signal.Db = Db;
            await signal.InsertAsync();
        }
    }
}
