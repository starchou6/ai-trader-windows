using System;
using System.Collections.Generic;

namespace AITrade.Entity
{
    public class StrategyConfig
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public int ScanInterval { get; set; } = 600;
        public List<string> SelectedCoins { get; set; } = new List<string>();
        public string CustomPrompt { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
