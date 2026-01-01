using System.Windows;

namespace AITrade
{
    public partial class InputPromptDialog : Window
    {
        public InputPromptDialog(string prompt)
        {
            InitializeComponent();
            PromptTextBox.Text = prompt;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
