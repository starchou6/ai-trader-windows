using System.Collections.Generic;
using System.Linq;
using System.Windows;
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

        public TraderSettingDialog(MenuTextData menuItem, List<string> availableCoins, List<string> selectedCoins, string customPrompt)
        {
            InitializeComponent();
            MenuItem = menuItem;
            DataContext = this;
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
    }
}
