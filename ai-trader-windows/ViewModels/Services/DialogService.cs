using System.Windows;
using AITrade.Entity;
using AITrade.Entity.AI;
using AITrade.ViewModels.Entity;
using AITrader.Views;

namespace AITrade.ViewModels.Services
{
    public class DialogService : IDialogService
    {
        public void ShowError(string message, string title = "Error")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowInfo(string message, string title = "Info")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public bool ShowConfirm(string message, string title = "Confirm")
        {
            return MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        public ApiImportResult ShowApiImportDialog(string currentKey, string currentSecret, string currentAiKey)
        {
            var dialog = new ApiImportDialog();
            dialog.Owner = Application.Current.MainWindow;

            // Set initial values through ViewModel
            if (dialog.ViewModel != null)
            {
                dialog.ViewModel.ApiKey = currentKey ?? "";
                dialog.ViewModel.ApiSecret = currentSecret ?? "";
                dialog.ViewModel.AiKey = currentAiKey ?? "";
            }

            if (dialog.ShowDialog() == true)
            {
                return new ApiImportResult
                {
                    Success = true,
                    ApiKey = dialog.ViewModel?.ApiKey?.Trim() ?? "",
                    ApiSecret = dialog.ViewModel?.ApiSecret?.Trim() ?? "",
                    AiKey = dialog.ViewModel?.AiKey?.Trim() ?? ""
                };
            }
            return new ApiImportResult { Success = false };
        }

        public TraderSettingResult ShowTraderSettingDialog(
            MenuTextData menuItem,
            List<string> availableCoins,
            List<string> selectedCoins,
            string customPrompt,
            int scanInterval,
            StrategyLibrary strategyLibrary)
        {
            var dialog = new TraderSettingDialog(menuItem, availableCoins, selectedCoins, customPrompt, strategyLibrary);
            dialog.Owner = Application.Current.MainWindow;

            // Set scan interval through ViewModel
            if (dialog.ViewModel != null)
            {
                dialog.ViewModel.ScanInterval = scanInterval.ToString();
            }

            if (dialog.ShowDialog() == true)
            {
                var vm = dialog.ViewModel;
                return new TraderSettingResult
                {
                    Success = true,
                    ScanInterval = int.TryParse(vm?.ScanInterval?.Trim(), out var interval) ? interval : scanInterval,
                    SelectedCoins = dialog.GetSelectedCoins(),
                    CustomPrompt = dialog.GetCustomPrompt(),
                    StrategyLibrary = dialog.GetStrategyLibrary()
                };
            }
            return new TraderSettingResult { Success = false };
        }

        public void ShowTradeLogDetailDialog(MenuTextData menuItem, DecisionRecord record)
        {
            var dialog = new TradeLogDetailDialog(menuItem, record);
            dialog.Owner = Application.Current.MainWindow;
            dialog.ShowDialog();
        }
    }
}
