using MineSweeper.App.Helpers;

namespace MineSweeper.Tests.App.Helpers
{
    /// <summary>
    /// - (EN) Unit tests for <see cref="TimeFormatHelper"/>.
    /// - (VI) Unit test cho <see cref="TimeFormatHelper"/>.
    /// </summary>
    public class TimeFormatHelperTests
    {
        /// <summary>
        /// - (EN) Verifies that Format returns the fallback value when the input time is null.
        /// - (VI) Kiểm tra Format trả về giá trị fallback khi thời gian đầu vào là null.
        /// </summary>
        [Fact]
        public void Format_ShouldReturnFallback_WhenNull()
        {
            var result = TimeFormatHelper.Format(null);

            Assert.Equal("--:--", result);
        }

        /// <summary>
        /// - (EN) Verifies that Format correctly formats a valid TimeSpan value.
        /// - (VI) Kiểm tra Format định dạng đúng với TimeSpan hợp lệ.
        /// </summary>
        [Fact]
        public void Format_ShouldFormatCorrectly()
        {
            var result = TimeFormatHelper.Format(TimeSpan.FromSeconds(65));

            Assert.Equal("01:05", result);
        }
    }
}
