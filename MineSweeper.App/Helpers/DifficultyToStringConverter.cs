using MineSweeper.App.Extensions;
using MineSweeper.Core.Models;
using System.Globalization;
using System.Windows.Data;

namespace MineSweeper.App.Helpers
{
    /// <summary>
    /// - (EN) Converts a DifficultyLevel value into its display string representation for UI binding.
    /// - (VI) Chuyển đổi giá trị DifficultyLevel thành chuỗi hiển thị tương ứng để sử dụng trong UI.
    /// </summary>
    public class DifficultyToStringConverter : IValueConverter
    {
        /// <summary>
        /// - (EN) Converts a DifficultyLevel value to its display string using extension methods.
        /// - (VI) Chuyển đổi giá trị DifficultyLevel thành chuỗi hiển thị thông qua extension method.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DifficultyLevel difficulty)
                return difficulty.ToDisplayString();

            return string.Empty;
        }

        /// <summary>
        /// - (EN) The value to convert back.
        /// - (VI) Giá trị cần chuyển đổi ngược.
        /// </summary>

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
