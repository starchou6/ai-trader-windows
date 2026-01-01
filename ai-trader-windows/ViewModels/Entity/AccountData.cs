using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AITrade.Entity
{
    public class AccountData : INotifyPropertyChanged
    {
        private string _walletAddress;
        public string WalletAddress
        {
            get => _walletAddress;
            set
            {
                _walletAddress = value;
                OnPropertyChanged();
            }
        }

        private bool _apiStatus;
        public bool ApiStatus
        {
            get => _apiStatus;
            set
            {
                _apiStatus = value;
                OnPropertyChanged();
            }
        }

        private decimal _balance;
        public decimal Balance
        {
            get => _balance;
            set
            {
                _balance = value;
                OnPropertyChanged();
            }
        }

        private decimal _availableBalance;
        public decimal AvailableBalance
        {
            get => _availableBalance;
            set
            {
                _availableBalance = value;
                OnPropertyChanged();
            }
        }

        private decimal _totalUnrealizedProfit;
        public decimal TotalUnrealizedProfit
        {
            get => _totalUnrealizedProfit;
            set
            {
                _totalUnrealizedProfit = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
