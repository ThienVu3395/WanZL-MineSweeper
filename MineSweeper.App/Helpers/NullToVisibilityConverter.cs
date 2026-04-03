using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace MineSweeper.App.Helpers
{
    /// <summary>
    /// Converts null or empty string to Collapsed, otherwise Visible.
    /// </summary>
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value as string)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}