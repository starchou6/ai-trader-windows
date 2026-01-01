using System.Text.Json.Serialization;

namespace AITrade.Entity.AI
{
    /// <summary>
    /// 账户信息
    /// </summary>
    public class AccountInfo
    {
        /// <summary>账户净值</summary>
        [JsonPropertyName("total_equity")]
        public double TotalEquity { get; set; }

        /// <summary>可用余额</summary>
        [JsonPropertyName("available_balance")]
        public double AvailableBalance { get; set; }

        /// <summary>总盈亏</summary>
        [JsonPropertyName("total_pnl")]
        public double TotalPnL { get; set; }

        /// <summary>总盈亏百分比</summary>
        [JsonPropertyName("total_pnl_pct")]
        public double TotalPnLPct { get; set; }

        /// <summary>已用保证金</summary>
        [JsonPropertyName("margin_used")]
        public double MarginUsed { get; set; }

        /// <summary>保证金使用率</summary>
        [JsonPropertyName("margin_used_pct")]
        public double MarginUsedPct { get; set; }

        /// <summary>持仓数量</summary>
        [JsonPropertyName("position_count")]
        public int PositionCount { get; set; }
    }
}
