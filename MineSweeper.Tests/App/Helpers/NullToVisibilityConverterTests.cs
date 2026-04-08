using System.Globalization;
using System.Windows;
using MineSweeper.App.Helpers;

namespace MineSweeper.Tests.App.Helpers;

/// <summary>
/// - (EN) Contains unit tests for NullToVisibilityConverter.
/// - (VI) Chứa các bài kiểm thử đơn vị cho NullToVisibilityConverter.
/// </summary>
public class NullToVisibilityConverterTests
{
    private readonly NullToVisibilityConverter _converter = new();

    /// <summary>
    /// - (EN) Should return Visibility.Collapsed when the input value is null.
    /// - (VI) Phải trả về Visibility.Collapsed khi giá trị đầu vào là null.
    /// </summary>
    [Fact]
    public void Convert_ShouldReturnCollapsed_WhenValueIsNull()
    {
        var result = _converter.Convert(null, null!, null!, CultureInfo.InvariantCulture);

        Assert.Equal(Visibility.Collapsed, result);
    }

    /// <summary>
    /// - (EN) Should return Visibility.Collapsed when the input value is an empty string.
    /// - (VI) Phải trả về Visibility.Collapsed khi giá trị đầu vào là chuỗi rỗng.
    /// </summary>
    [Fact]
    public void Convert_ShouldReturnCollapsed_WhenValueIsEmpty()
    {
        var result = _converter.Convert(string.Empty, null!, null!, CultureInfo.InvariantCulture);

        Assert.Equal(Visibility.Collapsed, result);
    }

    /// <summary>
    /// - (EN) Should return Visibility.Visible when the input value is not empty.
    /// - (VI) Phải trả về Visibility.Visible khi giá trị đầu vào không rỗng.
    /// </summary>
    [Fact]
    public void Convert_ShouldReturnVisible_WhenValueIsNotEmpty()
    {
        var result = _converter.Convert("Hello", null!, null!, CultureInfo.InvariantCulture);

        Assert.Equal(Visibility.Visible, result);
    }

    /// <summary>
    /// - (EN) Should throw NotImplementedException when ConvertBack is called.
    /// - (VI) Phải ném ra NotImplementedException khi gọi ConvertBack.
    /// </summary>
    [Fact]
    public void ConvertBack_ShouldThrowNotImplemented()
    {
        Assert.Throws<NotImplementedException>(() =>
            _converter.ConvertBack(null!, null!, null!, CultureInfo.InvariantCulture));
    }
}