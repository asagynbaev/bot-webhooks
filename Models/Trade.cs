using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bot_webhooks.Models
{
    [Table("trades")]
    public class Trade
    {
        [Key]
        public int ID { get; set; }
        public int SignalID { get; set; }
        public decimal Price { get; set; }
        public string Symbol { get; set; }
        public int Direction { get; set; }
        public string SignalType { get; set; }
        public decimal BoughtPrice { get; set; }
        public decimal Amount { get; set; }
        public decimal BuyQuoteCommission { get; set; }
        public decimal SellPrice { get; set; }
        public decimal SellUSDTCommission { get; set; }
        public decimal PNL { get; set; }
    }
}