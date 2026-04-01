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
        Assert.Equal(16, game.Board.Rows);
        Assert.Equal(16, game.Board.Columns);
        Assert.Equal(40, game.Board.MineCount);
    }

    /// <summary>
    /// Verifies that starting a new game changes the state to InProgress.
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
}