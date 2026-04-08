using MineSweeper.Core.Models;

namespace MineSweeper.Tests.Core.Models;

/// <summary>
/// - (EN) Contains unit tests for verifying the behavior of the <see cref="Cell"/> model.
/// - (VI) Chứa các unit test dùng để kiểm tra hành vi của model <see cref="Cell"/>.
/// </summary>
public class CellTests
{
    /// <summary>
    /// - (EN) Verifies that the cell constructor stores the provided row and column coordinates correctly.
    /// - (VI) Kiểm tra constructor của cell sẽ lưu đúng tọa độ hàng và cột được cung cấp.
    /// </summary>
    [Fact]
    public void Constructor_ShouldSetCoordinatesCorrectly()
    {
        // Arrange
        int row = 2;
        int column = 3;

        // Act
        var cell = new Cell(row, column);

        // Assert
        Assert.Equal(2, cell.Row);
        Assert.Equal(3, cell.Column);
    }

    /// <summary>
    /// - (EN) Verifies that a newly created cell starts with the expected default state values.
    /// - (VI) Kiểm tra một cell mới được tạo sẽ có các giá trị trạng thái mặc định như mong đợi.
    /// </summary>
    [Fact]
    public void Constructor_ShouldInitializeDefaultState()
    {
        // Arrange & Act
        var cell = new Cell(0, 0);

        // Assert
        Assert.False(cell.IsMine);
        Assert.False(cell.IsRevealed);
        Assert.False(cell.IsFlagged);
        Assert.Equal(0, cell.AdjacentMines);
        Assert.False(cell.IsExplodedMine);
        Assert.False(cell.IsIncorrectFlag);
    }

    /// <summary>
    /// - (EN) Verifies that the cell properties can be updated to represent gameplay state changes.
    /// - (VI) Kiểm tra các thuộc tính của cell có thể được cập nhật để phản ánh các thay đổi trạng thái trong gameplay.
    /// </summary>
    [Fact]
    public void Properties_ShouldAllowStateUpdates()
    {
        // Arrange
        var cell = new Cell(1, 1);

        // Act
        cell.IsMine = true;
        cell.IsRevealed = true;
        cell.IsFlagged = true;
        cell.AdjacentMines = 3;
        cell.IsExplodedMine = true;
        cell.IsIncorrectFlag = true;

        // Assert
        Assert.True(cell.IsMine);
        Assert.True(cell.IsRevealed);
        Assert.True(cell.IsFlagged);
        Assert.Equal(3, cell.AdjacentMines);
        Assert.True(cell.IsExplodedMine);
        Assert.True(cell.IsIncorrectFlag);
    }

    /// <summary>
    /// - (EN) Verifies that row and column coordinates can be reassigned after construction.
    /// - (VI) Kiểm tra tọa độ hàng và cột có thể được gán lại sau khi khởi tạo.
    /// </summary>
    [Fact]
    public void Coordinates_ShouldAllowUpdates_AfterConstruction()
    {
        // Arrange
        var cell = new Cell(0, 0);

        // Act
        cell.Row = 4;
        cell.Column = 5;

        // Assert
        Assert.Equal(4, cell.Row);
        Assert.Equal(5, cell.Column);
    }

    /// <summary>
    /// - (EN) Verifies that adjacent mine count supports zero and positive values used during gameplay.
    /// - (VI) Kiểm tra số lượng mìn lân cận hỗ trợ các giá trị bằng không và dương được dùng trong gameplay.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(8)]
    public void AdjacentMines_ShouldStoreExpectedValue(int adjacentMines)
    {
        // Arrange
        var cell = new Cell(0, 0);

        // Act
        cell.AdjacentMines = adjacentMines;

        // Assert
        Assert.Equal(adjacentMines, cell.AdjacentMines);
    }

    /// <summary>
    /// - (EN) Verifies that a flagged cell can still be marked as a mine candidate without being revealed.
    /// - (VI) Kiểm tra một cell được cắm cờ vẫn có thể được đánh dấu là nghi ngờ có mìn mà chưa bị mở.
    /// </summary>
    [Fact]
    public void FlaggedCell_ShouldRemainHidden_WhenNotRevealed()
    {
        // Arrange
        var cell = new Cell(1, 2);

        // Act
        cell.IsFlagged = true;

        // Assert
        Assert.True(cell.IsFlagged);
        Assert.False(cell.IsRevealed);
    }

    /// <summary>
    /// - (EN) Verifies that exploded-mine and incorrect-flag markers are independent state flags.
    /// - (VI) Kiểm tra các cờ đánh dấu mìn phát nổ và cờ sai là các trạng thái độc lập với nhau.
    /// </summary>
    [Fact]
    public void ExplosionAndIncorrectFlagMarkers_ShouldBeIndependent()
    {
        // Arrange
        var cell = new Cell(0, 0);

        // Act
        cell.IsExplodedMine = true;
        cell.IsIncorrectFlag = false;

        // Assert
        Assert.True(cell.IsExplodedMine);
        Assert.False(cell.IsIncorrectFlag);

        // Act
        cell.IsExplodedMine = false;
        cell.IsIncorrectFlag = true;

        // Assert
        Assert.False(cell.IsExplodedMine);
        Assert.True(cell.IsIncorrectFlag);
    }

    /// <summary>
    /// - (EN) Verifies that updating one cell instance does not affect another separate cell instance.
    /// - (VI) Kiểm tra việc cập nhật một instance cell sẽ không ảnh hưởng đến một instance cell khác.
    /// </summary>
    [Fact]
    public void SeparateCells_ShouldMaintainIndependentState()
    {
        // Arrange
        var firstCell = new Cell(0, 0);
        var secondCell = new Cell(0, 1);

        // Act
        firstCell.IsMine = true;
        firstCell.IsFlagged = true;
        firstCell.AdjacentMines = 2;

        // Assert
        Assert.True(firstCell.IsMine);
        Assert.True(firstCell.IsFlagged);
        Assert.Equal(2, firstCell.AdjacentMines);

        Assert.False(secondCell.IsMine);
        Assert.False(secondCell.IsFlagged);
        Assert.Equal(0, secondCell.AdjacentMines);
    }

    /// <summary>
    /// - (EN) Verifies that a cell can represent a revealed mine after a losing move.
    /// - (VI) Kiểm tra một cell có thể biểu diễn trạng thái mìn đã bị mở sau một nước đi thua.
    /// </summary>
    [Fact]
    public void Cell_ShouldRepresentRevealedMineState()
    {
        // Arrange
        var cell = new Cell(2, 2);

        // Act
        cell.IsMine = true;
        cell.IsRevealed = true;
        cell.IsExplodedMine = true;

        // Assert
        Assert.True(cell.IsMine);
        Assert.True(cell.IsRevealed);
        Assert.True(cell.IsExplodedMine);
        Assert.False(cell.IsIncorrectFlag);
    }

    /// <summary>
    /// - (EN) Verifies that a cell can represent an incorrectly flagged non-mine state after game over.
    /// - (VI) Kiểm tra một cell có thể biểu diễn trạng thái cắm cờ sai trên ô không phải mìn sau khi game kết thúc.
    /// </summary>
    [Fact]
    public void Cell_ShouldRepresentIncorrectFlaggedNonMineState()
    {
        // Arrange
        var cell = new Cell(3, 1);

        // Act
        cell.IsMine = false;
        cell.IsFlagged = true;
        cell.IsRevealed = true;
        cell.IsIncorrectFlag = true;

        // Assert
        Assert.False(cell.IsMine);
        Assert.True(cell.IsFlagged);
        Assert.True(cell.IsRevealed);
        Assert.True(cell.IsIncorrectFlag);
        Assert.False(cell.IsExplodedMine);
    }
}