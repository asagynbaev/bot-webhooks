namespace bot_webhooks.Models;

/// <summary>
/// DTO for Bybit unified account wallet balance (V5 API).
/// </summary>
public class BybitAccountInfo
{
	public string AccountType { get; set; } = "UNIFIED";
	public decimal TotalEquity { get; set; }
	public decimal TotalWalletBalance { get; set; }
	public decimal TotalMarginBalance { get; set; }
	public decimal TotalAvailableBalance { get; set; }
	public decimal TotalPerpUnrealizedPnl { get; set; }
	public decimal TotalInitialMargin { get; set; }
	public decimal TotalMaintenanceMargin { get; set; }
}
