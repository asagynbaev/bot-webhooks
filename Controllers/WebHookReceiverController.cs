using Microsoft.AspNetCore.Mvc;
using bot_webhooks.Data;
using bot_webhooks.Services;

namespace bot_webhooks.Controllers;

[ApiController]
[Route("[controller]")]
public class WebHookReceiverController(
	ITradeRepo trade,
	IBybitAccountService bybitAccount,
	ITelegramChannelMessageStore telegramMessages) : ControllerBase
{
	/// <summary>Binance USDT-M futures account info.</summary>
	[HttpGet("Info")]
	public async Task<IActionResult> GetBinanceInfoAsync(CancellationToken cancellationToken) =>
		Ok(await trade.BinanceAccountInfoAsync(cancellationToken));

	/// <summary>Bybit unified account wallet balance (V5).</summary>
	[HttpGet("BybitInfo")]
	public async Task<IActionResult> GetBybitInfoAsync(CancellationToken cancellationToken)
	{
		var info = await bybitAccount.GetAccountInfoAsync(cancellationToken);
		return info != null ? Ok(info) : NotFound("Bybit account info unavailable (check API keys or connection).");
	}

	/// <summary>Last messages received from the configured Telegram signal channel (userbot).</summary>
	/// <param name="count">Number of messages to return (default 50, max 500).</param>
	[HttpGet("TelegramMessages")]
	public IActionResult GetTelegramMessages([FromQuery] int count = 50) =>
		Ok(telegramMessages.GetLast(Math.Clamp(count, 1, 500)));
}
