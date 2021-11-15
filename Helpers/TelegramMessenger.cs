using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace bot_webhooks.Helpers
{
    public static class TelegramMessenger
    {
        private static readonly string token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_ID");
        private static readonly string channel = Environment.GetEnvironmentVariable("TELEGRAM_CHANNEL_ID");

        public async static Task<HttpResponseMessage> SendMessage(string message)
        {
            using (var httpClient = new HttpClient())
            {
                return await httpClient.GetAsync($"https://api.telegram.org/{token}/sendMessage?chat_id={channel}&text={message}");
            }
        }
    }
}