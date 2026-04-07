namespace MineSweeper.App.Helpers
{
    /// <summary>
    /// - (EN) Provides helper methods for formatting time values used in the UI.
    /// - (VI) Cung cấp các phương thức hỗ trợ định dạng thời gian dùng trong giao diện.
    /// </summary>
    public static class TimeFormatHelper
    {
        /// <summary>
        /// - (EN) Default time format (minutes:seconds).
        /// - (VI) Định dạng thời gian mặc định (phút:giây).
        /// </summary>
        private const string DefaultFormat = @"mm\:ss";

        /// <summary>
        /// - (EN) Fallback value when time is null.
        /// - (VI) Giá trị thay thế khi thời gian là null.
        /// </summary>
        private const string Fallback = "--:--";

        /// <summary>
        /// - (EN) Formats a nullable <see cref="TimeSpan"/> into a string using the default format.
        ///         Returns a fallback value if the input is null.
        /// - (VI) Định dạng <see cref="TimeSpan"/> nullable thành chuỗi theo định dạng mặc định.
        ///         Trả về giá trị thay thế nếu input là null.
        /// </summary>
        /// <param name="time">
        /// - (EN) The time value to format.
        /// - (VI) Giá trị thời gian cần định dạng.
        /// </param>
        /// <returns>
        /// - (EN) A formatted time string (e.g., "01:05") or fallback ("--:--") if null.
        /// - (VI) Chuỗi thời gian đã định dạng (ví dụ "01:05") hoặc giá trị thay thế ("--:--") nếu null.
        /// </returns>
        public static string Format(TimeSpan? time)
        {
            return time?.ToString(DefaultFormat) ?? Fallback;
        }
    }
}
