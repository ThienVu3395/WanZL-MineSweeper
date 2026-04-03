using MineSweeper.App.Extensions;
using MineSweeper.Core.Models;
using System.Globalization;
using System.Windows.Data;

namespace MineSweeper.App.Helpers
{
    public class DifficultyToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DifficultyLevel difficulty)
                return difficulty.ToDisplayString();

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
