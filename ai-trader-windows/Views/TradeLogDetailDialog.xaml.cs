using AITrade.Entity.AI;
using AITrade.ViewModels.Dialogs;
using AITrade.ViewModels.Entity;
using System.Windows;

namespace AITrader.Views
{
    public partial class TradeLogDetailDialog : Window
    {
        public TradeLogDetailDialog()
        {
            InitializeComponent();
        }

        public TradeLogDetailDialog(MenuTextData menuItem, DecisionRecord record)
        {
            InitializeComponent();
            if (DataContext is TradeLogDetailDialogViewModel vm)
            {
                vm.MenuItem = menuItem;
                vm.Record = record;
            }
        }

        public TradeLogDetailDialogViewModel ViewModel => DataContext as TradeLogDetailDialogViewModel;
    }
}
