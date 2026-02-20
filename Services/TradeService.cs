using Binance.Net.Enums;
using Binance.Net.Interfaces.Clients;
using bot_webhooks.Data;
using bot_webhooks.Helpers;
using bot_webhooks.Models;

namespace bot_webhooks.Services;

public class TradeService(
	WebHookContext db,
	IBinanceRestClient client,
	IBinanceBalanceProvider balanceProvider) : ITradeRepo
{
	private readonly WebHookContext _db = db;
	private readonly IBinanceRestClient _client = client;
	private readonly IBinanceBalanceProvider _balanceProvider = balanceProvider;

	public async Task<BinanceFutureAccountInfo> BinanceAccountInfoAsync(CancellationToken cancellationToken = default)
	{
		var response = await _client.UsdFuturesApi.Account.GetAccountInfoV2Async(null, cancellationToken);
		if (!response.Success || response.Data == null)
			return new BinanceFutureAccountInfo { Id = Guid.NewGuid() };

		var d = response.Data;
		return new BinanceFutureAccountInfo
		{
			Id = Guid.NewGuid(),
			CanDeposit = d.CanDeposit,
			CanTrade = d.CanTrade,
			CanWithdraw = d.CanWithdraw,
			FeeTier = d.FeeTier,
			MaxWithdrawAmount = (double)d.MaxWithdrawQuantity,
			TotalInitialMargin = (double)d.TotalInitialMargin,
			TotalMaintMargin = (double)d.TotalMaintMargin,
			TotalMarginBalance = (double)d.TotalMarginBalance,
			TotalOpenOrderInitialMargin = (double)d.TotalOpenOrderInitialMargin,
			TotalPositionInitialMargin = (double)d.TotalPositionInitialMargin,
			TotalUnrealizedProfit = (double)d.TotalUnrealizedProfit,
			TotalWalletBalance = (double)d.TotalWalletBalance,
			TotalCrossWalletBalance = (double)d.TotalCrossWalletBalance,
			TotalCrossUnPnl = (double)d.TotalCrossUnPnl,
			AvailableBalance = (double)d.AvailableBalance,
			updateTime = d.UpdateTime.HasValue ? (int)(d.UpdateTime.Value - DateTime.UnixEpoch).TotalSeconds : 0
		};
	}

	public Trade? GetTrade(string symbol, int direction) =>
		_db.Trades
			.FirstOrDefault(x => x.Symbol == symbol && x.Direction == direction);

	public async Task<int> InsertTrade(Signal signal, int signalId)
	{
		var newTrade = new Trade
		{
			Price = signal.Price,
			SignalID = signalId,
			Symbol = signal.Symbol,
			Direction = signal.Direction,
			SignalType = signal.SignalType
		};

		decimal myBalance = 0;
		decimal tokensAmount = 0;

		if (signal.Direction == 0)
			myBalance = await _balanceProvider.GetBalanceAsync();
		else
			tokensAmount = await _balanceProvider.GetTokensAmountAsync(signal.Symbol!);

		var result = await _client.SpotApi.Trading.PlaceOrderAsync(
			signal.Symbol!,
			(OrderSide)signal.Direction,
			SpotOrderType.Market,
			quantity: tokensAmount == 0 ? null : tokensAmount,
			quoteQuantity: myBalance == 0 ? null : myBalance);

		if (result.Success && result.Data?.Trades != null)
		{
			foreach (var item in result.Data.Trades)
			{
				newTrade.BoughtPrice = item.Price;
				newTrade.BuyQuoteCommission = item.Fee;
				newTrade.Amount = item.Quantity;
			}
		}
		else
		{
			await TelegramMessenger.SendMessage($"Error: {result.Error?.Message ?? "Unknown"}");
		}

		await _db.AddAsync(newTrade);
		await _db.SaveChangesAsync();

		await TelegramMessenger.SendMessage($"Bought: {newTrade.Symbol}, Price: {newTrade.BoughtPrice}");

		return newTrade.ID;
	}

	public Trade? UpdateTrade(int ID) => throw new NotImplementedException();
}
