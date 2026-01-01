using System.Text.Json.Serialization;

namespace AITrade.Entity.AI
{
    public class OIPosition
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }


        [JsonPropertyName("rank")]
        public int Rank { get; set; }


        [JsonPropertyName("current_oi")]
        public double CurrentOI { get; set; }


        [JsonPropertyName("oi_delta")]
        public double OIDelta { get; set; }


        [JsonPropertyName("oi_delta_percent")]
        public double OIDeltaPercent { get; set; }


        [JsonPropertyName("oi_delta_value")]
        public double OIDeltaValue { get; set; }


        [JsonPropertyName("price_delta_percent")]
        public double PriceDeltaPercent { get; set; }


        [JsonPropertyName("net_long")]
        public double NetLong { get; set; }


        [JsonPropertyName("net_short")]
        public double NetShort { get; set; }
    }


    public class OITopApiResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }


        [JsonPropertyName("data")]
        public OIPositionTopData Data { get; set; }
    }


    public class OIPositionTopData
    {
        [JsonPropertyName("positions")]
        public List<OIPosition> Positions { get; set; }


        [JsonPropertyName("count")]
        public int Count { get; set; }


        [JsonPropertyName("exchange")]
        public string Exchange { get; set; }


        [JsonPropertyName("time_range")]
        public string TimeRange { get; set; }
    }

    public class OITopCache
    {
        public List<OIPosition> Positions { get; set; }
        public DateTime FetchedAt { get; set; }
        public string SourceType { get; set; }
    }
}
