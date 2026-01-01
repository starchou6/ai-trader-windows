namespace AITrade.Entity
{
    public class CompletedTrade
    {
        public string Id { get; set; }
        public string Side { get; set; }
        public string Coin { get; set; }
        public int Leverage { get; set; }
        public decimal Quantity { get; set; }
        public decimal EntryPrice { get; set; }
        public decimal CompletedPrice { get; set; }
        public decimal EntryAmount { get; set; }
        public decimal CompletedAmount { get; set; }
        public decimal RealizedProfit { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
