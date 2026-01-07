using System.Windows;

namespace AITrader.Views
{
    /// <summary>
    /// TraderSettingDialog.xaml 的交互逻辑
    /// </summary>
    public partial class TraderSettingDialog : Window
    {
        public TraderSettingDialog()
        {
            InitializeComponent();
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
    }
}
