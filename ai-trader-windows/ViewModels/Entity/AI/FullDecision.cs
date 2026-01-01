using System.Text.Json.Serialization;

namespace AITrade.Entity.AI
{
    public class Decision
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; } = "";

        [JsonPropertyName("action")]
        public string Action { get; set; } = ""; // open_long/open_short/close_long/close_short/hold/wait

        [JsonPropertyName("leverage")]
        public int Leverage { get; set; }

        [JsonPropertyName("position_size_usd")]
        public double PositionSizeUSD { get; set; }

        [JsonPropertyName("stop_loss")]
        public double StopLoss { get; set; }

        [JsonPropertyName("take_profit")]
        public double TakeProfit { get; set; }

        [JsonPropertyName("confidence")]
        public int Confidence { get; set; }

        [JsonPropertyName("risk_usd")]
        public double RiskUSD { get; set; }

        [JsonPropertyName("reasoning")]
        public string Reasoning { get; set; } = "";
    }


    // -------- FullDecision --------
    public class FullDecision
    {
        public string UserPrompt { get; set; } = "";
        public string CoTTrace { get; set; } = "";
        public List<Decision> Decisions { get; set; } = new();
        public DateTime Timestamp { get; set; }
    }
}
