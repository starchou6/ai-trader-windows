using AITrade.Entity.AI;
using AITrade.ViewModels.Entity;

namespace AITrade.ViewModels.Dialogs
{
    public class TradeLogDetailDialogViewModel : ViewModelBase
    {
        private MenuTextData _menuItem;
        public MenuTextData MenuItem
        {
            get => _menuItem;
            set => SetProperty(ref _menuItem, value);
        }

        private DecisionRecord _record;
        public DecisionRecord Record
        {
            get => _record;
            set => SetProperty(ref _record, value);
        }

        public TradeLogDetailDialogViewModel()
        {
        }

        public TradeLogDetailDialogViewModel(MenuTextData menuItem, DecisionRecord record)
        {
            MenuItem = menuItem;
            Record = record;
        }
    }
}
