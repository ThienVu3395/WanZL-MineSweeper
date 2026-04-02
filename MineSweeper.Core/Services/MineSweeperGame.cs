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

        // Mở ô hiện tại (và có thể mở lan nếu AdjacentMines = 0)
        RevealRecursive(row, column);

        // Sau khi mở xong thì kiểm tra điều kiện thắng
        CheckWinCondition();
    }

    /// <summary>
    /// Toggles the flagged state of a cell.
    /// A flagged cell is marked as a potential mine by the player.
    /// - Người chơi có thể đánh dấu (flag) vào ô nghi là mìn và Không cho phép flag vào ô đã được mở
    /// </summary>
    /// <param name="row">The row index of the cell.</param>
    /// <param name="column">The column index of the cell.</param>
    public void ToggleFlag(int row, int column)
    {
        // Chỉ cho phép thao tác khi game đang chơi
        if (Board == null || State != GameState.InProgress)
            return;

        var cell = Board.Cells[row, column];

        // Nếu ô đã được mở thì không được flag
        if (cell.IsRevealed)
            return;

        // Toggle flag
        cell.IsFlagged = !cell.IsFlagged;
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

    /// <summary>
    /// Checks whether all non-mine cells on the board have been revealed.
    /// If all safe cells are revealed, the game state is updated to Won.
    /// </summary>
    /// <remarks>
    /// A MineSweeper game is considered won when every cell that does not contain a mine
    /// has been revealed. This method iterates through all cells to verify that condition.
    /// </remarks>
    private void CheckWinCondition()
    {
        // Duyệt toàn bộ các ô trên board
        foreach (var cell in Board!.Cells)
        {
            // Nếu tồn tại ô KHÔNG phải mìn mà chưa được mở
            // → chưa thỏa điều kiện thắng
            if (!cell.IsMine && !cell.IsRevealed)
            {
                return;
            }
        }

        // Nếu tất cả ô an toàn đã được mở → người chơi thắng
        State = GameState.Won;
    }
}