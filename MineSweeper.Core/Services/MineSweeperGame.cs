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
    /// Starts a new MineSweeper game session by creating a new board,
    /// placing mines randomly, and calculating adjacent mine counts
    /// for all cells.
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

        // Tính số mìn xung quanh cho từng ô
        // Đây là logic quan trọng để hiển thị số trong Minesweeper
        CalculateAdjacentMines(Board);

        // Chuyển trạng thái game sang đang chơi
        State = GameState.InProgress;
    }

    /// <summary>
    /// Calculates the number of adjacent mines for each non-mine cell on the board.
    /// This method iterates through all cells and counts the number of mines
    /// in the surrounding 8 neighboring cells around the current cell (top, bottom, left, right, and 4 diagonals)
    /// </summary>
    /// <param name="board">
    /// The board instance whose cells will be updated with adjacent mine counts.
    /// </param>
    public void CalculateAdjacentMines(Board board)
    {
        // Duyệt toàn bộ board
        for (int row = 0; row < board.Rows; row++)
        {
            for (int col = 0; col < board.Columns; col++)
            {
                var cell = board.Cells[row, col];

                // Nếu là mìn thì bỏ qua (không cần tính)
                if (cell.IsMine)
                    continue;

                int mineCount = 0;

                // Duyệt 8 ô xung quanh (trên, dưới, trái, phải và 4 ô chéo)
                for (int r = row - 1; r <= row + 1; r++)
                {
                    for (int c = col - 1; c <= col + 1; c++)
                    {
                        // Bỏ qua chính nó
                        if (r == row && c == col)
                            continue;

                        // Tránh vượt ngoài biên của board
                        if (r < 0 || r >= board.Rows || c < 0 || c >= board.Columns)
                            continue;

                        if (board.Cells[r, c].IsMine)
                            mineCount++;
                    }
                }

                // Gán số mìn xung quanh cho cell
                cell.AdjacentMines = mineCount;
            }
        }
    }

    /// <summary>
    /// Reveals a cell at the specified position.
    /// If the cell is a mine, the game ends.
    /// If the cell has zero adjacent mines, flood fill is triggered.
    /// </summary>
    public void RevealCell(int row, int column)
    {
        if (Board == null || State != GameState.InProgress)
            return;

        var cell = Board.Cells[row, column];

        // Nếu đã mở rồi thì bỏ qua
        if (cell.IsRevealed)
            return;

        // Nếu click trúng mìn thì thua game
        if (cell.IsMine)
        {
            cell.IsRevealed = true;
            State = GameState.Lost;
            return;
        }

        // Nếu không phải mìn thì mở ô
        RevealRecursive(row, column);
    }

    /// <summary>
    /// Recursively reveals cells starting from the given position.
    /// Expands to neighboring cells if no adjacent mines are present.
    /// - Nếu ô không có mìn xung quanh (AdjacentMines = 0) thì tiếp tục mở lan các ô xung quanh (flood fill)
    /// </summary>
    private void RevealRecursive(int row, int column)
    {
        var cell = Board!.Cells[row, column];

        // Nếu đã mở rồi thì dừng
        if (cell.IsRevealed)
            return;

        // Đánh dấu đã mở
        cell.IsRevealed = true;

        // Nếu có số mìn xung quanh → không lan tiếp
        if (cell.AdjacentMines > 0)
            return;

        // Duyệt 8 ô xung quanh
        for (int r = row - 1; r <= row + 1; r++)
        {
            for (int c = column - 1; c <= column + 1; c++)
            {
                if (r == row && c == column)
                    continue;

                if (r < 0 || r >= Board.Rows || c < 0 || c >= Board.Columns)
                    continue;

                RevealRecursive(r, c);
            }
        }
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