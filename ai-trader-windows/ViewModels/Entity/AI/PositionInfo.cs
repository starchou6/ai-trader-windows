using System.Text.Json.Serialization;

namespace AITrade.Entity.AI
{
    /// <summary>
    /// 持仓信息
    /// </summary>
    public class PositionInfo
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        /// <summary>PositionConstants.LONG或 PositionConstants.SHORT</summary>
        [JsonPropertyName("side")]
        public string Side { get; set; }

        [JsonPropertyName("entry_price")]
        public double EntryPrice { get; set; }

        [JsonPropertyName("mark_price")]
        public double MarkPrice { get; set; }

        [JsonPropertyName("quantity")]
        public double Quantity { get; set; }

        [JsonPropertyName("leverage")]
        public int Leverage { get; set; }

        [JsonPropertyName("unrealized_profit")]
        public double UnrealizedProfit { get; set; }

        [JsonPropertyName("unrealized_pnl_pct")]
        public double UnrealizedPnLPct { get; set; }

        [JsonPropertyName("liquidation_price")]
        public double LiquidationPrice { get; set; }

        [JsonPropertyName("margin_used")]
        public double MarginUsed { get; set; }

        /// <summary>持仓更新时间戳（毫秒）</summary>
        [JsonPropertyName("update_time")]
        public long UpdateTime { get; set; }
    }
}
