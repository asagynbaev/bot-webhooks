public class Position
    {
        //     Symbol
        public string Symbol { get; set; }
        //     Entry price
        public decimal EntryPrice { get; set; }
        //     Leverage
        public int Leverage { get; set; }
        //     Position side
        public int PositionSide { get; set; }
        //     Quantity
        public decimal Quantity { get; set; }
    }