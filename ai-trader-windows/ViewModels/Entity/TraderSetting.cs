using System.Collections.Generic;

namespace AITrade.Entity
{
    public class TraderSetting
    {
        public int ScanInterval { get; set; }
        public List<string> SelectedCoins { get; set; } = new List<string>();
    }
}
