using System.Windows;
using System.Windows.Input;

namespace AITrade.ViewModels.Dialogs
{
    public class ApiImportDialogViewModel : ViewModelBase
    {
        private string _apiKey = "";
        public string ApiKey
        {
            get => _apiKey;
            set => SetProperty(ref _apiKey, value);
        }

        private string _apiSecret = "";
        public string ApiSecret
        {
            get => _apiSecret;
            set => SetProperty(ref _apiSecret, value);
        }

        private string _aiKey = "";
        public string AiKey
        {
            get => _aiKey;
            set => SetProperty(ref _aiKey, value);
        }

        public ICommand ImportCommand { get; }
        public ICommand CancelCommand { get; }

        public ApiImportDialogViewModel()
        {
            ImportCommand = new RelayCommand(Import);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void Import(object parameter)
        {
            if (string.IsNullOrWhiteSpace(ApiKey?.Trim()) ||
                string.IsNullOrWhiteSpace(ApiSecret?.Trim()) ||
                string.IsNullOrWhiteSpace(AiKey?.Trim()))
            {
                MessageBox.Show("Api key or secret cannot be empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (parameter is Window window)
            {
                window.DialogResult = true;
                window.Close();
            }
        }

        private void Cancel(object parameter)
        {
            if (parameter is Window window)
            {
                window.DialogResult = false;
                window.Close();
            }
        }
    }
}
