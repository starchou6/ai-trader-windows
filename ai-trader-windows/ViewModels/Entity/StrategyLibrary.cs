using System.Collections.Generic;

namespace AITrade.Entity
{
    public class StrategyLibrary
    {
        public List<StrategyConfig> Strategies { get; set; } = new List<StrategyConfig>();
        public string ActiveStrategyId { get; set; } = string.Empty;
    }
}
