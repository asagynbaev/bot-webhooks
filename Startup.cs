using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using bot_webhooks.Data;
using bot_webhooks.Models;
using bot_webhooks.Services;
using Microsoft.Extensions.Options;
using Binance.Net.Interfaces;
using Binance.Net;

namespace bot_webhooks
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var optionsBuilder = new DbContextOptionsBuilder<WebHookContext>();
            optionsBuilder.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
            services.AddDbContext<WebHookContext>(options =>
                options.UseNpgsql(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<IUserRepo, UserService>();
            services.AddScoped<ISignalRepo, SignalService>();
            services.AddScoped<ITradeRepo, TradeService>();
            services.AddControllers();
            services.AddSingleton<IBinanceSocketClient, BinanceSocketClient>();
            services.AddTransient<IBinanceClient, BinanceClient>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "bot_webhooks", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "bot_webhooks v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
