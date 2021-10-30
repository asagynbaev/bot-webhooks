namespace bot_webhooks.Models
{
    public class Position
    {
        public string Symbol { get; set; }
        public int PositionSide { get; set; }
        //public decimal EntryPrice { get; set; }
        public int Level { get; set; }
    }
}