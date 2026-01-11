using System.Text.RegularExpressions;

namespace AITrade.Entity.AI
{
    public class DecisionRecord
    {
        public string Title { get; set; } = "";
        public List<string> ExecutionLog { get; set; } = new();
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = "";
        public string InputPrompt { get; set; } = "";
        public string CoTTrace { get; set; } = "";
        public string DecisionJSON { get; set; } = "";

        public AccountSnapshot AccountState { get; set; } = new();
        public List<PositionSnapshot> Positions { get; set; } = new();
        public List<string> CandidateCoins { get; set; } = new();
        public List<DecisionAction> Decisions { get; set; } = new();

        // Computed properties for DataGrid display
        public string RecordTime
        {
            get
            {
                var match = Regex.Match(Title, @"\*\*时间\*\*:\s*([^\|]+)");
                return match.Success ? match.Groups[1].Value.Trim() : "";
            }
        }

        public string CycleNumber
        {
            get
            {
                var match = Regex.Match(Title, @"\*\*周期\*\*:\s*#(\d+)");
                return match.Success ? match.Groups[1].Value : "";
            }
        }

        public string RuntimeMinutes
        {
            get
            {
                var match = Regex.Match(Title, @"\*\*运行\*\*:\s*(\d+)分钟");
                return match.Success ? match.Groups[1].Value : "";
            }
        }

        public int OpenLongCount => Decisions?.Count(d => d.Action == "open_long") ?? 0;
        public int OpenShortCount => Decisions?.Count(d => d.Action == "open_short") ?? 0;
        public int CloseLongCount => Decisions?.Count(d => d.Action == "close_long") ?? 0;
        public int CloseShortCount => Decisions?.Count(d => d.Action == "close_short") ?? 0;
        public int WaitCount => Decisions?.Count(d => d.Action == "wait" || d.Action == "hold") ?? 0;
    }

    public class AccountSnapshot
    {
        public double TotalBalance { get; set; }
        public double AvailableBalance { get; set; }
        public double TotalUnrealizedProfit { get; set; }
        public int PositionCount { get; set; }
        public double MarginUsedPct { get; set; }
    }

    public class PositionSnapshot
    {
        public string Symbol { get; set; } = "";
        public string Side { get; set; } = "";
        public double PositionAmt { get; set; }
        public double EntryPrice { get; set; }
        public double MarkPrice { get; set; }
        public double UnrealizedProfit { get; set; }
        public double Leverage { get; set; }
        public double LiquidationPrice { get; set; }
    }

    public class DecisionAction
    {
        public string Action { get; set; } = "";
        public string Symbol { get; set; } = "";
        public double Quantity { get; set; }
        public int Leverage { get; set; }
        public double Price { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Success { get; set; }
        public string Error { get; set; } = "";
        public long? OrderID { get; set; }
    }

}
