using System.Windows;

namespace AITrade
{
    public partial class ApiImportDialog : Window
    {
        public ApiImportDialog()
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
            if (string.IsNullOrWhiteSpace(Key.Text.Trim()) || string.IsNullOrWhiteSpace(Secret.Text.Trim()) || string.IsNullOrWhiteSpace(AiKey.Text.Trim()))
            {
                MessageBox.Show("Api key or secret cannot be empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DialogResult = true;
            Close();
        }
    }
}
