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
