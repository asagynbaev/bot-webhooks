using System;
using System.Net.Http;

namespace bot_webhooks.Helpers
{
    public static class TelegramMessenger
    {
        private static readonly string token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_ID");
        private static readonly string channel = Environment.GetEnvironmentVariable("TELEGRAM_CHANNEL_ID");

        public static void SendMessage(string message)
        {
            using (var httpClient = new HttpClient())
            {
                var res3 = httpClient.GetAsync($"https://api.telegram.org/{token}/sendMessage?chat_id={channel}&text={message}").Result;
            }
        }
    }
}