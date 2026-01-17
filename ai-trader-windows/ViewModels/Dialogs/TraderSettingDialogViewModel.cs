using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using AITrade.Entity;
using AITrade.Utils;
using AITrade.ViewModels.Entity;

namespace AITrade.ViewModels.Dialogs
{
    public class TraderSettingDialogViewModel : ViewModelBase
    {
        #region Private Fields
        private List<string> _allAvailableCoins = new();
        private StrategyConfig _selectedStrategy;
        private bool _isLoadingStrategy = false;
        #endregion

        #region Properties
        private MenuTextData _menuItem;
        public MenuTextData MenuItem
        {
            get => _menuItem;
            set => SetProperty(ref _menuItem, value);
        }

        private StrategyLibrary _strategyLibrary = new();
        public StrategyLibrary StrategyLibrary
        {
            get => _strategyLibrary;
            set => SetProperty(ref _strategyLibrary, value);
        }

        private string _scanInterval = "600";
        public string ScanInterval
        {
            get => _scanInterval;
            set => SetProperty(ref _scanInterval, value);
        }

        private string _strategyName = "";
        public string StrategyName
        {
            get => _strategyName;
            set => SetProperty(ref _strategyName, value);
        }

        private string _customPrompt = "";
        public string CustomPrompt
        {
            get => _customPrompt;
            set => SetProperty(ref _customPrompt, value);
        }

        private ObservableCollection<string> _strategies = new();
        public ObservableCollection<string> Strategies
        {
            get => _strategies;
            set => SetProperty(ref _strategies, value);
        }

        private string _selectedStrategyItem;
        public string SelectedStrategyItem
        {
            get => _selectedStrategyItem;
            set
            {
                if (SetProperty(ref _selectedStrategyItem, value))
                {
                    OnStrategySelectionChanged();
                }
            }
        }

        private ObservableCollection<string> _availableCoins = new();
        public ObservableCollection<string> AvailableCoins
        {
            get => _availableCoins;
            set => SetProperty(ref _availableCoins, value);
        }

        private ObservableCollection<string> _selectedCoins = new();
        public ObservableCollection<string> SelectedCoins
        {
            get => _selectedCoins;
            set => SetProperty(ref _selectedCoins, value);
        }

        private object _selectedAvailableCoin;
        public object SelectedAvailableCoin
        {
            get => _selectedAvailableCoin;
            set
            {
                if (SetProperty(ref _selectedAvailableCoin, value))
                {
                    OnPropertyChanged(nameof(CanAddCoin));
                }
            }
        }

        private object _selectedSelectedCoin;
        public object SelectedSelectedCoin
        {
            get => _selectedSelectedCoin;
            set
            {
                if (SetProperty(ref _selectedSelectedCoin, value))
                {
                    OnPropertyChanged(nameof(CanRemoveCoin));
                }
            }
        }

        public bool CanAddCoin => SelectedAvailableCoin != null;
        public bool CanRemoveCoin => SelectedSelectedCoin != null;

        private bool _canDeleteStrategy = false;
        public bool CanDeleteStrategy
        {
            get => _canDeleteStrategy;
            set => SetProperty(ref _canDeleteStrategy, value);
        }
        #endregion

        #region Commands
        public ICommand AddCoinCommand { get; }
        public ICommand RemoveCoinCommand { get; }
        public ICommand SaveStrategyCommand { get; }
        public ICommand DeleteStrategyCommand { get; }
        public ICommand ResetPromptCommand { get; }
        public ICommand ImportCommand { get; }
        public ICommand CancelCommand { get; }
        #endregion

        public TraderSettingDialogViewModel()
        {
            AddCoinCommand = new RelayCommand(AddCoin, _ => CanAddCoin);
            RemoveCoinCommand = new RelayCommand(RemoveCoin, _ => CanRemoveCoin);
            SaveStrategyCommand = new RelayCommand(SaveStrategy);
            DeleteStrategyCommand = new RelayCommand(DeleteStrategy, _ => CanDeleteStrategy);
            ResetPromptCommand = new RelayCommand(ResetPrompt);
            ImportCommand = new RelayCommand(Import);
            CancelCommand = new RelayCommand(Cancel);
        }

        public void Initialize(
            MenuTextData menuItem,
            List<string> availableCoins,
            List<string> selectedCoins,
            string customPrompt,
            StrategyLibrary strategyLibrary)
        {
            MenuItem = menuItem;
            StrategyLibrary = strategyLibrary ?? new StrategyLibrary();
            _allAvailableCoins = availableCoins.Concat(selectedCoins).Distinct().ToList();

            LoadStrategies();
            LoadCoins(availableCoins, selectedCoins);
            LoadCustomPrompt(customPrompt);
        }

        #region Private Methods
        private void LoadCustomPrompt(string customPrompt)
        {
            CustomPrompt = string.IsNullOrWhiteSpace(customPrompt)
                ? PromptUtil.DefaultCustomPrompt
                : customPrompt;
        }

        private void LoadCoins(List<string> availableCoins, List<string> selectedCoins)
        {
            AvailableCoins.Clear();
            foreach (var coin in availableCoins)
            {
                AvailableCoins.Add(coin);
            }

            SelectedCoins.Clear();
            foreach (var coin in selectedCoins)
            {
                SelectedCoins.Add(coin);
            }
        }

        private void LoadStrategies()
        {
            Strategies.Clear();
            Strategies.Add(MenuItem?.LblNewStrategy ?? "New Strategy");

            foreach (var strategy in StrategyLibrary.Strategies)
            {
                Strategies.Add(strategy.Name);
            }

            if (!string.IsNullOrEmpty(StrategyLibrary.ActiveStrategyId))
            {
                var activeStrategy = StrategyLibrary.Strategies.FirstOrDefault(s => s.Id == StrategyLibrary.ActiveStrategyId);
                if (activeStrategy != null)
                {
                    SelectedStrategyItem = activeStrategy.Name;
                    StrategyName = activeStrategy.Name;
                    _selectedStrategy = activeStrategy;
                }
                else
                {
                    SelectedStrategyItem = Strategies.FirstOrDefault();
                }
            }
            else
            {
                SelectedStrategyItem = Strategies.FirstOrDefault();
            }
        }

        private void OnStrategySelectionChanged()
        {
            if (_isLoadingStrategy) return;

            var selectedItem = SelectedStrategyItem;
            if (string.IsNullOrEmpty(selectedItem)) return;

            if (selectedItem == MenuItem?.LblNewStrategy || selectedItem == "New Strategy")
            {
                _selectedStrategy = null;
                StrategyName = string.Empty;
                ClearForm();
                CanDeleteStrategy = false;
            }
            else
            {
                var strategy = StrategyLibrary.Strategies.FirstOrDefault(s => s.Name == selectedItem);
                if (strategy != null)
                {
                    _selectedStrategy = strategy;
                    StrategyName = strategy.Name;
                    LoadStrategyToForm(strategy);
                    CanDeleteStrategy = true;
                }
            }
        }

        private void ClearForm()
        {
            _isLoadingStrategy = true;
            ScanInterval = "600";

            SelectedCoins.Clear();
            AvailableCoins.Clear();
            foreach (var coin in _allAvailableCoins)
            {
                AvailableCoins.Add(coin);
            }

            CustomPrompt = PromptUtil.DefaultCustomPrompt;
            _isLoadingStrategy = false;
        }

        private void LoadStrategyToForm(StrategyConfig strategy)
        {
            _isLoadingStrategy = true;
            ScanInterval = strategy.ScanInterval.ToString();

            SelectedCoins.Clear();
            AvailableCoins.Clear();

            foreach (var coin in strategy.SelectedCoins)
            {
                SelectedCoins.Add(coin);
            }

            foreach (var coin in _allAvailableCoins)
            {
                if (!strategy.SelectedCoins.Contains(coin))
                {
                    AvailableCoins.Add(coin);
                }
            }

            CustomPrompt = string.IsNullOrWhiteSpace(strategy.CustomPrompt)
                ? PromptUtil.DefaultCustomPrompt
                : strategy.CustomPrompt;

            _isLoadingStrategy = false;
        }

        private void AddCoin(object parameter)
        {
            if (SelectedAvailableCoin is string coin)
            {
                AvailableCoins.Remove(coin);
                SelectedCoins.Add(coin);
                SelectedAvailableCoin = null;
            }
        }

        private void RemoveCoin(object parameter)
        {
            if (SelectedSelectedCoin is string coin)
            {
                SelectedCoins.Remove(coin);
                AvailableCoins.Add(coin);
                SelectedSelectedCoin = null;
            }
        }

        private void SaveStrategy(object parameter)
        {
            var strategyName = StrategyName?.Trim();
            if (string.IsNullOrWhiteSpace(strategyName))
            {
                MessageBox.Show("Please enter a strategy name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(ScanInterval?.Trim(), out int scanInterval))
            {
                MessageBox.Show("Scan interval must be a number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var existingStrategy = StrategyLibrary.Strategies.FirstOrDefault(s => s.Name == strategyName && s != _selectedStrategy);
            if (existingStrategy != null)
            {
                MessageBox.Show("A strategy with this name already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_selectedStrategy == null)
            {
                _selectedStrategy = new StrategyConfig
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = strategyName,
                    ScanInterval = scanInterval,
                    SelectedCoins = SelectedCoins.ToList(),
                    CustomPrompt = CustomPrompt?.Trim() ?? "",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                StrategyLibrary.Strategies.Add(_selectedStrategy);
            }
            else
            {
                _selectedStrategy.Name = strategyName;
                _selectedStrategy.ScanInterval = scanInterval;
                _selectedStrategy.SelectedCoins = SelectedCoins.ToList();
                _selectedStrategy.CustomPrompt = CustomPrompt?.Trim() ?? "";
                _selectedStrategy.UpdatedAt = DateTime.Now;
            }

            StrategyLibrary.ActiveStrategyId = _selectedStrategy.Id;
            LoadStrategies();
            SelectedStrategyItem = _selectedStrategy.Name;
            MessageBox.Show("Strategy saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DeleteStrategy(object parameter)
        {
            if (_selectedStrategy == null)
            {
                MessageBox.Show("Please select a strategy to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete strategy '{_selectedStrategy.Name}'?",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                StrategyLibrary.Strategies.Remove(_selectedStrategy);
                if (StrategyLibrary.ActiveStrategyId == _selectedStrategy.Id)
                {
                    StrategyLibrary.ActiveStrategyId = string.Empty;
                }
                _selectedStrategy = null;
                LoadStrategies();
                ClearForm();
                MessageBox.Show("Strategy deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ResetPrompt(object parameter)
        {
            CustomPrompt = PromptUtil.DefaultCustomPrompt;
        }

        private void Import(object parameter)
        {
            if (string.IsNullOrWhiteSpace(ScanInterval?.Trim()))
            {
                MessageBox.Show("Scan interval cannot be empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(ScanInterval?.Trim(), out _))
            {
                MessageBox.Show("Scan interval must be a number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
        #endregion

        #region Public Methods
        public List<string> GetSelectedCoins()
        {
            return SelectedCoins.ToList();
        }

        public string GetCustomPrompt()
        {
            return CustomPrompt?.Trim() ?? "";
        }

        public StrategyLibrary GetStrategyLibrary()
        {
            return StrategyLibrary;
        }

        public StrategyConfig GetSelectedStrategy()
        {
            return _selectedStrategy;
        }
        #endregion
    }
}
