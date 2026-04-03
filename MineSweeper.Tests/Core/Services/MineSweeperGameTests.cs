using MineSweeper.Core.Models;
using MineSweeper.Core.Services;

namespace MineSweeper.Tests.Core.Services;

/// <summary>
/// Contains unit tests for verifying the behavior of the MineSweeperGame service.
/// Focuses on game initialization and state transitions for a new session.
/// </summary>
public class MineSweeperGameTests
{
    /// <summary>
    /// Verifies that a new game starts with a board instance.
    /// - Kiểm tra game khi start phải tạo board
    /// </summary>
    [Fact]
    public void StartNewGame_ShouldCreateBoard()
    {
        // Arrange
        var game = new MineSweeperGame();

        // Act
        game.StartNewGame(9, 9, 10);

        // Assert
        Assert.NotNull(game.Board);
    }

    /// <summary>
    /// Verifies that a new game starts with the correct board configuration.
    /// -  Đảm bảo config truyền vào được áp dụng đúng
    /// </summary>
    [Fact]
    public void StartNewGame_ShouldInitializeBoardWithCorrectSettings()
    {
        // Arrange
        var game = new MineSweeperGame();

        // Act
        game.StartNewGame(16, 16, 40);

        // Assert
        Assert.NotNull(game.Board);
        Assert.Equal(16, game.Board!.Rows);
        Assert.Equal(16, game.Board.Columns);
        Assert.Equal(40, game.Board.MineCount);
    }

    /// <summary>
    /// Verifies that starting a new game changes the state to InProgress.
    /// - Khi start game → state phải chuyển sang InProgress
    /// </summary>
    [Fact]
    public void StartNewGame_ShouldSetStateToInProgress()
    {
        // Arrange
        var game = new MineSweeperGame();

        // Act
        game.StartNewGame(9, 9, 10);

        // Assert
        Assert.Equal(GameState.InProgress, game.State);
    }

    /// <summary>
    /// Verifies that starting a new game replaces the previous board instance.
    /// - Start game lần 2 phải replace board cũ (không reuse)
    /// </summary>
    [Fact]
    public void StartNewGame_ShouldReplacePreviousBoard()
    {
        // Arrange
        var game = new MineSweeperGame();

        game.StartNewGame(9, 9, 10);
        var firstBoard = game.Board;

        // Act
        game.StartNewGame(16, 16, 40);

        // Assert
        Assert.NotNull(game.Board);
        Assert.NotSame(firstBoard, game.Board);
        Assert.Equal(16, game.Board.Rows);
        Assert.Equal(16, game.Board.Columns);
        Assert.Equal(40, game.Board.MineCount);
    }

    /// <summary>
    /// Verifies that the exact configured number of mines is placed on the board.
    /// - Kiểm tra số lượng mìn được đặt đúng
    /// </summary>
    [Fact]
    public void StartNewGame_ShouldPlaceExpectedNumberOfMines()
    {
        // Arrange
        var game = new MineSweeperGame();
        int expectedMineCount = 10;

        // Act
        game.StartNewGame(9, 9, expectedMineCount);

        // Assert
        Assert.NotNull(game.Board);

        int actualMineCount = 0;

        foreach (var cell in game.Board!.Cells)
        {
            if (cell.IsMine)
            {
                actualMineCount++;
            }
        }

        Assert.Equal(expectedMineCount, actualMineCount);
    }

    /// <summary>
    /// Verifies that mine placement does not create duplicate mine entries
    /// and all mined cells occupy unique board positions.
    /// - Kiểm tra không có 2 mìn trùng vị trí
    /// </summary>
    [Fact]
    public void StartNewGame_ShouldPlaceMinesInUniqueCells()
    {
        // Arrange
        var game = new MineSweeperGame();

        // Act
        game.StartNewGame(9, 9, 10);

        // Assert
        Assert.NotNull(game.Board);

        var minedPositions = new HashSet<string>();

        foreach (var cell in game.Board!.Cells)
        {
            if (!cell.IsMine)
            {
                continue;
            }

            string positionKey = $"{cell.Row}-{cell.Column}";

            // HashSet.Add sẽ trả false nếu trùng
            bool isUnique = minedPositions.Add(positionKey);

            Assert.True(isUnique);
        }

        Assert.Equal(10, minedPositions.Count);
    }

    /// <summary>
    /// Verifies that adjacent mine counts are calculated correctly
    /// for each non-mine cell after manual mine placement.
    /// </summary>
    [Fact]
    public void StartNewGame_ShouldCalculateAdjacentMinesCorrectly()
    {
        // Arrange
        var game = new MineSweeperGame();

        game.StartNewGame(3, 3, 0);

        var board = game.Board!;

        // Manually place mines (bỏ random để test chính xác)
        // Đặt 2 mìn tại (0,0) và (0,1)
        board.Cells[0, 0].IsMine = true;
        board.Cells[0, 1].IsMine = true;

        // Act - gọi logic tính số mìn xung quanh
        game.CalculateAdjacentMines(board);

        // Assert - kiểm tra các ô xung quanh có giá trị đúng

        // Ô (1,0) nằm dưới 2 mìn → phải là 2
        Assert.Equal(2, board.Cells[1, 0].AdjacentMines);

        // Ô (1,1) nằm gần cả 2 mìn → phải là 2
        Assert.Equal(2, board.Cells[1, 1].AdjacentMines);

        // Ô (0,2) chỉ gần 1 mìn → phải là 1
        Assert.Equal(1, board.Cells[0, 2].AdjacentMines);
    }

    /// <summary>
    /// Verifies that adjacent mine calculation correctly handles
    /// edge and corner cells without causing out-of-bounds errors.
    /// </summary>
    [Fact]
    public void CalculateAdjacentMines_ShouldHandleBoardEdgesCorrectly()
    {
        // Arrange - tạo board 3x3 không có mìn ban đầu
        var game = new MineSweeperGame();
        game.StartNewGame(3, 3, 0);

        var board = game.Board!;

        // Đặt mìn ở góc trên bên trái (0,0) - Đây là case dễ gây lỗi vì ít neighbor nhất
        board.Cells[0, 0].IsMine = true;

        // Act - tính số mìn xung quanh
        game.CalculateAdjacentMines(board);

        // Assert - chỉ các ô hợp lệ xung quanh mới được tính

        // Ô (0,1) nằm bên phải mìn → có 1 mìn
        Assert.Equal(1, board.Cells[0, 1].AdjacentMines);

        // Ô (1,0) nằm dưới mìn → có 1 mìn
        Assert.Equal(1, board.Cells[1, 0].AdjacentMines);

        // Ô (1,1) nằm chéo mìn → có 1 mìn
        Assert.Equal(1, board.Cells[1, 1].AdjacentMines);
    }

    /// <summary>
    /// Verifies that revealing a non-mine cell marks it as revealed.
    /// </summary>
    [Fact]
    public void RevealCell_ShouldRevealNonMineCell()
    {
        // Arrange
        var game = new MineSweeperGame();
        game.StartNewGame(3, 3, 0);

        var board = game.Board!;

        // Act
        game.RevealCell(1, 1);

        // Assert
        Assert.True(board.Cells[1, 1].IsRevealed);
    }

    /// <summary>
    /// Verifies that revealing a mine cell sets the game state to Lost.
    /// </summary>
    [Fact]
    public void RevealCell_ShouldSetStateToLost_WhenMineIsRevealed()
    {
        // Arrange
        var game = new MineSweeperGame();
        game.StartNewGame(3, 3, 0);

        var board = game.Board!;
        board.Cells[1, 1].IsMine = true;

        // Act
        game.RevealCell(1, 1);

        // Assert
        Assert.Equal(GameState.Lost, game.State);
    }

    /// <summary>
    /// Verifies that revealing a cell with zero adjacent mines
    /// triggers recursive reveal (flood fill).
    /// </summary>
    [Fact]
    public void RevealCell_ShouldRevealAdjacentCells_WhenNoAdjacentMines()
    {
        // Arrange
        var game = new MineSweeperGame();
        game.StartNewGame(3, 3, 0);

        var board = game.Board!;

        // Act
        game.RevealCell(1, 1);

        // Assert
        // Tất cả cell đều được mở vì không có mìn
        foreach (var cell in board.Cells)
        {
            Assert.True(cell.IsRevealed);
        }
    }

    /// <summary>
    /// Verifies that the game is marked as won when all non-mine cells are revealed.
    /// </summary>
    [Fact]
    public void RevealCell_ShouldSetStateToWon_WhenAllSafeCellsRevealed()
    {
        // Arrange
        var game = new MineSweeperGame();
        game.StartNewGame(2, 2, 0);

        var board = game.Board!;

        // Đặt 1 mìn
        board.Cells[0, 0].IsMine = true;

        game.CalculateAdjacentMines(board);

        // Act - mở tất cả ô không phải mìn
        game.RevealCell(0, 1);
        game.RevealCell(1, 0);
        game.RevealCell(1, 1);

        // Assert
        Assert.Equal(GameState.Won, game.State);
    }

    /// <summary>
    /// Verifies that a cell can be flagged.
    /// </summary>
    [Fact]
    public void ToggleFlag_ShouldFlagCell()
    {
        // Arrange
        var game = new MineSweeperGame();
        game.StartNewGame(3, 3, 0);

        var board = game.Board!;

        // Act
        game.ToggleFlag(1, 1);

        // Assert
        Assert.True(board.Cells[1, 1].IsFlagged);
    }

    /// <summary>
    /// Verifies that a flagged cell can be unflagged.
    /// </summary>
    [Fact]
    public void ToggleFlag_ShouldUnflagCell()
    {
        // Arrange
        var game = new MineSweeperGame();
        game.StartNewGame(3, 3, 0);

        var board = game.Board!;

        game.ToggleFlag(1, 1);

        // Act
        game.ToggleFlag(1, 1);

        // Assert
        Assert.False(board.Cells[1, 1].IsFlagged);
    }

    /// <summary>
    /// Verifies that a revealed cell cannot be flagged.
    /// </summary>
    [Fact]
    public void ToggleFlag_ShouldNotFlag_WhenCellIsRevealed()
    {
        // Arrange
        var game = new MineSweeperGame();
        game.StartNewGame(3, 3, 0);

        var board = game.Board!;

        game.RevealCell(1, 1);

        // Act
        game.ToggleFlag(1, 1);

        // Assert
        Assert.False(board.Cells[1, 1].IsFlagged);
    }

    [Fact]
    public void RevealCell_ShouldNotRevealFlaggedCells_DuringFloodFill()
    {
        // Arrange
        var game = new MineSweeperGame();
        game.StartNewGame(3, 3, 0);

        var board = game.Board!;

        // Flag một ô ở giữa
        game.ToggleFlag(1, 1);

        // Act - click ô khác để trigger flood fill
        game.RevealCell(0, 0);

        // Assert
        // Ô flagged không được mở
        Assert.False(board.Cells[1, 1].IsRevealed);
        Assert.True(board.Cells[1, 1].IsFlagged);
    }

    [Fact]
    public void RevealCell_ShouldNotRemoveFlag_WhenFloodFillOccurs()
    {
        var game = new MineSweeperGame();
        game.StartNewGame(3, 3, 0);

        var board = game.Board!;

        game.ToggleFlag(1, 1);

        game.RevealCell(0, 0);

        Assert.True(board.Cells[1, 1].IsFlagged);
    }

    [Fact]
    public void RevealCell_ShouldRevealAllMines_WhenPlayerLoses()
    {
        // Arrange
        var game = new MineSweeperGame();
        game.StartNewGame(3, 3, 0);

        var board = game.Board!;

        board.Cells[0, 0].IsMine = true;
        board.Cells[2, 2].IsMine = true;

        game.CalculateAdjacentMines(board);

        // Act
        game.RevealCell(0, 0);

        // Assert
        Assert.Equal(GameState.Lost, game.State);
        Assert.True(board.Cells[0, 0].IsRevealed);
        Assert.True(board.Cells[2, 2].IsRevealed);
    }
}