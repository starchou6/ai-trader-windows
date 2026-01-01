using System.Text.Json.Serialization;

namespace AITrade.Entity.AI
{
    public class ContextData
    {
        [JsonPropertyName("current_time")]
        public string CurrentTime { get; set; }

        [JsonPropertyName("runtime_minutes")]
        public int RuntimeMinutes { get; set; }

        [JsonPropertyName("call_count")]
        public int CallCount { get; set; }

        [JsonPropertyName("account")]
        public AccountInfo Account { get; set; }

        [JsonPropertyName("positions")]
        public List<PositionInfo> Positions { get; set; }

        [JsonPropertyName("candidate_coins")]
        public List<CandidateCoin> CandidateCoins { get; set; }

        [JsonIgnore]
        public Dictionary<string, MarketData> MarketDataMap { get; set; }

        [JsonIgnore]
        public Dictionary<string, OITopData> OITopDataMap { get; set; }

        [JsonIgnore]
        public object Performance { get; set; }

        [JsonIgnore]
        public int BTCETHLeverage { get; set; }

        [JsonIgnore]
        public int AltcoinLeverage { get; set; }
    }
}
