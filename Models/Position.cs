namespace bot_webhooks.Models
{
    // FIXME: Rename this class to "Signal", it's not position :(
    public class Position
    {
        public string Symbol { get; set; }
        public int PositionSide { get; set; }
        public int Level { get; set; }
    }
}