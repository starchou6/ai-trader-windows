using System.Windows;
using AITrade.ViewModels.Dialogs;

namespace AITrade
{
    public partial class ApiImportDialog : Window
    {
        public ApiImportDialog()
        {
            InitializeComponent();
        }

        public ApiImportDialogViewModel ViewModel => DataContext as ApiImportDialogViewModel;
    }
}
