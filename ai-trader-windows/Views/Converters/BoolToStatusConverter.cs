using System.Globalization;
using System.Windows.Data;

namespace AITrade.Converters
{
    public class BoolToStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return b ? "Effective" : "Ineffective";
            return "Ineffective";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s)
                return s == "Effective";
            return false;
        }
    }
}
