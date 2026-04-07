using MineSweeper.Core.Models;
using MineSweeper.Core.Services;
using System.Reflection;

namespace MineSweeper.Tests.Core.Services;

/// <summary>
/// - (EN) Contains unit tests for verifying the behavior of the <see cref="MineSweeperGame"/> service.
/// Focuses on game initialization, mine placement, reveal logic, flag handling,
/// chording behavior, first-click safety, and game state transitions.
/// - (VI) Chứa các unit test dùng để kiểm tra hành vi của service <see cref="MineSweeperGame"/>.
/// Tập trung vào khởi tạo game, đặt mìn, logic mở ô, xử lý cờ,
/// hành vi chording, an toàn ở lần click đầu tiên, và các chuyển đổi trạng thái game.
/// </summary>
public class MineSweeperGameTests
{
    #region StartNewGame
    /// <summary>
    /// - (EN) Verifies that starting a new game creates a board instance.
    /// - (VI) Kiểm tra khi bắt đầu game mới thì một board sẽ được tạo ra.
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
    /// - (EN) Verifies that a new game starts with the correct board configuration.
    /// - (VI) Kiểm tra cấu hình board được khởi tạo đúng theo tham số truyền vào.
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
    /// - (EN) Verifies that starting a new game changes the state to <see cref="GameState.InProgress"/>.
    /// - (VI) Kiểm tra khi bắt đầu game mới thì trạng thái sẽ chuyển sang <see cref="GameState.InProgress"/>.
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
    /// - (EN) Verifies that starting a new game replaces the previous board instance.
    /// - (VI) Kiểm tra khi bắt đầu game mới lần tiếp theo thì board cũ sẽ được thay thế, không bị tái sử dụng.
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
    /// - (EN) Verifies that the exact configured number of mines is placed
    /// after deferred mine placement is triggered by the first reveal.
    /// - (VI) Kiểm tra đúng số lượng mìn được đặt
    /// sau khi cơ chế đặt mìn trì hoãn được kích hoạt ở lần mở ô đầu tiên.
    /// </summary>
    [Fact]
    public void RevealCell_ShouldPlaceExpectedNumberOfMines_OnFirstReveal()
    {
        // Arrange
        var game = new MineSweeperGame();
        int expectedMineCount = 10;

        game.StartNewGame(9, 9, expectedMineCount);

        // Act
        game.RevealCell(0, 0);

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
    /// - (EN) Verifies that deferred mine placement does not create duplicate mine entries
    /// and all mined cells occupy unique board positions after the first reveal.
    /// - (VI) Kiểm tra cơ chế đặt mìn trì hoãn không tạo mìn trùng vị trí
    /// và tất cả ô chứa mìn đều có tọa độ duy nhất sau lần mở ô đầu tiên.
    /// </summary>
    [Fact]
    public void RevealCell_ShouldPlaceMinesInUniqueCells_OnFirstReveal()
    {
        // Arrange
        var game = new MineSweeperGame();

        game.StartNewGame(9, 9, 10);

        // Act
        game.RevealCell(0, 0);

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

            bool isUnique = minedPositions.Add(positionKey);

            Assert.True(isUnique);
        }

        Assert.Equal(10, minedPositions.Count);
    }

    /// <summary>
    /// - (EN) Verifies that adjacent mine counts are calculated correctly
    /// for each non-mine cell after manual mine placement.
    /// - (VI) Kiểm tra số mìn lân cận được tính đúng
    /// cho từng ô không phải mìn sau khi đặt mìn thủ công.
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
    #endregion

    #region BoardConfigurationValidation

    /// <summary>
    /// - (EN) Verifies that starting a new game throws an exception
    /// when the number of rows is less than or equal to zero.
    /// - (VI) Kiểm tra bắt đầu game mới sẽ ném ra exception
    /// khi số hàng nhỏ hơn hoặc bằng 0.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void StartNewGame_ShouldThrow_WhenRowsIsLessThanOrEqualToZero(int invalidRows)
    {
        // Arrange
        var game = new MineSweeperGame();

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            game.StartNewGame(invalidRows, 9, 10));

        Assert.Equal("rows", exception.ParamName);
    }

    /// <summary>
    /// - (EN) Verifies that starting a new game throws an exception
    /// when the number of columns is less than or equal to zero.
    /// - (VI) Kiểm tra bắt đầu game mới sẽ ném ra exception
    /// khi số cột nhỏ hơn hoặc bằng 0.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void StartNewGame_ShouldThrow_WhenColumnsIsLessThanOrEqualToZero(int invalidColumns)
    {
        // Arrange
        var game = new MineSweeperGame();

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            game.StartNewGame(9, invalidColumns, 10));

        Assert.Equal("columns", exception.ParamName);
    }

    /// <summary>
    /// - (EN) Verifies that starting a new game throws an exception
    /// when mine count is negative.
    /// - (VI) Kiểm tra bắt đầu game mới sẽ ném ra exception
    /// khi số lượng mìn là số âm.
    /// </summary>
    [Fact]
    public void StartNewGame_ShouldThrow_WhenMineCountIsNegative()
    {
        // Arrange
        var game = new MineSweeperGame();

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            game.StartNewGame(9, 9, -1));

        Assert.Equal("mineCount", exception.ParamName);
    }

    /// <summary>
    /// - (EN) Verifies that starting a new game throws an exception
    /// when mine count is greater than or equal to total cells,
    /// which would make a safe first reveal impossible.
    /// - (VI) Kiểm tra bắt đầu game mới sẽ ném ra exception
    /// khi số lượng mìn lớn hơn hoặc bằng tổng số ô,
    /// vì điều đó khiến việc đảm bảo click đầu tiên an toàn là không thể.
    /// </summary>
    [Theory]
    [InlineData(1, 1, 1)]
    [InlineData(2, 2, 4)]
    [InlineData(3, 3, 9)]
    public void StartNewGame_ShouldThrow_WhenMineCountIsGreaterThanOrEqualToTotalCells(
        int rows,
        int columns,
        int mineCount)
    {
        // Arrange
        var game = new MineSweeperGame();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            game.StartNewGame(rows, columns, mineCount));

        Assert.Equal("mineCount", exception.ParamName);
    }

    /// <summary>
    /// - (EN) Verifies that starting a new game does not throw
    /// when the board configuration is valid.
    /// - (VI) Kiểm tra bắt đầu game mới sẽ không ném ra exception
    /// khi cấu hình board hợp lệ.
    /// </summary>
    [Theory]
    [InlineData(1, 2, 1)]
    [InlineData(2, 2, 3)]
    [InlineData(9, 9, 10)]
    [InlineData(16, 16, 40)]
    public void StartNewGame_ShouldNotThrow_WhenBoardConfigurationIsValid(
        int rows,
        int columns,
        int mineCount)
    {
        // Arrange
        var game = new MineSweeperGame();

        // Act
        var exception = Record.Exception(() =>
            game.StartNewGame(rows, columns, mineCount));

        // Assert
        Assert.Null(exception);
    }

    #endregion

    #region CalculateAdjacentMines
    /// <summary>
    /// - (EN) Verifies that adjacent mine calculation correctly handles
    /// edge and corner cells without causing out-of-bounds errors.
    /// - (VI) Kiểm tra việc tính số mìn lân cận xử lý đúng
    /// các ô ở cạnh và góc mà không gây lỗi vượt phạm vi mảng.
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
    #endregion

    #region RevealCell
    /// <summary>
    /// - (EN) Verifies that revealing a non-mine cell marks it as revealed.
    /// - (VI) Kiểm tra khi mở một ô không phải mìn thì ô đó sẽ được đánh dấu là đã mở.
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
    /// - (EN) Verifies that revealing a mine cell sets the game state to <see cref="GameState.Lost"/>.
    /// - (VI) Kiểm tra khi mở một ô chứa mìn thì trạng thái game sẽ chuyển sang <see cref="GameState.Lost"/>.
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
    /// - (EN) Verifies that revealing a cell with zero adjacent mines
    /// triggers recursive reveal behavior (flood fill).
    /// - (VI) Kiểm tra khi mở một ô có số mìn lân cận bằng 0
    /// thì cơ chế mở lan (flood fill) sẽ được kích hoạt.
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
    /// - (EN) Verifies that the game is marked as won when all non-mine cells are revealed.
    /// - (VI) Kiểm tra game được đánh dấu là thắng khi tất cả ô an toàn đã được mở.
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
    /// - (EN) Verifies that flagged cells are not revealed during flood fill.
    /// - (VI) Kiểm tra các ô đã được cắm cờ sẽ không bị mở trong quá trình flood fill.
    /// </summary>
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

    /// <summary>
    /// - (EN) Verifies that flood fill does not remove an existing flag from a cell.
    /// - (VI) Kiểm tra flood fill không làm mất cờ đã được đặt trên một ô.
    /// </summary>
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

    /// <summary>
    /// - (EN) Verifies that all mine cells are revealed when the player loses the game.
    /// - (VI) Kiểm tra tất cả ô chứa mìn sẽ được mở ra khi người chơi thua game.
    /// </summary>
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

    /// <summary>
    /// - (EN) Verifies that the clicked mine is marked as the exploded mine when the player loses.
    /// - (VI) Kiểm tra ô mìn bị click sẽ được đánh dấu là ô phát nổ khi người chơi thua.
    /// </summary>
    [Fact]
    public void RevealCell_ShouldMarkExplodedMine_WhenPlayerHitsMine()
    {
        // Arrange
        var game = new MineSweeperGame();
        game.StartNewGame(2, 2, 1);

        var board = game.Board!;

        foreach (var cell in board.Cells)
        {
            cell.IsMine = false;
            cell.IsRevealed = false;
            cell.IsFlagged = false;
            cell.AdjacentMines = 0;
            cell.IsExplodedMine = false;
            cell.IsIncorrectFlag = false;
        }

        board.Cells[0, 0].IsMine = true;
        game.CalculateAdjacentMines(board);

        var firstRevealPendingField = typeof(MineSweeperGame)
            .GetField("_isFirstRevealPending", BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.NotNull(firstRevealPendingField);
        firstRevealPendingField!.SetValue(game, false);

        // Act
        game.RevealCell(0, 0);

        // Assert
        Assert.Equal(GameState.Lost, game.State);
        Assert.True(board.Cells[0, 0].IsExplodedMine);
        Assert.True(board.Cells[0, 0].IsRevealed);
    }

    /// <summary>
    /// - (EN) Verifies that flagged non-mine cells are marked as incorrect after the player loses.
    /// - (VI) Kiểm tra các ô bị cắm cờ nhưng không phải mìn sẽ bị đánh dấu là cờ sai sau khi người chơi thua.
    /// </summary>
    [Fact]
    public void RevealCell_ShouldMarkIncorrectFlags_WhenPlayerLoses()
    {
        // Arrange
        var game = new MineSweeperGame();
        game.StartNewGame(2, 2, 1);

        var board = game.Board!;

        foreach (var cell in board.Cells)
        {
            cell.IsMine = false;
            cell.IsRevealed = false;
            cell.IsFlagged = false;
            cell.AdjacentMines = 0;
            cell.IsExplodedMine = false;
            cell.IsIncorrectFlag = false;
        }

        board.Cells[0, 0].IsMine = true;
        board.Cells[1, 1].IsFlagged = true; // wrong flag
        game.CalculateAdjacentMines(board);

        var firstRevealPendingField = typeof(MineSweeperGame)
            .GetField("_isFirstRevealPending", BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.NotNull(firstRevealPendingField);
        firstRevealPendingField!.SetValue(game, false);

        // Act
        game.RevealCell(0, 0);

        // Assert
        Assert.Equal(GameState.Lost, game.State);
        Assert.True(board.Cells[1, 1].IsIncorrectFlag);
        Assert.True(board.Cells[1, 1].IsRevealed);
        Assert.False(board.Cells[1, 1].IsMine);
    }
    #endregion

    #region ToggleFlag
    /// <summary>
    /// - (EN) Verifies that a cell can be flagged.
    /// - (VI) Kiểm tra một ô có thể được cắm cờ.
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
    /// - (EN) Verifies that a flagged cell can be unflagged.
    /// - (VI) Kiểm tra một ô đã được cắm cờ có thể được bỏ cờ.
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
    /// - (EN) Verifies that a revealed cell cannot be flagged.
    /// - (VI) Kiểm tra một ô đã mở thì không thể được cắm cờ.
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
    #endregion

    #region Chord
    /// <summary>
    /// - (EN) Verifies that chording does nothing when the target cell has not been revealed.
    /// - (VI) Kiểm tra chording không có tác dụng nếu ô mục tiêu chưa được mở.
    /// </summary>
    [Fact]
    public void ChordCell_ShouldDoNothing_WhenTargetCellIsNotRevealed()
    {
        // Arrange
        var game = new MineSweeperGame();
        game.StartNewGame(3, 3, 0);

        var board = game.Board!;

        board.Cells[0, 0].IsMine = true;
        game.CalculateAdjacentMines(board);

        // Act
        game.ChordCell(1, 1);

        // Assert
        Assert.False(board.Cells[1, 0].IsRevealed);
        Assert.False(board.Cells[1, 1].IsRevealed);
        Assert.Equal(GameState.InProgress, game.State);
    }

    /// <summary>
    /// - (EN) Verifies that chording does nothing when the number of adjacent flags
    /// does not match the revealed cell's adjacent mine count.
    /// - (VI) Kiểm tra chording không hoạt động khi số cờ xung quanh chưa khớp với số mìn lân cận.
    /// </summary>
    [Fact]
    public void ChordCell_ShouldDoNothing_WhenAdjacentFlagCountDoesNotMatch()
    {
        // Arrange
        var game = new MineSweeperGame();
        game.StartNewGame(3, 3, 0);

        var board = game.Board!;

        board.Cells[0, 0].IsMine = true;
        board.Cells[0, 1].IsMine = true;
        game.CalculateAdjacentMines(board);

        game.RevealCell(1, 1); // AdjacentMines = 2
        game.ToggleFlag(0, 0); // only 1 flag

        // Act
        game.ChordCell(1, 1);

        // Assert
        Assert.False(board.Cells[1, 0].IsRevealed);
        Assert.False(board.Cells[1, 2].IsRevealed);
        Assert.False(board.Cells[2, 0].IsRevealed);
        Assert.Equal(GameState.InProgress, game.State);
    }

    /// <summary>
    /// - (EN) Verifies that chording reveals all hidden and unflagged neighboring cells
    /// when the adjacent flag count matches the revealed number cell.
    /// - (VI) Kiểm tra chording sẽ mở tất cả ô lân cận còn ẩn và không bị flag
    /// khi số cờ xung quanh khớp với ô số đã mở.
    /// </summary>
    [Fact]
    public void ChordCell_ShouldRevealHiddenUnflaggedNeighbors_WhenFlagCountMatches()
    {
        // Arrange
        var game = new MineSweeperGame();
        game.StartNewGame(3, 3, 0);

        var board = game.Board!;

        board.Cells[0, 0].IsMine = true;
        game.CalculateAdjacentMines(board);

        game.RevealCell(1, 1);   // AdjacentMines = 1
        game.ToggleFlag(0, 0);   // correct flag

        // Act
        game.ChordCell(1, 1);

        // Assert
        Assert.True(board.Cells[0, 1].IsRevealed);
        Assert.True(board.Cells[0, 2].IsRevealed);
        Assert.True(board.Cells[1, 0].IsRevealed);
        Assert.True(board.Cells[1, 2].IsRevealed);
        Assert.True(board.Cells[2, 0].IsRevealed);
        Assert.True(board.Cells[2, 1].IsRevealed);
        Assert.True(board.Cells[2, 2].IsRevealed);

        Assert.True(board.Cells[0, 0].IsFlagged);
        Assert.False(board.Cells[0, 0].IsRevealed);
    }

    /// <summary>
    /// - (EN) Verifies that chording can trigger a loss when the placed flags are incorrect
    /// and an unflagged mine is revealed as part of the chord action.
    /// - (VI) Kiểm tra chording có thể làm thua game nếu người chơi cắm cờ sai
    /// và một ô mìn chưa được flag bị mở ra.
    /// </summary>
    [Fact]
    public void ChordCell_ShouldSetStateToLost_WhenFlagsAreIncorrectAndMineGetsRevealed()
    {
        // Arrange
        var game = new MineSweeperGame();
        game.StartNewGame(3, 3, 0);

        var board = game.Board!;

        board.Cells[0, 0].IsMine = true;
        board.Cells[0, 1].IsMine = true;
        game.CalculateAdjacentMines(board);

        game.RevealCell(1, 1);   // AdjacentMines = 2
        game.ToggleFlag(0, 0);   // correct
        game.ToggleFlag(2, 2);   // wrong

        // Act
        game.ChordCell(1, 1);

        // Assert
        Assert.Equal(GameState.Lost, game.State);
        Assert.True(board.Cells[0, 1].IsRevealed);
    }

    /// <summary>
    /// - (EN) Verifies that chording can complete the game and set the state to won
    /// when all remaining safe cells are revealed.
    /// - (VI) Kiểm tra chording có thể giúp thắng game khi mở hết các ô an toàn còn lại.
    /// </summary>
    [Fact]
    public void ChordCell_ShouldSetStateToWon_WhenAllSafeCellsAreRevealed()
    {
        // Arrange
        var game = new MineSweeperGame();
        game.StartNewGame(2, 2, 0);

        var board = game.Board!;

        board.Cells[0, 0].IsMine = true;
        game.CalculateAdjacentMines(board);

        game.RevealCell(0, 1);
        game.ToggleFlag(0, 0);

        // Act
        game.ChordCell(0, 1);

        // Assert
        Assert.Equal(GameState.Won, game.State);
        Assert.True(board.Cells[1, 0].IsRevealed);
        Assert.True(board.Cells[1, 1].IsRevealed);
    }
    #endregion

    #region FirstClickSafety
    /// <summary>
    /// - (EN) Verifies that the first reveal never causes an immediate loss.
    /// - (VI) Kiểm tra lần mở ô đầu tiên sẽ không bao giờ làm thua ngay lập tức.
    /// </summary>
    [Fact]
    public void RevealCell_ShouldNotLoseGame_OnFirstReveal()
    {
        // Arrange
        var game = new MineSweeperGame();
        game.StartNewGame(9, 9, 10);

        // Act
        game.RevealCell(0, 0);

        // Assert
        Assert.NotEqual(GameState.Lost, game.State);
    }

    /// <summary>
    /// - (EN) Verifies that the first revealed cell is guaranteed not to contain a mine.
    /// - (VI) Kiểm tra ô được mở đầu tiên chắc chắn không chứa mìn.
    /// </summary>
    [Fact]
    public void RevealCell_ShouldEnsureFirstRevealedCellIsNotMine()
    {
        // Arrange
        var game = new MineSweeperGame();
        game.StartNewGame(9, 9, 10);

        // Act
        game.RevealCell(2, 3);

        // Assert
        Assert.NotNull(game.Board);
        Assert.False(game.Board!.Cells[2, 3].IsMine);
        Assert.True(game.Board.Cells[2, 3].IsRevealed);
    }

    /// <summary>
    /// - (EN) Verifies that the configured mine count is preserved
    /// after mines are placed during the first reveal.
    /// - (VI) Kiểm tra tổng số mìn được cấu hình vẫn được giữ nguyên
    /// sau khi mìn được đặt ở lần mở ô đầu tiên.
    /// </summary>
    [Fact]
    public void RevealCell_ShouldPreserveConfiguredMineCount_AfterFirstRevealSetup()
    {
        // Arrange
        var game = new MineSweeperGame();
        int expectedMineCount = 10;

        game.StartNewGame(9, 9, expectedMineCount);

        // Act
        game.RevealCell(4, 4);

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
    /// - (EN) Verifies that the first safe reveal does not change normal losing behavior for later moves.
    /// - (VI) Kiểm tra lần mở ô đầu tiên an toàn không làm thay đổi cơ chế thua bình thường ở các lượt sau.
    /// </summary>
    [Fact]
    public void RevealCell_ShouldAllowNormalLoss_AfterFirstSafeReveal()
    {
        // Arrange
        var game = new MineSweeperGame();
        game.StartNewGame(3, 3, 7);

        // First reveal must be safe
        game.RevealCell(0, 0);

        var board = game.Board!;
        var firstCell = board.Cells[0, 0];

        Assert.False(firstCell.IsMine);

        // Find a non-revealed mine cell after first-click placement
        Cell? mineCell = null;

        foreach (var cell in board.Cells)
        {
            if (cell.IsMine && !cell.IsRevealed)
            {
                mineCell = cell;
                break;
            }
        }

        Assert.NotNull(mineCell);

        // Act
        game.RevealCell(mineCell!.Row, mineCell.Column);

        // Assert
        Assert.Equal(GameState.Lost, game.State);
    }

    /// <summary>
    /// - (EN) Verifies that adjacent mine counts are generated correctly
    /// after deferred mine placement on the first reveal.
    /// - (VI) Kiểm tra số mìn lân cận được tính đúng
    /// sau khi đặt mìn trì hoãn ở lần mở ô đầu tiên.
    /// </summary>
    [Fact]
    public void RevealCell_ShouldCalculateAdjacentMines_AfterDeferredMinePlacement()
    {
        // Arrange
        var game = new MineSweeperGame();
        game.StartNewGame(5, 5, 5);

        // Act
        game.RevealCell(2, 2);

        // Assert
        var board = game.Board!;
        foreach (var cell in board.Cells)
        {
            if (cell.IsMine)
                continue;

            int actualAdjacentMines = 0;

            for (int row = cell.Row - 1; row <= cell.Row + 1; row++)
            {
                for (int column = cell.Column - 1; column <= cell.Column + 1; column++)
                {
                    if (row == cell.Row && column == cell.Column)
                        continue;

                    if (row < 0 || row >= board.Rows || column < 0 || column >= board.Columns)
                        continue;

                    if (board.Cells[row, column].IsMine)
                        actualAdjacentMines++;
                }
            }

            Assert.Equal(actualAdjacentMines, cell.AdjacentMines);
        }
    }
    #endregion
}