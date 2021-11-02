using Binance.Net;
using Binance.Net.Objects;
using CryptoExchange.Net.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace bot_webhooks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Load env variables
            DotNetEnv.Env.Load();            
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
