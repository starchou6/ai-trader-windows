using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AITrade.ViewModels.Entity
{
    public class MenuTextData : INotifyPropertyChanged
    {
        private string _homeView;
        public string HomeView
        {
            get => _homeView;
            set { _homeView = value; OnPropertyChanged(); }
        }

        private string _mainTitle;
        public string MainTitle
        {
            get => _mainTitle;
            set { _mainTitle = value; OnPropertyChanged(); }
        }

        private string _btnSetApiInfo;
        public string BtnSetApiInfo
        {
            get => _btnSetApiInfo;
            set { _btnSetApiInfo = value; OnPropertyChanged(); }
        }

        private string _btnSetAITrader;
        public string BtnSetAITrader
        {
            get => _btnSetAITrader;
            set { _btnSetAITrader = value; OnPropertyChanged(); }
        }

        private string _lblBinanceApiStatus;
        public string LblBinanceApiStatus
        {
            get => _lblBinanceApiStatus;
            set { _lblBinanceApiStatus = value; OnPropertyChanged(); }
        }

        private string _lblDeepSeekApiStatus;
        public string LblDeepSeekApiStatus
        {
            get => _lblDeepSeekApiStatus;
            set { _lblDeepSeekApiStatus = value; OnPropertyChanged(); }
        }

        private string _lblPeriod;
        public string LblPeriod
        {
            get => _lblPeriod;
            set { _lblPeriod = value; OnPropertyChanged(); }
        }

        private string _lblPeriodUnit;
        public string LblPeriodUnit
        {
            get => _lblPeriodUnit;
            set { _lblPeriodUnit = value; OnPropertyChanged(); }
        }

        private string _lblBalance;
        public string LblBalance
        {
            get => _lblBalance;
            set { _lblBalance = value; OnPropertyChanged(); }
        }

        private string _lblAvailableBalance;
        public string LblAvailableBalance
        {
            get => _lblAvailableBalance;
            set { _lblAvailableBalance = value; OnPropertyChanged(); }
        }

        private string _lblTotalUnrealizedProfit;
        public string LblTotalUnrealizedProfit
        {
            get => _lblTotalUnrealizedProfit;
            set { _lblTotalUnrealizedProfit = value; OnPropertyChanged(); }
        }

        private string _btnStart;
        public string BtnStart
        {
            get => _btnStart;
            set { _btnStart = value; OnPropertyChanged(); }
        }

        private string _btnStop;
        public string BtnStop
        {
            get => _btnStop;
            set { _btnStop = value; OnPropertyChanged(); }
        }

        private string _lblStatus;
        public string LblStatus
        {
            get => _lblStatus;
            set { _lblStatus = value; OnPropertyChanged(); }
        }

        private string _tabPositions;
        public string TabPositions
        {
            get => _tabPositions;
            set { _tabPositions = value; OnPropertyChanged(); }
        }

        private string _tabCompletedTrades;
        public string TabCompletedTrades
        {
            get => _tabCompletedTrades;
            set { _tabCompletedTrades = value; OnPropertyChanged(); }
        }

        private string _tabTradeLogs;
        public string TabTradeLogs
        {
            get => _tabTradeLogs;
            set { _tabTradeLogs = value; OnPropertyChanged(); }
        }

        private string _gridSide;
        public string GridSide
        {
            get => _gridSide;
            set { _gridSide = value; OnPropertyChanged(); }
        }

        private string _gridCoin;
        public string GridCoin
        {
            get => _gridCoin;
            set { _gridCoin = value; OnPropertyChanged(); }
        }

        private string _gridLeverage;
        public string GridLeverage
        {
            get => _gridLeverage;
            set { _gridLeverage = value; OnPropertyChanged(); }
        }

        private string _gridQuantity;
        public string GridQuantity
        {
            get => _gridQuantity;
            set { _gridQuantity = value; OnPropertyChanged(); }
        }

        private string _gridEntryPrice;
        public string GridEntryPrice
        {
            get => _gridEntryPrice;
            set { _gridEntryPrice = value; OnPropertyChanged(); }
        }

        private string _gridCurrentPrice;
        public string GridCurrentPrice
        {
            get => _gridCurrentPrice;
            set { _gridCurrentPrice = value; OnPropertyChanged(); }
        }

        private string _gridEntryAmount;
        public string GridEntryAmount
        {
            get => _gridEntryAmount;
            set { _gridEntryAmount = value; OnPropertyChanged(); }
        }

        private string _gridCurrentAmount;
        public string GridCurrentAmount
        {
            get => _gridCurrentAmount;
            set { _gridCurrentAmount = value; OnPropertyChanged(); }
        }

        private string _gridUnrealizedProfit;
        public string GridUnrealizedProfit
        {
            get => _gridUnrealizedProfit;
            set { _gridUnrealizedProfit = value; OnPropertyChanged(); }
        }

        private string _gridAction;
        public string GridAction
        {
            get => _gridAction;
            set { _gridAction = value; OnPropertyChanged(); }
        }

        private string _btnClose;
        public string BtnClose
        {
            get => _btnClose;
            set { _btnClose = value; OnPropertyChanged(); }
        }

        private string _gridCompletedPrice;
        public string GridCompletedPrice
        {
            get => _gridCompletedPrice;
            set { _gridCompletedPrice = value; OnPropertyChanged(); }
        }

        private string _gridRealizedProfit;
        public string GridRealizedProfit
        {
            get => _gridRealizedProfit;
            set { _gridRealizedProfit = value; OnPropertyChanged(); }
        }

        private string _gridTimestamp;
        public string GridTimestamp
        {
            get => _gridTimestamp;
            set { _gridTimestamp = value; OnPropertyChanged(); }
        }

        private string _lblCoTTrace;
        public string LblCoTTrace
        {
            get => _lblCoTTrace;
            set { _lblCoTTrace = value; OnPropertyChanged(); }
        }

        private string _lblDecisions;
        public string LblDecisions
        {
            get => _lblDecisions;
            set { _lblDecisions = value; OnPropertyChanged(); }
        }

        private string _lblInputPrompt;
        public string LblInputPrompt
        {
            get => _lblInputPrompt;
            set { _lblInputPrompt = value; OnPropertyChanged(); }
        }

        private string _btnShow;
        public string BtnShow
        {
            get => _btnShow;
            set { _btnShow = value; OnPropertyChanged(); }
        }

        private string _apiImportTitle;
        public string ApiImportTitle
        {
            get => _apiImportTitle;
            set { _apiImportTitle = value; OnPropertyChanged(); }
        }

        private string _apiImportBinanceKey;
        public string ApiImportBinanceKey
        {
            get => _apiImportBinanceKey;
            set { _apiImportBinanceKey = value; OnPropertyChanged(); }
        }

        private string _apiImportBinanceSecret;
        public string ApiImportBinanceSecret
        {
            get => _apiImportBinanceSecret;
            set { _apiImportBinanceSecret = value; OnPropertyChanged(); }
        }

        private string _apiImportDeepSeekKey;
        public string ApiImportDeepSeekKey
        {
            get => _apiImportDeepSeekKey;
            set { _apiImportDeepSeekKey = value; OnPropertyChanged(); }
        }

        private string _btnCancel;
        public string BtnCancel
        {
            get => _btnCancel;
            set { _btnCancel = value; OnPropertyChanged(); }
        }

        private string _btnImport;
        public string BtnImport
        {
            get => _btnImport;
            set { _btnImport = value; OnPropertyChanged(); }
        }

        private string _traderSettingTitle;
        public string TraderSettingTitle
        {
            get => _traderSettingTitle;
            set { _traderSettingTitle = value; OnPropertyChanged(); }
        }

        private string _lblSelectedCoins;
        public string LblSelectedCoins
        {
            get => _lblSelectedCoins;
            set { _lblSelectedCoins = value; OnPropertyChanged(); }
        }

        private string _lblAvailableCoins;
        public string LblAvailableCoins
        {
            get => _lblAvailableCoins;
            set { _lblAvailableCoins = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
