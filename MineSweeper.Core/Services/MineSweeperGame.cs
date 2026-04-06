using MineSweeper.Core.Models;

namespace MineSweeper.Core.Services;

/// <summary>
/// - (EN) Represents the main game service responsible for managing
/// the lifecycle and state of a MineSweeper session.
/// - (VI) Đại diện cho service game chính chịu trách nhiệm quản lý
/// vòng đời và trạng thái của một phiên chơi MineSweeper.
/// </summary>
public class MineSweeperGame
{
    // Random dùng để generate vị trí mìn (đây là engine sinh số ngẫu nhiên)
    // Dùng cách này để tạo 1 lần nhưng Reuse nhiều lần, Random ổn định hơn
    private readonly Random _random = new();

    /// <summary>
    /// - (EN) Gets the current board instance for the active game.
    /// - (VI) Lấy board hiện tại của phiên chơi đang hoạt động.
    /// </summary>
    public Board? Board { get; private set; }

    /// <summary>
    /// - (EN) Gets the current state of the game session.
    /// - (VI) Lấy trạng thái hiện tại của phiên chơi.
    /// </summary>
    public GameState State { get; private set; } = GameState.NotStarted;

    /// <summary>
    /// - (EN) Tracks whether the first reveal has not yet been processed.
    /// - (VI) Theo dõi xem lần mở ô đầu tiên đã được xử lý hay chưa.
    /// </summary>
    private bool _isFirstRevealPending;

    /// <summary>
    /// - (EN) Starts a new MineSweeper game session by creating a new board.
    /// Mine placement is deferred until the first reveal so the first clicked cell is always safe.
    /// - (VI) Bắt đầu một phiên chơi MineSweeper mới bằng cách tạo board mới.
    /// Việc đặt mìn sẽ được hoãn tới lần mở ô đầu tiên để đảm bảo ô đầu tiên luôn an toàn.
    /// </summary>
    /// <param name="rows">- (EN) The number of rows for the new board. / (VI) Số hàng của board mới.</param>
    /// <param name="columns">- (EN) The number of columns for the new board. / (VI) Số cột của board mới.</param>
    /// <param name="mineCount">- (EN) The number of mines configured for the new board. / (VI) Số lượng mìn được cấu hình cho board mới.</param>
    public void StartNewGame(int rows, int columns, int mineCount)
    {
        ValidateBoardConfiguration(rows, columns, mineCount);

        // Khởi tạo board mới với cấu hình truyền vào
        Board = new Board(rows, columns, mineCount);

        // Chưa đặt mìn ngay. Sẽ đặt ở lần reveal đầu tiên
        // để đảm bảo ô đầu tiên luôn an toàn.
        _isFirstRevealPending = true;

        // Chuyển trạng thái game sang đang chơi
        State = GameState.InProgress;
    }

    /// <summary>
    /// - (EN) Calculates the number of adjacent mines for each non-mine cell
    /// by scanning the 8 neighboring cells around it.
    /// - (VI) Tính số lượng mìn lân cận cho mỗi ô không phải mìn
    /// bằng cách duyệt 8 ô xung quanh.
    /// </summary>
    /// <param name="board">- (EN) The game board instance. / (VI) Board của trò chơi.</param>
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
    /// - (EN) Reveals a cell at the specified position.
    /// If the revealed cell is a mine, the game ends.
    /// If the cell has zero adjacent mines, flood fill is triggered.
    /// The first revealed cell is always guaranteed to be safe.
    /// - (VI) Mở một ô tại vị trí được chỉ định.
    /// Nếu ô được mở là mìn thì game kết thúc.
    /// Nếu ô không có mìn xung quanh thì sẽ kích hoạt flood fill.
    /// Ô đầu tiên được mở luôn được đảm bảo là an toàn.
    /// </summary>
    /// <param name="row">- (EN) The row index of the cell. / (VI) Chỉ số hàng của ô.</param>
    /// <param name="column">- (EN) The column index of the cell. / (VI) Chỉ số cột của ô.</param>
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

        // Nếu đây là lần mở đầu tiên thì mới đặt mìn
        // và đảm bảo không đặt mìn tại ô đang được click.
        if (_isFirstRevealPending)
        {
            PlaceMines(Board, row, column);
            CalculateAdjacentMines(Board);
            _isFirstRevealPending = false;

            // Refresh lại cell sau khi board đã có mìn và số lân cận
            cell = Board.Cells[row, column];
        }

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
    /// - (EN) Toggles the flagged state of a cell.
    /// A flagged cell is marked by the player as a potential mine.
    /// Revealed cells cannot be flagged.
    /// - (VI) Bật hoặc tắt trạng thái cắm cờ của một ô.
    /// Người chơi có thể đánh dấu ô nghi là mìn bằng cờ.
    /// Các ô đã được mở thì không thể cắm cờ.
    /// </summary>
    /// <param name="row">- (EN) The row index of the cell. / (VI) Chỉ số hàng của ô.</param>
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
    /// - (EN) Performs the chording action on a revealed numbered cell.
    /// If the number of flagged neighboring cells matches the adjacent mine count,
    /// all remaining hidden and unflagged neighboring cells will be revealed.
    /// This action may result in a win or loss depending on the correctness of placed flags.
    /// - (VI) Thực hiện thao tác chording trên một ô số đã được mở.
    /// Nếu số lượng cờ xung quanh bằng với số mìn lân cận,
    /// tất cả các ô lân cận còn ẩn và không bị cắm cờ sẽ được mở.
    /// Hành động này có thể dẫn đến thắng hoặc thua tùy theo độ chính xác của các cờ đã đặt.
    /// </summary>
    /// <param name="row">- (EN) The row index of the target cell. / (VI) Chỉ số hàng của ô mục tiêu.</param>
    /// <param name="column">- (EN) The column index of the target cell. / (VI) Chỉ số cột của ô mục tiêu.</param>
    public void ChordCell(int row, int column)
    {
        // - (EN) Only allow action during active game
        // - (VI) Chỉ cho phép khi game đang chạy
        if (Board == null || State != GameState.InProgress)
            return;

        var cell = Board.Cells[row, column];

        // - (EN) Chording only works on revealed cells
        // - (VI) Chỉ áp dụng cho ô đã được mở
        if (!cell.IsRevealed)
            return;

        // - (EN) Do not chord on mine cells
        // - (VI) Không thực hiện trên ô mìn
        if (cell.IsMine)
            return;

        var neighbors = GetNeighborCells(Board, row, column).ToList();

        // - (EN) Count flagged neighbors
        // - (VI) Đếm số lượng ô đã được flag xung quanh
        int flaggedCount = neighbors.Count(c => c.IsFlagged);

        // - (EN) Only proceed if flag count matches expected adjacent mines
        // - (VI) Chỉ thực hiện nếu số flag khớp với số mìn lân cận
        if (flaggedCount != cell.AdjacentMines)
            return;

        // - (EN) Reveal all hidden & unflagged neighbors
        // - (VI) Mở tất cả ô lân cận chưa mở và không bị flag
        foreach (var neighbor in neighbors)
        {
            if (neighbor.IsRevealed || neighbor.IsFlagged)
                continue;

            // - (EN) Reuse existing reveal logic to ensure consistency
            // - (VI) Tái sử dụng logic reveal để đảm bảo đồng nhất
            RevealCell(neighbor.Row, neighbor.Column);

            // - (EN) Stop early if game already lost
            // - (VI) Dừng ngay nếu đã thua game
            if (State == GameState.Lost)
                return;
        }

        // - (EN) Check win condition after chord reveal
        // - (VI) Kiểm tra điều kiện thắng sau khi chord
        CheckWinCondition();
    }

    #region Private Helpers
    /// <summary>
    /// - (EN) Recursively reveals cells using flood fill logic.
    /// If a cell has zero adjacent mines, its neighboring cells are revealed recursively.
    /// - (VI) Đệ quy mở các ô theo cơ chế flood fill.
    /// Nếu ô không có mìn lân cận thì tiếp tục mở lan các ô xung quanh.
    /// </summary>
    /// <param name="row">- (EN) The row index of the cell. / (VI) Chỉ số hàng của ô.</param>
    /// <param name="column">- (EN) The column index of the cell. / (VI) Chỉ số cột của ô.</param>
    private void RevealRecursive(int row, int column)
    {
        var cell = Board!.Cells[row, column];

        if (cell.IsRevealed)
            return;

        if (cell.IsFlagged)
            return;

        cell.IsRevealed = true;

        if (cell.AdjacentMines > 0)
            return;

        foreach (var neighbor in GetNeighborCells(Board, row, column))
        {
            RevealRecursive(neighbor.Row, neighbor.Column);
        }
    }

    /// <summary>
    /// - (EN) Places mines randomly on the board without duplicating positions,
    /// while keeping the specified safe cell free of mines.
    /// - (VI) Đặt mìn ngẫu nhiên lên board mà không trùng vị trí,
    /// đồng thời đảm bảo ô an toàn được chỉ định sẽ không chứa mìn.
    /// </summary>
    /// <param name="board">- (EN) The board that will receive mine placement. / (VI) Board sẽ được đặt mìn.</param>
    /// <param name="safeRow">- (EN) The row of the guaranteed safe cell. / (VI) Hàng của ô được đảm bảo an toàn.</param>
    /// <param name="safeColumn">- (EN) The column of the guaranteed safe cell. / (VI) Cột của ô được đảm bảo an toàn.</param>
    private void PlaceMines(Board board, int safeRow, int safeColumn)
    {
        int placedMines = 0;

        // Lặp đến khi đặt đủ số mìn
        while (placedMines < board.MineCount)
        {
            // Random vị trí (trả về số nguyên có giá trị 0 <= số < max)
            int row = _random.Next(board.Rows);
            int column = _random.Next(board.Columns);

            // Không được đặt mìn vào ô click đầu tiên
            if (row == safeRow && column == safeColumn)
                continue;

            var cell = board.Cells[row, column];

            // Nếu ô đã có mìn → bỏ qua (tránh trùng)
            if (cell.IsMine)
                continue;

            // Đặt mìn vào ô
            cell.IsMine = true;
            placedMines++;
        }
    }

    /// <summary>
    /// - (EN) Checks whether all non-mine cells on the board have been revealed.
    /// If all safe cells are revealed, the game state is updated to <see cref="GameState.Won"/>.
    /// - (VI) Kiểm tra xem tất cả các ô không phải mìn trên board đã được mở hay chưa.
    /// Nếu mọi ô an toàn đã được mở, trạng thái game sẽ được cập nhật thành <see cref="GameState.Won"/>.
    /// </summary>
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
    /// - (EN) Reveals all mine cells on the board after the player loses.
    /// - (VI) Mở toàn bộ các ô chứa mìn trên board sau khi người chơi thua.
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
    /// - (EN) Validates the board configuration before starting a new game.
    /// Ensures that dimensions are positive, mine count is not negative,
    /// and at least one safe cell remains available for the guaranteed first reveal.
    /// - (VI) Kiểm tra cấu hình board trước khi bắt đầu một ván mới.
    /// Đảm bảo kích thước board hợp lệ, số lượng mìn không âm,
    /// và luôn còn ít nhất một ô an toàn cho lần mở đầu tiên được đảm bảo.
    /// </summary>
    /// <param name="rows">- (EN) Number of board rows. / (VI) Số hàng của board.</param>
    /// <param name="columns">- (EN) Number of board columns. / (VI) Số cột của board.</param>
    /// <param name="mineCount">- (EN) Number of configured mines. / (VI) Số lượng mìn được cấu hình.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// - (EN) Thrown when rows, columns, or mine count are out of valid range.
    /// - (VI) Được ném ra khi số hàng, số cột, hoặc số lượng mìn nằm ngoài phạm vi hợp lệ.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// - (EN) Thrown when mine count makes a safe first reveal impossible.
    /// - (VI) Được ném ra khi số lượng mìn khiến việc đảm bảo click đầu tiên an toàn là không thể.
    /// </exception>
    private static void ValidateBoardConfiguration(int rows, int columns, int mineCount)
    {
        if (rows <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(rows), "Rows must be greater than zero.");
        }

        if (columns <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(columns), "Columns must be greater than zero.");
        }

        if (mineCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(mineCount), "Mine count cannot be negative.");
        }

        int totalCells = rows * columns;

        if (mineCount >= totalCells)
        {
            throw new ArgumentException(
                "Mine count must be less than the total number of cells to guarantee a safe first reveal.",
                nameof(mineCount));
        }
    }

    /// <summary>
    /// - (EN) Gets all valid neighboring cells around a given position.
    /// - (VI) Lấy tất cả các ô lân cận hợp lệ xung quanh một vị trí.
    /// </summary>
    /// <param name="board">- (EN) The game board instance. / (VI) Board của trò chơi.</param>
    /// <param name="row">- (EN) The row index of the target cell. / (VI) Chỉ số hàng của ô mục tiêu.</param>
    /// <param name="column">- (EN) The column index of the target cell. / (VI) Chỉ số cột của ô mục tiêu.</param>
    /// <returns>
    /// - (EN) A collection of neighboring cells excluding the current cell.
    /// - (VI) Tập hợp các ô lân cận, không bao gồm chính ô hiện tại.
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
    #endregion
}