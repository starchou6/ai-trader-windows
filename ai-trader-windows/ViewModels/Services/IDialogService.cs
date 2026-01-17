using AITrade.Entity;
using AITrade.Entity.AI;
using AITrade.ViewModels.Entity;

namespace AITrade.ViewModels.Services
{
    public interface IDialogService
    {
        void ShowError(string message, string title = "Error");
        void ShowInfo(string message, string title = "Info");
        bool ShowConfirm(string message, string title = "Confirm");

        ApiImportResult ShowApiImportDialog(string currentKey, string currentSecret, string currentAiKey);
        TraderSettingResult ShowTraderSettingDialog(
            MenuTextData menuItem,
            List<string> availableCoins,
            List<string> selectedCoins,
            string customPrompt,
            int scanInterval,
            StrategyLibrary strategyLibrary);
        void ShowTradeLogDetailDialog(MenuTextData menuItem, DecisionRecord record);
    }

    public class ApiImportResult
    {
        public bool Success { get; set; }
        public string ApiKey { get; set; } = "";
        public string ApiSecret { get; set; } = "";
        public string AiKey { get; set; } = "";
    }

    public class TraderSettingResult
    {
        public bool Success { get; set; }
        public int ScanInterval { get; set; }
        public List<string> SelectedCoins { get; set; } = new();
        public string CustomPrompt { get; set; } = "";
        public StrategyLibrary StrategyLibrary { get; set; }
    }
}
