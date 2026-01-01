namespace AITrade.Entity
{
    public class PositionData
    {
        public string Id { get; set; }
        public string Side { get; set; }
        public string Symbol { get; set; }
        public int Leverage { get; set; }
        public decimal Quantity { get; set; }
        public decimal EntryPrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal EntryAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public decimal UnrealizedProfit { get; set; }
        public decimal LiquidationPrice { get; set; }
    }
}
