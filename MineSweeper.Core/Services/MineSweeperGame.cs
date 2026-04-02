using MineSweeper.Core.Models;

namespace MineSweeper.Core.Services;

/// <summary>
/// Represents the main game service responsible for managing
/// the lifecycle and state of a MineSweeper session.
/// </summary>
public class MineSweeperGame
{
    // Random dùng để generate vị trí mìn (đây là engine sinh số ngẫu nhiên)
    // Dùng cách này để tạo 1 lần nhưng Reuse nhiều lần, Random ổn định hơn
    private readonly Random _random = new();

    /// <summary>
    /// Gets the current board instance for the active game.
    /// </summary>
    public Board? Board { get; private set; }

    /// <summary>
    /// Gets the current state of the game session.
    /// </summary>
    public GameState State { get; private set; } = GameState.NotStarted;

    /// <summary>
    /// Starts a new game with the specified board dimensions and mine count.
    /// </summary>
    /// <param name="rows">The number of rows for the new board.</param>
    /// <param name="columns">The number of columns for the new board.</param>
    /// <param name="mineCount">The number of mines configured for the new board.</param>
    public void StartNewGame(int rows, int columns, int mineCount)
    {
        // Khởi tạo board mới với cấu hình truyền vào
        Board = new Board(rows, columns, mineCount);

        // Đặt mìn ngẫu nhiên lên board
        PlaceMines(Board);

        // Chuyển trạng thái game sang đang chơi
        State = GameState.InProgress;
    }

    /// <summary>
    /// Places mines randomly on the board without duplicating positions.
    /// </summary>
    /// <param name="board">The board that will receive mine placement.</param>
    private void PlaceMines(Board board)
    {
        int placedMines = 0;

        // Lặp đến khi đặt đủ số mìn
        while (placedMines < board.MineCount)
        {
            // Random vị trí (trả về số nguyên có giá trị 0 <= số < max)
            int row = _random.Next(board.Rows);
            int column = _random.Next(board.Columns);

            var cell = board.Cells[row, column];

            // Nếu ô đã có mìn → bỏ qua (tránh trùng)
            if (cell.IsMine)
            {
                continue;
            }

            // Đặt mìn vào ô
            cell.IsMine = true;
            placedMines++;
        }
    }
}