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
            
            //FIXME: Make this credentials personal for each user
            BinanceClient.SetDefaultOptions(new BinanceClientOptions()
            {
                ApiCredentials = new ApiCredentials(
                    "cmz4W8IeYpbKx8W4jZZNS3jPE3gCf4mkYh8FH007Y078IsGyplV3XC7mMhAQWUA7", 
                    "09LGxQZnoEr1N0TIIfPNWJP4YDSCta682hblibY2JAMIwrTxn22UXkNm7OoDQ3NB"
                ),
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
