using Binance.Net.Interfaces.Clients;
using bot_webhooks.Helpers;

namespace bot_webhooks.Services;

public class BinanceBalanceProvider(IBinanceRestClient client) : IBinanceBalanceProvider
{
	private readonly IBinanceRestClient _client = client;

	public async Task<decimal> GetBalanceAsync(CancellationToken cancellationToken = default)
	{
		var data = await _client.SpotApi.Account.GetAccountInfoAsync(null, null, cancellationToken);
		if (!data.Success || data.Data?.Balances == null)
		{
			if (data.Error != null)
				await TelegramMessenger.SendMessage($"Error: {data.Error.Message}");
			return 0;
		}
		var usdt = data.Data.Balances.FirstOrDefault(b => b.Asset == "USDT");
		return usdt?.Available ?? 0;
	}

	public async Task<decimal> GetTokensAmountAsync(string symbol, CancellationToken cancellationToken = default)
	{
		var data = await _client.SpotApi.Account.GetAccountInfoAsync(null, null, cancellationToken);
		if (!data.Success || data.Data?.Balances == null)
		{
			if (data.Error != null)
				await TelegramMessenger.SendMessage($"Error: {data.Error.Message}");
			return 0;
		}
		var asset = data.Data.Balances.FirstOrDefault(b => b.Asset == symbol);
		return asset?.Available ?? 0;
	}
}
