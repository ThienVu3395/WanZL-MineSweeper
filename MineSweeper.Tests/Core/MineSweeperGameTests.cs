using MineSweeper.Core.Models;
using MineSweeper.Core.Services;

namespace MineSweeper.Tests.Core;

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
}