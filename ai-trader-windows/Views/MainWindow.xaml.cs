using System.Windows;
using System.Windows.Controls;

namespace AITrade
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (DataContext is MainViewModel vm)
            {
                vm.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(vm.MenuItem))
                    {
                        RefreshDataGridHeaders();
                    }
                };
            }
            RefreshDataGridHeaders();
        }

        private void ShowInputPrompt_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string prompt)
            {
                var dlg = new InputPromptDialog(prompt);
                dlg.Owner = this;
                dlg.ShowDialog();
            }
        }
        private void RefreshDataGridHeaders()
        {
            var menu = (DataContext as MainViewModel)?.MenuItem;
            if (menu != null)
            {
                // Positions DataGrid
                colSide.Header = menu.GridSide;
                colCoin.Header = menu.GridCoin;
                colLeverage.Header = menu.GridLeverage;
                colQuantity.Header = menu.GridQuantity;
                colEntryPrice.Header = menu.GridEntryPrice;
                colCurrentPrice.Header = menu.GridCurrentPrice;
                colEntryAmount.Header = menu.GridEntryAmount;
                colCurrentAmount.Header = menu.GridCurrentAmount;
                colUnrealizedProfit.Header = menu.GridUnrealizedProfit;
                colAction.Header = menu.GridAction;

                // CompletedTrades DataGrid
                colCompletedSide.Header = menu.GridSide;
                colCompletedCoin.Header = menu.GridCoin;
                colCompletedQuantity.Header = menu.GridQuantity;
                colCompletedPrice.Header = menu.GridCompletedPrice;
                colCompletedRealizedProfit.Header = menu.GridRealizedProfit;
                colCompletedTimestamp.Header = menu.GridTimestamp;
            }
        }
    }
}
