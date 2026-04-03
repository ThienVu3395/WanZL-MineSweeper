using System.Globalization;
using System.Windows;
using MineSweeper.App.Helpers;

namespace MineSweeper.Tests.App.Helpers;

public class NullToVisibilityConverterTests
{
    private readonly NullToVisibilityConverter _converter = new();

    [Fact]
    public void Convert_ShouldReturnCollapsed_WhenValueIsNull()
    {
        var result = _converter.Convert(null, null!, null!, CultureInfo.InvariantCulture);

        Assert.Equal(Visibility.Collapsed, result);
    }

    [Fact]
    public void Convert_ShouldReturnCollapsed_WhenValueIsEmpty()
    {
        var result = _converter.Convert(string.Empty, null!, null!, CultureInfo.InvariantCulture);

        Assert.Equal(Visibility.Collapsed, result);
    }

    [Fact]
    public void Convert_ShouldReturnVisible_WhenValueIsNotEmpty()
    {
        var result = _converter.Convert("Hello", null!, null!, CultureInfo.InvariantCulture);

        Assert.Equal(Visibility.Visible, result);
    }

    [Fact]
    public void ConvertBack_ShouldThrowNotImplemented()
    {
        Assert.Throws<NotImplementedException>(() =>
            _converter.ConvertBack(null!, null!, null!, CultureInfo.InvariantCulture));
    }
}