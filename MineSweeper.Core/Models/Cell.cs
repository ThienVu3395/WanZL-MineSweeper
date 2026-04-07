namespace MineSweeper.Core.Models
{
    /// <summary>
    /// Represents a single cell on the MineSweeper board.
    /// A cell can contain a mine, be revealed, be flagged,
    /// and store the number of adjacent mines.
    /// </summary>
    public class Cell
    {
        public Cell(int row, int column)
        {
            Row = row;
            Column = column;
        }

        /// <summary>l
        /// Gets or sets the zero-based row index of the cell.
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// Gets or sets the zero-based column index of the cell.
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this cell contains a mine.
        /// - Có chứa mìn không
        /// </summary>
        public bool IsMine { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this cell has been revealed by the player.
        /// - Đã mở chưa
        /// </summary>
        public bool IsRevealed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this cell is currently flagged by the player.
        /// - Có bị đánh dấu cờ không
        /// </summary>
        public bool IsFlagged { get; set; }

        /// <summary>
        /// Gets or sets the number of mines adjacent to this cell.
        /// - Số mìn xung quanh
        /// </summary>
        public int AdjacentMines { get; set; }

        /// <summary>
        /// - (EN) Gets or sets a value indicating whether this mine cell is the one that exploded.
        /// - (VI) Lấy hoặc gán giá trị cho biết ô mìn này có phải là ô phát nổ hay không.
        /// </summary>
        public bool IsExplodedMine { get; set; }

        /// <summary>
        /// - (EN) Gets or sets a value indicating whether this flagged cell is incorrect.
        /// - (VI) Lấy hoặc gán giá trị cho biết ô đang cắm cờ này có phải là cờ sai hay không.
        /// </summary>
        public bool IsIncorrectFlag { get; set; }
    }
}
