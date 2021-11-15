using System;
using Binance.Net;
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
            BinanceClient.SetDefaultOptions(new Binance.Net.Objects.BinanceClientOptions()
            {
                ApiCredentials = new ApiCredentials(
                    Environment.GetEnvironmentVariable("API_KEY"), 
                    Environment.GetEnvironmentVariable("API_SECRET")
                )
            });          
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
