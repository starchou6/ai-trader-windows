using AITrade.Entity.AI;
using AITrade.ViewModels.Entity;
using System.Windows;

namespace AITrader.Views
{
    public partial class TradeLogDetailDialog : Window
    {
        public MenuTextData MenuItem { get; set; }
        public DecisionRecord Record { get; set; }

        public TradeLogDetailDialog(MenuTextData menuItem, DecisionRecord record)
        {
            InitializeComponent();
            MenuItem = menuItem;
            Record = record;
            DataContext = this;
        }
    }
}
