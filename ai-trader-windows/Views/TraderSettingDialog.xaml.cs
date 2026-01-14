using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using AITrade.Entity;
using AITrade.Utils;
using AITrade.ViewModels.Entity;

namespace AITrader.Views
{
    /// <summary>
    /// TraderSettingDialog.xaml 的交互逻辑
    /// </summary>
    public partial class TraderSettingDialog : Window
    {
        public MenuTextData MenuItem { get; set; }

        private StrategyLibrary _strategyLibrary;
        private List<string> _allAvailableCoins;
        private StrategyConfig _selectedStrategy;
        private bool _isLoadingStrategy = false;

        public TraderSettingDialog(MenuTextData menuItem, List<string> availableCoins, List<string> selectedCoins, string customPrompt, StrategyLibrary strategyLibrary)
        {
            InitializeComponent();
            MenuItem = menuItem;
            _strategyLibrary = strategyLibrary ?? new StrategyLibrary();
            _allAvailableCoins = availableCoins.Concat(selectedCoins).Distinct().ToList();
            DataContext = this;
            LoadStrategies();
            LoadCoins(availableCoins, selectedCoins);
            LoadCustomPrompt(customPrompt);
        }

        private void LoadCustomPrompt(string customPrompt)
        {
            CustomPromptTextBox.Text = string.IsNullOrWhiteSpace(customPrompt)
                ? PromptUtil.DefaultCustomPrompt
                : customPrompt;
        }

        private void LoadCoins(List<string> availableCoins, List<string> selectedCoins)
        {
            foreach (var coin in availableCoins)
            {
                AvailableCoinsListBox.Items.Add(coin);
            }
            foreach (var coin in selectedCoins)
            {
                SelectedCoinsListBox.Items.Add(coin);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ScanInterval.Text.Trim()))
            {
                MessageBox.Show("Scan interval cannot be empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(ScanInterval.Text.Trim(), out _))
            {
                MessageBox.Show("Scan interval must be a number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DialogResult = true;
            Close();
        }

        private void AddCoin_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = AvailableCoinsListBox.SelectedItems.Cast<object>().ToList();
            foreach (var item in selectedItems)
            {
                AvailableCoinsListBox.Items.Remove(item);
                SelectedCoinsListBox.Items.Add(item);
            }
            UpdateButtonStates();
        }

        private void RemoveCoin_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = SelectedCoinsListBox.SelectedItems.Cast<object>().ToList();
            foreach (var item in selectedItems)
            {
                SelectedCoinsListBox.Items.Remove(item);
                AvailableCoinsListBox.Items.Add(item);
            }
            UpdateButtonStates();
        }

        private void AvailableCoinsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            AddButton.IsEnabled = AvailableCoinsListBox.SelectedItems.Count > 0;
        }

        private void SelectedCoinsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            RemoveButton.IsEnabled = SelectedCoinsListBox.SelectedItems.Count > 0;
        }

        private void UpdateButtonStates()
        {
            AddButton.IsEnabled = AvailableCoinsListBox.SelectedItems.Count > 0;
            RemoveButton.IsEnabled = SelectedCoinsListBox.SelectedItems.Count > 0;
        }

        public List<string> GetSelectedCoins()
        {
            return SelectedCoinsListBox.Items.Cast<string>().ToList();
        }

        public string GetCustomPrompt()
        {
            return CustomPromptTextBox.Text.Trim();
        }

        private void ResetPromptButton_Click(object sender, RoutedEventArgs e)
        {
            CustomPromptTextBox.Text = PromptUtil.DefaultCustomPrompt;
        }

        private void LoadStrategies()
        {
            StrategyComboBox.Items.Clear();
            StrategyComboBox.Items.Add(MenuItem.LblNewStrategy);
            foreach (var strategy in _strategyLibrary.Strategies)
            {
                StrategyComboBox.Items.Add(strategy.Name);
            }

            if (!string.IsNullOrEmpty(_strategyLibrary.ActiveStrategyId))
            {
                var activeStrategy = _strategyLibrary.Strategies.FirstOrDefault(s => s.Id == _strategyLibrary.ActiveStrategyId);
                if (activeStrategy != null)
                {
                    StrategyComboBox.SelectedItem = activeStrategy.Name;
                    StrategyNameTextBox.Text = activeStrategy.Name;
                    _selectedStrategy = activeStrategy;
                }
                else
                {
                    StrategyComboBox.SelectedIndex = 0;
                }
            }
            else
            {
                StrategyComboBox.SelectedIndex = 0;
            }
        }

        private void StrategyComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (_isLoadingStrategy) return;

            var selectedItem = StrategyComboBox.SelectedItem as string;
            if (string.IsNullOrEmpty(selectedItem)) return;

            if (selectedItem == MenuItem.LblNewStrategy)
            {
                _selectedStrategy = null;
                StrategyNameTextBox.Text = string.Empty;
                ClearForm();
                DeleteStrategyButton.IsEnabled = false;
            }
            else
            {
                var strategy = _strategyLibrary.Strategies.FirstOrDefault(s => s.Name == selectedItem);
                if (strategy != null)
                {
                    _selectedStrategy = strategy;
                    StrategyNameTextBox.Text = strategy.Name;
                    LoadStrategyToForm(strategy);
                    DeleteStrategyButton.IsEnabled = true;
                }
            }
        }

        private void ClearForm()
        {
            _isLoadingStrategy = true;
            ScanInterval.Text = "600";
            SelectedCoinsListBox.Items.Clear();
            AvailableCoinsListBox.Items.Clear();
            foreach (var coin in _allAvailableCoins)
            {
                AvailableCoinsListBox.Items.Add(coin);
            }
            CustomPromptTextBox.Text = PromptUtil.DefaultCustomPrompt;
            _isLoadingStrategy = false;
        }

        private void LoadStrategyToForm(StrategyConfig strategy)
        {
            _isLoadingStrategy = true;
            ScanInterval.Text = strategy.ScanInterval.ToString();

            SelectedCoinsListBox.Items.Clear();
            AvailableCoinsListBox.Items.Clear();

            foreach (var coin in strategy.SelectedCoins)
            {
                SelectedCoinsListBox.Items.Add(coin);
            }

            foreach (var coin in _allAvailableCoins)
            {
                if (!strategy.SelectedCoins.Contains(coin))
                {
                    AvailableCoinsListBox.Items.Add(coin);
                }
            }

            CustomPromptTextBox.Text = string.IsNullOrWhiteSpace(strategy.CustomPrompt)
                ? PromptUtil.DefaultCustomPrompt
                : strategy.CustomPrompt;
            _isLoadingStrategy = false;
        }

        private void SaveStrategy_Click(object sender, RoutedEventArgs e)
        {
            var strategyName = StrategyNameTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(strategyName))
            {
                MessageBox.Show("Please enter a strategy name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(ScanInterval.Text.Trim(), out int scanInterval))
            {
                MessageBox.Show("Scan interval must be a number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var existingStrategy = _strategyLibrary.Strategies.FirstOrDefault(s => s.Name == strategyName && s != _selectedStrategy);
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
                    SelectedCoins = GetSelectedCoins(),
                    CustomPrompt = GetCustomPrompt(),
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                _strategyLibrary.Strategies.Add(_selectedStrategy);
            }
            else
            {
                _selectedStrategy.Name = strategyName;
                _selectedStrategy.ScanInterval = scanInterval;
                _selectedStrategy.SelectedCoins = GetSelectedCoins();
                _selectedStrategy.CustomPrompt = GetCustomPrompt();
                _selectedStrategy.UpdatedAt = DateTime.Now;
            }

            _strategyLibrary.ActiveStrategyId = _selectedStrategy.Id;
            LoadStrategies();
            StrategyComboBox.SelectedItem = _selectedStrategy.Name;
            MessageBox.Show("Strategy saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DeleteStrategy_Click(object sender, RoutedEventArgs e)
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
                _strategyLibrary.Strategies.Remove(_selectedStrategy);
                if (_strategyLibrary.ActiveStrategyId == _selectedStrategy.Id)
                {
                    _strategyLibrary.ActiveStrategyId = string.Empty;
                }
                _selectedStrategy = null;
                LoadStrategies();
                ClearForm();
                MessageBox.Show("Strategy deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public StrategyLibrary GetStrategyLibrary()
        {
            return _strategyLibrary;
        }

        public StrategyConfig GetSelectedStrategy()
        {
            return _selectedStrategy;
        }
    }
}
