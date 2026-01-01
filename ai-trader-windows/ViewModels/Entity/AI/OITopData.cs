using System.Text.Json.Serialization;

namespace AITrade.Entity.AI
{
    /// <summary>
    /// 持仓量增长Top数据（用于AI决策参考）
    /// </summary>
    public class OITopData
    {
        [JsonIgnore]
        public int Rank { get; set; }

        [JsonIgnore]
        public double OIDeltaPercent { get; set; }

        [JsonIgnore]
        public double OIDeltaValue { get; set; }

        [JsonIgnore]
        public double PriceDeltaPercent { get; set; }

        [JsonIgnore]
        public double NetLong { get; set; }

        [JsonIgnore]
        public double NetShort { get; set; }
    }
}
