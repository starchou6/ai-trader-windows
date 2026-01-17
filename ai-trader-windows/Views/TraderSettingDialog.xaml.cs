using System.Windows;
using AITrade.Entity;
using AITrade.ViewModels.Dialogs;
using AITrade.ViewModels.Entity;

namespace AITrader.Views
{
    public partial class TraderSettingDialog : Window
    {
        public TraderSettingDialog()
        {
            InitializeComponent();
        }

        public TraderSettingDialog(MenuTextData menuItem, List<string> availableCoins, List<string> selectedCoins, string customPrompt, StrategyLibrary strategyLibrary)
        {
            InitializeComponent();

            if (DataContext is TraderSettingDialogViewModel vm)
            {
                vm.Initialize(menuItem, availableCoins, selectedCoins, customPrompt, strategyLibrary);
            }
        }

        public TraderSettingDialogViewModel ViewModel => DataContext as TraderSettingDialogViewModel;

        public List<string> GetSelectedCoins()
        {
            return ViewModel?.GetSelectedCoins() ?? new List<string>();
        }

        public string GetCustomPrompt()
        {
            return ViewModel?.GetCustomPrompt() ?? "";
        }

        public StrategyLibrary GetStrategyLibrary()
        {
            return ViewModel?.GetStrategyLibrary() ?? new StrategyLibrary();
        }

        public StrategyConfig GetSelectedStrategy()
        {
            return ViewModel?.GetSelectedStrategy();
        }
    }
}
