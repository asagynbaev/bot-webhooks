using System;

namespace bot_webhooks.Models
{
    public class BinanceFutureAccountInfo
    {
        public Guid Id { get; set; }
        public bool CanDeposit { get; set; }
        public bool CanTrade { get; set; }
        public bool CanWithdraw { get; set; }
        public int FeeTier { get; set; }
        public double MaxWithdrawAmount { get; set; }
        public double TotalInitialMargin { get; set; }
        public double TotalMaintMargin { get; set; }
        public double TotalMarginBalance { get; set; }
        public double TotalOpenOrderInitialMargin { get; set; }
        public double TotalPositionInitialMargin { get; set; }
        public double TotalUnrealizedProfit { get; set; }
        public double TotalWalletBalance { get; set; }
        public double TotalCrossWalletBalance { get; set; }
        public double TotalCrossUnPnl { get; set; }
        public double AvailableBalance { get; set; }
        public int updateTime { get; set; }
    }
}
