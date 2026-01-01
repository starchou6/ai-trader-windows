using System.Text.Json.Serialization;

namespace AITrade.Entity.AI
{
    /// <summary>
    /// 候选币种（来自币种池）
    /// </summary>
    public class CandidateCoin
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        /// <summary>来源: "ai500" 和/或 "oi_top"</summary>
        [JsonPropertyName("sources")]
        public List<string> Sources { get; set; }
    }
}
