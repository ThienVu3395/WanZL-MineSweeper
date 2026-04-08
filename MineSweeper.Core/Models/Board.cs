namespace MineSweeper.Core.Models
{
    /// <summary>
    /// - (EN) Represents the game board containing all cells, dimensions, and mine configuration.
    /// - (VI) Đại diện cho bàn chơi chứa toàn bộ các ô, kích thước và cấu hình mìn.
    /// </summary>
    public class Board
    {
        /// <summary>
        /// - (EN) Initializes a new instance of the Board class with specified dimensions and mine count.
        /// - (VI) Khởi tạo một instance mới của Board với kích thước và số lượng mìn được chỉ định.
        /// </summary>
        /// <param name="rows">
        /// - (EN) The total number of rows on the board.
        /// - (VI) Tổng số hàng của bàn chơi.
        /// </param>
        /// <param name="columns">
        /// - (EN) The total number of columns on the board.
        /// - (VI) Tổng số cột của bàn chơi.
        /// </param>
        /// <param name="mineCount">
        /// - (EN) The total number of mines configured for the board.
        /// - (VI) Tổng số lượng mìn được cấu hình trên bàn chơi.
        /// </param>
        public Board(int rows, int columns, int mineCount)
        {
            Rows = rows;
            Columns = columns;
            MineCount = mineCount;
            Cells = new Cell[rows, columns];

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    Cells[row, col] = new Cell(row, col);
                }
            }
        }

        /// <summary>
        /// - (EN) Gets the total number of rows on the board.
        /// - (VI) Lấy tổng số hàng của bàn chơi.
        /// </summary>
        public int Rows { get; }

        /// <summary>
        /// - (EN) Gets the total number of columns on the board.
        /// - (VI) Lấy tổng số cột của bàn chơi.
        /// </summary>
        public int Columns { get; }

        /// <summary>
        /// - (EN) Gets the total number of mines configured for the board.
        /// - (VI) Lấy tổng số lượng mìn được cấu hình trên bàn chơi.
        /// </summary>
        public int MineCount { get; }

        /// <summary>
        /// - (EN) Gets the two-dimensional collection of cells that make up the board.
        /// - (VI) Lấy tập hợp hai chiều các ô tạo nên bàn chơi.
        /// </summary>
        public Cell[,] Cells { get; }
    }
}
