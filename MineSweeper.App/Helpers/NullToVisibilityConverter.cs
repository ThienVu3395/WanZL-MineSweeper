using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace MineSweeper.App.Helpers
{
    /// <summary>
    /// - (EN) Converts a null or empty string into a Visibility state for UI rendering.
    /// - (VI) Chuyển đổi chuỗi null hoặc rỗng thành trạng thái Visibility để hiển thị trên UI.
    /// </summary>
    public class NullToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// - (EN) Converts the input value to Visibility.Collapsed if null or empty; otherwise Visibility.Visible.
        /// - (VI) Chuyển giá trị đầu vào thành Visibility.Collapsed nếu null hoặc rỗng; ngược lại là Visibility.Visible.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value as string)
                ? Visibility.Collapsed
                : Visibility.Visible;
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