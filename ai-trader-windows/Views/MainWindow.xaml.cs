using System.Windows;

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

        // DataGridTextColumn.Header does not support binding directly in WPF
        // This is a known limitation, so we need to update headers manually
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
