namespace MineSweeper.Core.Models
{
    /// <summary>
    /// Represents the game board that contains all cells
    /// and basic board configuration such as dimensions and mine count.
    /// - Quản lý toàn bộ grid, chứa:
    /// + Kích thước
    /// + Danh sách Cell
    /// + Logic generate mìn
    /// </summary>
    public class Board
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Board"/> class.
        /// </summary>
        /// <param name="rows">The total number of rows on the board.</param>
        /// <param name="columns">The total number of columns on the board.</param>
        /// <param name="mineCount">The total number of mines configured for the board.</param>
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
        /// Gets the total number of rows on the board.
        /// </summary>
        public int Rows { get; }

        /// <summary>
        /// Gets the total number of columns on the board.
        /// </summary>
        public int Columns { get; }

        /// <summary>
        /// Gets the total number of mines configured for the board.
        /// </summary>
        public int MineCount { get; }

        /// <summary>
        /// Gets the two-dimensional collection of cells that make up the board.
        /// </summary>
        public Cell[,] Cells { get; }
    }
}
