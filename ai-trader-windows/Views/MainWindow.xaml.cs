using System.Windows;
using System.Windows.Controls;
using AITrade.Entity.AI;
using AITrader.Views;

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

        private void ViewTradeLogDetail_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is DecisionRecord record)
            {
                var vm = DataContext as MainViewModel;
                if (vm?.MenuItem != null)
                {
                    var dlg = new TradeLogDetailDialog(vm.MenuItem, record);
                    dlg.Owner = this;
                    dlg.ShowDialog();
                }
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

                // TradeLogs DataGrid
                colLogTime.Header = menu.GridTime;
                colLogCycle.Header = menu.GridCycle;
                colLogRuntime.Header = menu.GridRuntime;
                colLogOpenLong.Header = menu.GridOpenLong;
                colLogOpenShort.Header = menu.GridOpenShort;
                colLogWait.Header = menu.GridWait;
                colLogAction.Header = menu.GridAction;
            }
        }
    }
}
