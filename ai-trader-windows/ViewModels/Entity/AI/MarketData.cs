namespace AITrade.Entity.AI
{
    public class MarketData
    {
        public string Symbol { get; set; }
        public double CurrentPrice { get; set; }
        public double PriceChange1h { get; set; }
        public double PriceChange4h { get; set; }
        public double CurrentEMA20 { get; set; }
        public double CurrentMACD { get; set; }
        public double CurrentRSI7 { get; set; }
        public OIData OpenInterest { get; set; }
        public double FundingRate { get; set; }
        public IntradayData IntradaySeries { get; set; }
        public LongerTermData LongerTermContext { get; set; }
    }

    public class OIData
    {
        public double Latest { get; set; }
        public double Average { get; set; }
    }

    public class IntradayData
    {
        public List<double> MidPrices { get; set; }
        public List<double> EMA20Values { get; set; }
        public List<double> MACDValues { get; set; }
        public List<double> RSI7Values { get; set; }
        public List<double> RSI14Values { get; set; }
    }

    public class LongerTermData
    {
        public double EMA20 { get; set; }
        public double EMA50 { get; set; }
        public double ATR3 { get; set; }
        public double ATR14 { get; set; }
        public double CurrentVolume { get; set; }
        public double AverageVolume { get; set; }
        public List<double> MACDValues { get; set; }
        public List<double> RSI14Values { get; set; }
    }

    public class Kline
    {
        public long OpenTime { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double Volume { get; set; }
        public long CloseTime { get; set; }
    }
}
