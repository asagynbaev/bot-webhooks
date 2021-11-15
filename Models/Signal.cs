using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bot_webhooks.Models
{
    [Table("signals")]
    public class Signal
    {
        [Key]
        public int ID { get; set; }
        public string Symbol { get; set; }
        public decimal Price { get; set; }
        public int Direction { get; set; }
        public string SignalType { get; set; }
        public int IsActive { get; set; }
    }
}