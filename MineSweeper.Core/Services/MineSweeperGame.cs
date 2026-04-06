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
    /// - (EN) Calculates the number of adjacent mines for each non-mine cell on the board 
    /// in the surrounding 8 neighboring cells around the current cell (top, bottom, left, right, and 4 diagonals)
    /// - (VI) Tính số lượng mìn xung quanh cho mỗi ô không phải là mìn.
    /// </summary>
    /// <param name="board"> The game board instance / Bảng trò chơi </param>
    public void CalculateAdjacentMines(Board board)
    {
        for (int row = 0; row < board.Rows; row++)
        {
            for (int col = 0; col < board.Columns; col++)
            {
                var cell = board.Cells[row, col];

                if (cell.IsMine)
                    continue;

                // - (EN) Use shared neighbor logic
                // - (VI) Sử dụng logic duyệt ô lân cận đã được gom chung
                cell.AdjacentMines = GetNeighborCells(board, row, col).Count(c => c.IsMine);
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

        // Nếu đã flag rồi thì bỏ qua
        if (cell.IsFlagged)
            return;

        // Nếu click trúng mìn thì thua game
        if (cell.IsMine)
        {
            cell.IsRevealed = true;
            RevealAllMines();
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
    /// - (EN) Recursively reveals cells using flood fill logic.
    /// - (VI) Đệ quy mở các ô theo cơ chế flood fill. Nếu ô không có mìn xung quanh (AdjacentMines = 0) 
    /// thì tiếp tục mở lan các ô xung quanh (flood fill)
    /// <param name="row"> (EN) Row index of the cell / (VI) Chỉ số hàng</param>
    /// <param name="column"> (EN) Column index of the cell / (VI) Chỉ số cột</param>
    /// </summary>
    private void RevealRecursive(int row, int column)
    {
        var cell = Board!.Cells[row, column];

        // - (EN) Prevent re-processing or revealing flagged cells
        // - (VI) Tránh xử lý lại hoặc mở các ô đã được flag
        if (cell.IsRevealed)
            return;

        if (cell.IsFlagged)
            return;

        cell.IsRevealed = true;

        // - (EN) Stop recursion if this cell has adjacent mines
        // - (VI) Dừng đệ quy nếu ô này có mìn xung quanh
        if (cell.AdjacentMines > 0)
            return;

        // - (EN) Reveal all neighboring cells
        // - (VI) Mở tất cả các ô lân cận
        foreach (var neighbor in GetNeighborCells(Board, row, column))
        {
            RevealRecursive(neighbor.Row, neighbor.Column);
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

    /// <summary>
    /// Reveals all mine cells on the board after the player loses.
    /// </summary>
    private void RevealAllMines()
    {
        foreach (var cell in Board!.Cells)
        {
            if (cell.IsMine)
            {
                cell.IsRevealed = true;
            }
        }
    }

    /// <summary>
    /// - (EN) Gets all valid neighboring cells around a given position.
    /// - (VI) Lấy tất cả các ô lân cận hợp lệ xung quanh một vị trí.
    /// </summary>
    /// <param name="board">The game board instance / Bảng trò chơi</param>
    /// <param name="row">Row index of the target cell / Chỉ số hàng</param>
    /// <param name="column">Column index of the target cell / Chỉ số cột</param>
    /// <returns>
    /// - (EN) A collection of neighboring cells excluding the current cell.
    /// - (VI) Danh sách các ô lân cận, không bao gồm chính ô hiện tại.
    /// </returns>
    private static IEnumerable<Cell> GetNeighborCells(Board board, int row, int column)
    {
        for (int r = row - 1; r <= row + 1; r++)
        {
            for (int c = column - 1; c <= column + 1; c++)
            {
                // (EN) Skip the current cell
                // (VI) Bỏ qua chính ô hiện tại
                if (r == row && c == column)
                    continue;

                // (EN) Skip out-of-bounds cells
                // (VI) Bỏ qua các ô nằm ngoài phạm vi board
                if (r < 0 || r >= board.Rows || c < 0 || c >= board.Columns)
                    continue;

                yield return board.Cells[r, c];
            }
        }
    }
}