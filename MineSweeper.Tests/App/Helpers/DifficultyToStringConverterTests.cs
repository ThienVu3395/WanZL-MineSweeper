using System.Globalization;
using MineSweeper.App.Helpers;
using MineSweeper.Core.Models;

namespace MineSweeper.Tests.App.Helpers;

/// <summary>
/// - (EN) Contains unit tests for <see cref="DifficultyToStringConverter"/>.
/// - (VI) Chứa các unit test cho <see cref="DifficultyToStringConverter"/>.
/// </summary>
public class DifficultyToStringConverterTests
{
    private readonly DifficultyToStringConverter _converter = new();

    /// <summary>
    /// - (EN) Verifies that Convert returns the formatted difficulty display string when the input is a valid difficulty level.
    /// - (VI) Kiểm tra Convert sẽ trả về chuỗi hiển thị độ khó đã định dạng khi đầu vào là một mức độ khó hợp lệ.
    /// </summary>
    [Fact]
    public void Convert_ShouldReturnDisplayString_WhenValueIsDifficultyLevel()
    {
        var result = _converter.Convert(
            DifficultyLevel.Beginner,
            null!,
            null!,
            CultureInfo.InvariantCulture);

        Assert.Equal("Beginner (9x9 | 10 mines)", result);
    }

    /// <summary>
    /// - (EN) Verifies that Convert returns an empty string when the input is not a difficulty level.
    /// - (VI) Kiểm tra Convert sẽ trả về chuỗi rỗng khi đầu vào không phải là mức độ khó.
    /// </summary>
    [Fact]
    public void Convert_ShouldReturnEmpty_WhenValueIsNotDifficultyLevel()
    {
        var result = _converter.Convert(
            "not-a-difficulty",
            null!,
            null!,
            CultureInfo.InvariantCulture);

        Assert.Equal(string.Empty, result);
    }

    /// <summary>
    /// - (EN) Verifies that ConvertBack throws <see cref="NotImplementedException"/> because reverse conversion is not supported.
    /// - (VI) Kiểm tra ConvertBack sẽ ném ra <see cref="NotImplementedException"/> vì không hỗ trợ chuyển đổi ngược.
    /// </summary>
    [Fact]
    public void ConvertBack_ShouldThrowNotImplementedException()
    {
        Assert.Throws<NotImplementedException>(() =>
            _converter.ConvertBack(null!, null!, null!, CultureInfo.InvariantCulture));
    }
}