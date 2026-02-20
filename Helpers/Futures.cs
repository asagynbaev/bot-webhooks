using Binance.Net.Enums;
using Binance.Net.Interfaces.Clients;
using bot_webhooks.Data;
using bot_webhooks.Models;
using Microsoft.EntityFrameworkCore;

namespace bot_webhooks.Helpers;

public class Futures(WebHookContext db, IBinanceRestClient binanceClient)
{
	private readonly WebHookContext _db = db;
	private readonly IBinanceRestClient _binanceClient = binanceClient;
	private decimal Balance { get; set; }
	private decimal TokensAmount { get; set; }

	public async Task OpenPositionAsync(Signal signal)
	{
		_ = await _db.Users.ToListAsync();
		// TODO: Implement method to check open positions
		// TODO: Implement method to close position
		// TODO: finish USDT-M positions functionality
	}

	public async Task ClosePositionAsync(Signal signal)
	{
		_ = await _binanceClient.SpotApi.Trading.PlaceOrderAsync(
			signal.Symbol!,
			OrderSide.Sell,
			SpotOrderType.Market,
			quantity: TokensAmount == 0 ? null : TokensAmount,
			quoteQuantity: Balance == 0 ? null : Balance);
	}
}
