using MineSweeper.App.Extensions;
using MineSweeper.Core.Models;

namespace MineSweeper.Tests.App.Extensions;

/// <summary>
/// - (EN) Contains unit tests for <see cref="DifficultyExtensions"/>.
/// - (VI) Chứa các unit test cho <see cref="DifficultyExtensions"/>.
/// </summary>
public class DifficultyExtensionsTests
{
    #region ToDisplayString

    /// <summary>
    /// - (EN) Verifies that Beginner difficulty returns the correct display string.
    /// - (VI) Kiểm tra độ khó Beginner trả về đúng chuỗi hiển thị.
    /// </summary>
    [Fact]
    public void ToDisplayString_ShouldReturnCorrectValue_ForBeginner()
    {
        // Arrange
        var difficulty = DifficultyLevel.Beginner;

        // Act
        var result = difficulty.ToDisplayString();

        // Assert
        Assert.Equal("Beginner (9x9 | 10 mines)", result);
    }

    /// <summary>
    /// - (EN) Verifies that Intermediate difficulty returns the correct display string.
    /// - (VI) Kiểm tra độ khó Intermediate trả về đúng chuỗi hiển thị.
    /// </summary>
    [Fact]
    public void ToDisplayString_ShouldReturnCorrectValue_ForIntermediate()
    {
        // Arrange
        var difficulty = DifficultyLevel.Intermediate;

        // Act
        var result = difficulty.ToDisplayString();

        // Assert
        Assert.Equal("Intermediate (16x16 | 40 mines)", result);
    }

    /// <summary>
    /// - (EN) Verifies that Expert difficulty returns the correct display string.
    /// - (VI) Kiểm tra độ khó Expert trả về đúng chuỗi hiển thị.
    /// </summary>
    [Fact]
    public void ToDisplayString_ShouldReturnCorrectValue_ForExpert()
    {
        // Arrange
        var difficulty = DifficultyLevel.Expert;

        // Act
        var result = difficulty.ToDisplayString();

        // Assert
        Assert.Equal("Expert (16x30 | 99 mines)", result);
    }

    /// <summary>
    /// - (EN) Verifies that unsupported difficulty values fall back to the enum string.
    /// - (VI) Kiểm tra các giá trị độ khó không được hỗ trợ sẽ fallback về chuỗi enum gốc.
    /// </summary>
    [Fact]
    public void ToDisplayString_ShouldFallbackToEnumString_ForUnsupportedDifficulty()
    {
        // Arrange
        var difficulty = DifficultyLevel.Custom;

        // Act
        var result = difficulty.ToDisplayString();

        // Assert
        Assert.Equal(nameof(DifficultyLevel.Custom), result);
    }

    #endregion

    #region ToPreset

    /// <summary>
    /// - (EN) Verifies that Beginner maps to the correct preset.
    /// - (VI) Kiểm tra Beginner được map đúng sang preset tương ứng.
    /// </summary>
    [Fact]
    public void ToPreset_ShouldReturnCorrectPreset_ForBeginner()
    {
        // Arrange
        var difficulty = DifficultyLevel.Beginner;

        // Act
        var preset = difficulty.ToPreset();

        // Assert
        Assert.Equal(9, preset.Rows);
        Assert.Equal(9, preset.Columns);
        Assert.Equal(10, preset.MineCount);
    }

    /// <summary>
    /// - (EN) Verifies that Intermediate maps to the correct preset.
    /// - (VI) Kiểm tra Intermediate được map đúng sang preset tương ứng.
    /// </summary>
    [Fact]
    public void ToPreset_ShouldReturnCorrectPreset_ForIntermediate()
    {
        // Arrange
        var difficulty = DifficultyLevel.Intermediate;

        // Act
        var preset = difficulty.ToPreset();

        // Assert
        Assert.Equal(16, preset.Rows);
        Assert.Equal(16, preset.Columns);
        Assert.Equal(40, preset.MineCount);
    }

    /// <summary>
    /// - (EN) Verifies that Expert maps to the correct preset.
    /// - (VI) Kiểm tra Expert được map đúng sang preset tương ứng.
    /// </summary>
    [Fact]
    public void ToPreset_ShouldReturnCorrectPreset_ForExpert()
    {
        // Arrange
        var difficulty = DifficultyLevel.Expert;

        // Act
        var preset = difficulty.ToPreset();

        // Assert
        Assert.Equal(16, preset.Rows);
        Assert.Equal(30, preset.Columns);
        Assert.Equal(99, preset.MineCount);
    }

    /// <summary>
    /// - (EN) Verifies that unsupported difficulty values fall back to the default beginner preset.
    /// - (VI) Kiểm tra các giá trị độ khó không được hỗ trợ sẽ fallback về preset Beginner mặc định.
    /// </summary>
    [Fact]
    public void ToPreset_ShouldFallbackToBeginnerPreset_ForUnsupportedDifficulty()
    {
        // Arrange
        var difficulty = DifficultyLevel.Custom;

        // Act
        var preset = difficulty.ToPreset();

        // Assert
        Assert.Equal(9, preset.Rows);
        Assert.Equal(9, preset.Columns);
        Assert.Equal(10, preset.MineCount);
    }

    /// <summary>
    /// - (EN) Verifies that each call to ToPreset returns a valid preset instance.
    /// - (VI) Kiểm tra mỗi lần gọi ToPreset sẽ trả về một instance preset hợp lệ.
    /// </summary>
    [Fact]
    public void ToPreset_ShouldReturnNonNullPreset()
    {
        // Arrange
        var difficulty = DifficultyLevel.Beginner;

        // Act
        var preset = difficulty.ToPreset();

        // Assert
        Assert.NotNull(preset);
    }

    #endregion
}