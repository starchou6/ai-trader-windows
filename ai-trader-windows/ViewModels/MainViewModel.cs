using AITrade.API;
using AITrade.Consts;
using AITrade.Converter;
using AITrade.Entity;
using AITrade.Entity.AI;
using AITrade.Services;
using AITrade.ViewModels.Consts;
using AITrade.ViewModels.Entity;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;

namespace AITrade
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Const
        private const string API_INFO_FILE_NAME = "apiInfo.txt";
        private const string AI_KEY_FILE_NAME = "aiKey.txt";
        private const string LANGUAGE_SETTING_FILE_NAME = "languageSetting.txt";
        #endregion

        #region 私有属性
        private string _apiKey;
        private string _apiSecret;
        private string _aiKey;
        private AutoTrader _autoTrader;
        private string _logDir;
        #endregion

        #region 画面绑定属性
        public bool CanPrevPage => CurrentPage > 0;
        public bool CanNextPage => CurrentPage < TotalPageCount - 1;

        private int _currentPage = 0;
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanPrevPage));
                    OnPropertyChanged(nameof(CanNextPage));
                    LoadTradeLogsPage();
                }
            }
        }

        public int PageSize { get; set; } = 10;

        private int _totalPageCount = 0;
        public int TotalPageCount
        {
            get => _totalPageCount;
            set
            {
                _totalPageCount = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanPrevPage));
                OnPropertyChanged(nameof(CanNextPage));
            }
        }

        private ObservableCollection<string> _availableLanguages = [.. CommonConstants.LANGUAGE_LIST];
        public ObservableCollection<string> AvailableLanguages
        {
            get => _availableLanguages;
            set { _availableLanguages = value; OnPropertyChanged(); }
        }

        private string _selectedLanguage = CommonConstants.LANGUAGE_EN;
        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                _selectedLanguage = value;
                OnPropertyChanged();
                MenuItem = MenuConstants.GetMenuItems(_selectedLanguage);
                File.WriteAllText(LANGUAGE_SETTING_FILE_NAME, _selectedLanguage);
            }
        }

        private MenuTextData _menuItem = MenuConstants.MENU_ITEMS_EN;
        public MenuTextData MenuItem
        {
            get => _menuItem;
            set { _menuItem = value; OnPropertyChanged(); }
        }

        private bool _isAiKeyEffective;
        public bool IsAiKeyEffective
        {
            get => _isAiKeyEffective;
            set
            {
                _isAiKeyEffective = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<DecisionRecord> _aiTradeLogs;
        public ObservableCollection<DecisionRecord> AiTradeLogs
        {
            get => _aiTradeLogs;
            set { _aiTradeLogs = value; OnPropertyChanged(); }
        }

        private AccountData _accountData;
        public AccountData AccountData
        {
            get => _accountData;
            set { _accountData = value; OnPropertyChanged(); }
        }

        private ObservableCollection<PositionData> _positions = [];
        public ObservableCollection<PositionData> Positions
        {
            get => _positions;
            set { _positions = value; OnPropertyChanged(); }
        }

        private ObservableCollection<CompletedTrade> _completedTrades = [];
        public ObservableCollection<CompletedTrade> CompletedTrades
        {
            get => _completedTrades;
            set { _completedTrades = value; OnPropertyChanged(); }
        }

        private bool _isRunning;
        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                _isRunning = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Status));
                OnPropertyChanged(nameof(IsNotRunning));
            }
        }

        public string Status => _isRunning ? "running" : "not running";
        public bool IsNotRunning => !IsRunning;
        #endregion

        #region Command
        public ICommand NextPageCommand { get; }
        public ICommand PrevPageCommand { get; }
        public ICommand ImportWalletCommand { get; }
        public ICommand StartCommand { get; }
        public ICommand EndCommand { get; }
        public ICommand ClosePositionCommand { get; }
        public ICommand CloseAllPositionCommand { get; }
        public ICommand TestAiTraderCommand { get; }
        #endregion

        public MainViewModel()
        {
            ImportWalletCommand = new RelayCommand(ImportWallet);
            StartCommand = new RelayCommand(Start);
            EndCommand = new RelayCommand(End);
            ClosePositionCommand = new RelayCommand(
                execute: p => ClosePosition((PositionData)p!),
                canExecute: p => p is PositionData
            );
            CloseAllPositionCommand = new RelayCommand(CloseAllPosition);
            TestAiTraderCommand = new RelayCommand(TestAiTrader);
            NextPageCommand = new RelayCommand(_ => { if (CurrentPage < TotalPageCount - 1) CurrentPage++; });
            PrevPageCommand = new RelayCommand(_ => { if (CurrentPage > 0) CurrentPage--; });
            #region language setting
            if (File.Exists(LANGUAGE_SETTING_FILE_NAME))
            {
                var languageCode = File.ReadAllText(LANGUAGE_SETTING_FILE_NAME).Trim();
                if (!string.IsNullOrEmpty(languageCode))
                {
                    SelectedLanguage = languageCode;
                }
            }
            #endregion

            AccountData = new AccountData();

            if (File.Exists(API_INFO_FILE_NAME))
            {
                var apiInfo = File.ReadAllText(API_INFO_FILE_NAME).Trim();
                if (!string.IsNullOrEmpty(apiInfo))
                {
                    var apiInfos = apiInfo.Split(" ");
                    if (apiInfos.Length == 2)
                    {
                        _apiKey = apiInfos[0];
                        _apiSecret = apiInfos[1];
                    }
                }
            }
            if (File.Exists(AI_KEY_FILE_NAME))
            {
                var aiKey = File.ReadAllText(AI_KEY_FILE_NAME).Trim();
                if (!string.IsNullOrEmpty(aiKey))
                {
                    _aiKey = aiKey;
                }
            }
            AccountData.ApiStatus = false;
            _ = LoadInfoStateAsync();
            IsAiKeyEffective = false;
            _ = ValidAiKeyAsync();
        }

        #region 函数
        private async Task LoadInfoStateAsync()
        {
            if (string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(_apiSecret))
            {
                return;
            }

            try
            {
                var binanceClient = new BinanceClient(_apiKey, _apiSecret);
                var accountInfo = await binanceClient.GetAccountInfo();
                var positions = await binanceClient.GetPositions();
                var ciompletedTrades = await binanceClient.GetCompletedTrades();

                await App.Current.Dispatcher.InvokeAsync(() =>
                {
                    AccountData = BinanceDataConverter.ConvertToAccountData(accountInfo);
                    Positions = [.. BinanceDataConverter.ConvertToPositionDatas(positions)];
                    CompletedTrades = [.. BinanceDataConverter.CompletedTrades(ciompletedTrades)];
                });
            }
            catch (Exception ex)
            {
                await App.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"加载账户状态失败: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    AccountData.ApiStatus = false;
                });
            }
        }

        private async Task ValidAiKeyAsync()
        {
            if (string.IsNullOrEmpty(_aiKey))
            {
                return;
            }

            try
            {

                var deepSeekClient = new DeepSeekAIClient(_aiKey);
                var isEffctive = await deepSeekClient.ValidApiKey();

                await App.Current.Dispatcher.InvokeAsync(() =>
                {
                    IsAiKeyEffective = isEffctive;
                });
            }
            catch (Exception ex)
            {
                await App.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"验证AI Key失败: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    IsAiKeyEffective = false;
                });
            }
        }

        private void Start(object parameter)
        {
            if (!(AccountData.ApiStatus && IsAiKeyEffective))
            {
                MessageBox.Show("Please input your api info first. 请先输入api信息", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (IsRunning) return;
            IsRunning = true;
            var traderId = "demo";
            _logDir = Path.Combine("decision_logs", traderId);
            _ = Task.Run(RunTradeLoopAsync);
            _ = Task.Run(GetLogAsync);
            var cfg = new AutoTraderConfig
            {
                ID = traderId,
                Name = "Demo Trader",
                AIModel = "deepseek",
                DeepSeekKey = _aiKey,
                Exchange = "binance",
                InitialBalance = 500,
                BinanceAPIKey = _apiKey,
                BinanceSecretKey = _apiSecret,
                ScanInterval = TimeSpan.FromSeconds(180),
                BTCETHLeverage = 20,
                AltcoinLeverage = 10,
                LogDirectory = _logDir,
            };
            _autoTrader = AutoTrader.Create(cfg);
            _ = Task.Run(() => _autoTrader.Run());
        }

        private void End(object parameter)
        {
            if (!IsRunning) return;
            IsRunning = false;
            _autoTrader.Stop();
        }

        private void ImportWallet(object parameter)
        {
            var dialog = new ApiImportDialog();
            dialog.Key.Text = _apiKey;
            dialog.Secret.Text = _apiSecret;
            dialog.AiKey.Text = _aiKey;
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    _apiKey = dialog.Key.Text.Trim();
                    _apiSecret = dialog.Secret.Text.Trim();
                    _aiKey = dialog.AiKey.Text.Trim();
                    SaveApiInfo(_apiKey + " " + _apiSecret);
                    SaveAiKeyInfo(_aiKey);
                    _ = LoadInfoStateAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to import wallet: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveApiInfo(string privateKey)
        {
            System.IO.File.WriteAllText(API_INFO_FILE_NAME, privateKey);
        }

        private void SaveAiKeyInfo(string aiKey)
        {
            System.IO.File.WriteAllText(AI_KEY_FILE_NAME, aiKey);
        }

        private async Task RunTradeLoopAsync()
        {
            try
            {
                while (IsRunning)
                {
                    await LoadInfoStateAsync();
                    // 每隔10秒钟检查一次
                    await Task.Delay(10000).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                // 任何后台异常都立即通知用户并停止循环
                IsRunning = false;
                var detail = ex is HttpRequestException httpEx && httpEx.Data != null
                    ? $"{ex.Message}"
                    : ex.Message;

                await App.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"RunTradeLoop failed:\n{detail}", "Trade Loop Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        private async Task GetLogAsync()
        {
            try
            {
                while (IsRunning)
                {
                    await App.Current.Dispatcher.InvokeAsync(() =>
                    {
                        LoadTradeLogsPage();
                    });
                    // 每隔60秒钟检查一次日志
                    await Task.Delay(60000).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                await App.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"Get log failed:\n", "GetLogAsync() Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        private DecisionRecord[] ReadRecordsByPage(int pageIndex, int pageSize)
        {
            var files = Directory.GetFiles(_logDir, "*.json")
                .OrderByDescending(f => f)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToArray();

            var list = new List<DecisionRecord>();
            foreach (var file in files)
            {
                try
                {
                    var json = File.ReadAllText(file);
                    var record = JsonSerializer.Deserialize<DecisionRecord>(json, CommonConstants.CachedJsonSerializerOptions);
                    if (record != null)
                        list.Add(record);
                }
                catch
                {
                    // 可根据需要记录或忽略异常
                }
            }
            return [.. list];
        }

        private int GetTotalRecordCount()
        {
            if (string.IsNullOrEmpty(_logDir) || !Directory.Exists(_logDir))
                return 0;
            return Directory.GetFiles(_logDir, "*.json").Length;
        }

        private int GetTotalPageCount(int pageSize)
        {
            int totalCount = GetTotalRecordCount();
            return (totalCount + pageSize - 1) / pageSize;
        }

        private void LoadTradeLogsPage()
        {
            TotalPageCount = GetTotalPageCount(PageSize);
            AiTradeLogs = [.. ReadRecordsByPage(CurrentPage, PageSize)];
        }

        private async Task ClosePosition(PositionData pos)
        {
            // 平仓逻辑
            var binanceClient = new BinanceClient(_apiKey, _apiSecret);
            try
            {
                var order = await binanceClient.PlaceOrder(
                    symbol: pos.Symbol,
                    quantity: pos.Quantity,
                    price: null,
                    side: pos.Side == PositionConstants.LONG ? Binance.Net.Enums.OrderSide.Sell : Binance.Net.Enums.OrderSide.Buy,
                    orderType: Binance.Net.Enums.FuturesOrderType.Market
                );
                if (order != null)
                {
                    await LoadInfoStateAsync();
                }
            }
            catch (Exception ex)
            {
                await App.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"平仓失败: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        private async void CloseAllPosition(object parameter)
        {
            IsRunning = false;
            foreach (var pos in Positions)
            {
                await ClosePosition(pos);
            }
        }

        private async void TestAiTrader(object parameter)
        {
            var cfg = new AutoTraderConfig
            {
                ID = "demo",
                Name = "Demo Trader",
                AIModel = "deepseek",
                DeepSeekKey = "sk-e451e1580a3e4318914dc57064b49630",
                Exchange = "binance",
                InitialBalance = 500,
                BinanceAPIKey = "350KPNRi01dLS6933iIXB6sOYxUWvodjy6zH6X8WV84PmyyXEpEnnQqRvANMGyaT",
                BinanceSecretKey = "uFdSsI6UfvBTOWtCoW4IGyFwanGtE1FQXQT4faKZtrc9CcQh2ijt2w6hB8loUZjD",
                ScanInterval = TimeSpan.FromSeconds(60),
                BTCETHLeverage = 10,
                AltcoinLeverage = 10
            };
            var aiTrader = AutoTrader.Create(cfg);
            await aiTrader.Run();
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
