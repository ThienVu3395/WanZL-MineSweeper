using MineSweeper.App.ViewModels;
using MineSweeper.Core.Models;

namespace MineSweeper.Tests.App.ViewModels;

/// <summary>
/// - (EN) Contains unit tests for CellViewModel, including coordinate mapping, display text rules, and property change notifications.
/// - (VI) Chứa các bài kiểm thử đơn vị cho CellViewModel, bao gồm ánh xạ tọa độ, quy tắc hiển thị nội dung và thông báo thay đổi thuộc tính.
/// </summary>
public class CellViewModelTests
{
    /// <summary>
    /// - (EN) Should expose the row and column values from the underlying cell correctly.
    /// - (VI) Phải hiển thị đúng giá trị hàng và cột từ cell nền tảng.
    /// </summary>
    [Fact]
    public void Constructor_ShouldExposeCoordinatesCorrectly()
    {
        // Arrange
        var cell = new Cell(2, 3);

        // Act
        var vm = new CellViewModel(cell);

        // Assert
        Assert.Equal(2, vm.Row);
        Assert.Equal(3, vm.Column);
    }

    /// <summary>
    /// - (EN) Should return an empty string when the cell is hidden and not flagged.
    /// - (VI) Phải trả về chuỗi rỗng khi ô đang ẩn và không được cắm cờ.
    /// </summary>
    [Fact]
    public void DisplayText_ShouldBeEmpty_WhenCellIsHidden()
    {
        var cell = new Cell(0, 0)
        {
            IsRevealed = false,
            IsFlagged = false,
            IsMine = false,
            AdjacentMines = 3
        };

        var vm = new CellViewModel(cell);

        Assert.Equal(string.Empty, vm.DisplayText);
    }

    /// <summary>
    /// - (EN) Should show the flag icon when the cell is hidden and flagged.
    /// - (VI) Phải hiển thị biểu tượng cờ khi ô đang ẩn và được cắm cờ.
    /// </summary>
    [Fact]
    public void DisplayText_ShouldShowFlag_WhenCellIsFlagged()
    {
        var cell = new Cell(0, 0)
        {
            IsRevealed = false,
            IsFlagged = true
        };

        var vm = new CellViewModel(cell);

        Assert.Equal("🚩", vm.DisplayText);
    }

    /// <summary>
    /// - (EN) Should show the mine icon when the cell is revealed and contains a mine.
    /// - (VI) Phải hiển thị biểu tượng mìn khi ô đã được mở và có chứa mìn.
    /// </summary>
    [Fact]
    public void DisplayText_ShouldShowMine_WhenRevealedAndIsMine()
    {
        var cell = new Cell(0, 0)
        {
            IsRevealed = true,
            IsMine = true
        };

        var vm = new CellViewModel(cell);

        Assert.Equal("💣", vm.DisplayText);
    }

    /// <summary>
    /// - (EN) Should show the adjacent mine count when the cell is revealed and has adjacent mines.
    /// - (VI) Phải hiển thị số lượng mìn lân cận khi ô đã được mở và có mìn ở các ô xung quanh.
    /// </summary>
    [Fact]
    public void DisplayText_ShouldShowNumber_WhenRevealedAndHasAdjacentMines()
    {
        var cell = new Cell(0, 0)
        {
            IsRevealed = true,
            IsMine = false,
            AdjacentMines = 4
        };

        var vm = new CellViewModel(cell);

        Assert.Equal("4", vm.DisplayText);
    }

    /// <summary>
    /// - (EN) Should return an empty string when the cell is revealed and has zero adjacent mines.
    /// - (VI) Phải trả về chuỗi rỗng khi ô đã được mở và không có mìn lân cận.
    /// </summary>
    [Fact]
    public void DisplayText_ShouldBeEmpty_WhenRevealedAndAdjacentMinesIsZero()
    {
        var cell = new Cell(0, 0)
        {
            IsRevealed = true,
            IsMine = false,
            AdjacentMines = 0
        };

        var vm = new CellViewModel(cell);

        Assert.Equal(string.Empty, vm.DisplayText);
    }

    /// <summary>
    /// - (EN) Should raise PropertyChanged notifications for all relevant UI properties when Refresh is called.
    /// - (VI) Phải phát sinh thông báo PropertyChanged cho tất cả thuộc tính UI liên quan khi gọi Refresh.
    /// </summary>
    [Fact]
    public void Refresh_ShouldRaisePropertyChanged_ForAllRelevantProperties()
    {
        // Arrange
        var cell = new Cell(0, 0);
        var vm = new CellViewModel(cell);

        var changedProperties = new List<string>();

        vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName != null)
                changedProperties.Add(e.PropertyName);
        };

        // Act
        vm.Refresh();

        // Assert
        Assert.Contains(nameof(vm.IsRevealed), changedProperties);
        Assert.Contains(nameof(vm.IsHidden), changedProperties);
        Assert.Contains(nameof(vm.IsFlagged), changedProperties);
        Assert.Contains(nameof(vm.IsMine), changedProperties);
        Assert.Contains(nameof(vm.IsExplodedMine), changedProperties);
        Assert.Contains(nameof(vm.IsIncorrectFlag), changedProperties);
        Assert.Contains(nameof(vm.AdjacentMines), changedProperties);
        Assert.Contains(nameof(vm.DisplayText), changedProperties);
    }

    /// <summary>
    /// - (EN) Should reflect the updated model state after Refresh is called.
    /// - (VI) Phải phản ánh đúng trạng thái model đã được cập nhật sau khi gọi Refresh.
    /// </summary>
    [Fact]
    public void Refresh_ShouldReflectUpdatedModelState()
    {
        // Arrange
        var cell = new Cell(0, 0)
        {
            IsRevealed = false,
            IsFlagged = false,
            AdjacentMines = 0
        };

        var vm = new CellViewModel(cell);

        // Update model
        cell.IsFlagged = true;
        cell.AdjacentMines = 2;

        // Act
        vm.Refresh();

        // Assert
        Assert.True(vm.IsFlagged);
        Assert.Equal(2, vm.AdjacentMines);
        Assert.Equal("🚩", vm.DisplayText);
    }

    /// <summary>
    /// - (EN) Verifies that the exploded mine is displayed with the explosion icon.
    /// - (VI) Kiểm tra ô mìn phát nổ sẽ hiển thị biểu tượng vụ nổ.
    /// </summary>
    [Fact]
    public void DisplayText_ShouldShowExplosion_WhenCellIsExplodedMine()
    {
        // Arrange
        var cell = new Cell(0, 0)
        {
            IsRevealed = true,
            IsMine = true,
            IsExplodedMine = true
        };

        var vm = new CellViewModel(cell);

        // Assert
        Assert.True(vm.IsExplodedMine);
        Assert.Equal("💥", vm.DisplayText);
    }

    /// <summary>
    /// - (EN) Verifies that an incorrect flag is displayed with the incorrect-marker icon.
    /// - (VI) Kiểm tra cờ sai sẽ hiển thị biểu tượng đánh dấu sai.
    /// </summary>
    [Fact]
    public void DisplayText_ShouldShowIncorrectMarker_WhenFlagIsIncorrect()
    {
        // Arrange
        var cell = new Cell(0, 0)
        {
            IsRevealed = true,
            IsFlagged = true,
            IsMine = false,
            IsIncorrectFlag = true
        };

        var vm = new CellViewModel(cell);

        // Assert
        Assert.True(vm.IsIncorrectFlag);
        Assert.Equal("❌", vm.DisplayText);
    }

    /// <summary>
    /// - (EN) Verifies that the incorrect-flag marker takes priority over other display states.
    /// - (VI) Kiểm tra biểu tượng cờ sai sẽ được ưu tiên hiển thị hơn các trạng thái hiển thị khác.
    /// </summary>
    [Fact]
    public void DisplayText_ShouldPrioritizeIncorrectFlag_OverOtherStates()
    {
        // Arrange
        var cell = new Cell(0, 0)
        {
            IsRevealed = true,
            IsFlagged = true,
            IsMine = true,
            IsExplodedMine = true,
            IsIncorrectFlag = true,
            AdjacentMines = 3
        };

        var vm = new CellViewModel(cell);

        // Assert
        Assert.Equal("❌", vm.DisplayText);
    }

    /// <summary>
    /// - (EN) Verifies that the exploded-mine marker takes priority over the normal mine display.
    /// - (VI) Kiểm tra biểu tượng mìn phát nổ sẽ được ưu tiên hiển thị hơn biểu tượng mìn thông thường.
    /// </summary>
    [Fact]
    public void DisplayText_ShouldPrioritizeExplodedMine_OverNormalMineDisplay()
    {
        // Arrange
        var cell = new Cell(0, 0)
        {
            IsRevealed = true,
            IsMine = true,
            IsExplodedMine = true,
            IsIncorrectFlag = false
        };

        var vm = new CellViewModel(cell);

        // Assert
        Assert.Equal("💥", vm.DisplayText);
    }
}