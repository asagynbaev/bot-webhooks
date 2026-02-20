using Bybit.Net.Clients;
using Bybit.Net.Enums;
using Bybit.Net.Interfaces.Clients;
using bot_webhooks.Models;

namespace bot_webhooks.Services;

public class BybitAccountService(IBybitRestClient client) : IBybitAccountService
{
	private readonly IBybitRestClient _client = client;

	public async Task<BybitAccountInfo?> GetAccountInfoAsync(CancellationToken cancellationToken = default)
	{
		var response = await _client.V5Api.Account.GetBalancesAsync(AccountType.Unified, null, cancellationToken);
		if (!response.Success || response.Data?.List == null || response.Data.List.Length == 0)
			return null;

		var b = response.Data.List[0];
		return new BybitAccountInfo
		{
			AccountType = b.AccountType.ToString(),
			TotalEquity = b.TotalEquity ?? 0,
			TotalWalletBalance = b.TotalWalletBalance ?? 0,
			TotalMarginBalance = b.TotalMarginBalance ?? 0,
			TotalAvailableBalance = b.TotalAvailableBalance ?? 0,
			TotalPerpUnrealizedPnl = b.TotalPerpUnrealizedPnl ?? 0,
			TotalInitialMargin = b.TotalInitialMargin ?? 0,
			TotalMaintenanceMargin = b.TotalMaintenanceMargin ?? 0
		};
	}
}
