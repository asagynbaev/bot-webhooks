using Binance.Net.Clients;
using Binance.Net.Interfaces.Clients;
using Binance.Net.Objects.Options;
using Bybit.Net.Clients;
using Bybit.Net.Interfaces.Clients;
using Bybit.Net.Objects.Options;
using CryptoExchange.Net.Authentication;
using Microsoft.EntityFrameworkCore;
using bot_webhooks.Data;
using bot_webhooks.Services;

DotNetEnv.Env.Load();

// Binance
var binanceApiKey = Environment.GetEnvironmentVariable("BINANCE_API_KEY") ?? "";
var binanceSecret = Environment.GetEnvironmentVariable("BINANCE_SECRET") ?? "";
BinanceRestClient.SetDefaultOptions(options =>
{
	options.ApiCredentials = new ApiCredentials(binanceApiKey, binanceSecret);
});

// Bybit (optional: leave empty to skip Bybit endpoints)
var bybitApiKey = Environment.GetEnvironmentVariable("BYBIT_API_KEY") ?? "";
var bybitSecret = Environment.GetEnvironmentVariable("BYBIT_SECRET") ?? "";
BybitRestClient.SetDefaultOptions(options =>
{
	options.ApiCredentials = new ApiCredentials(bybitApiKey, bybitSecret);
});

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<WebHookContext>(options =>
	options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUserRepo, UserService>();
builder.Services.AddScoped<ISignalRepo, SignalService>();
builder.Services.AddScoped<ITradeRepo, TradeService>();
builder.Services.AddSingleton<IBinanceBalanceProvider, BinanceBalanceProvider>();
builder.Services.AddSingleton<IBinanceRestClient, BinanceRestClient>();
builder.Services.AddSingleton<IBybitRestClient, BybitRestClient>();
builder.Services.AddSingleton<IBybitAccountService, BybitAccountService>();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "bot_webhooks", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
	app.UseSwagger();
	app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "bot_webhooks v1"));
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
