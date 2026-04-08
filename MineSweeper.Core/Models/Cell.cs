namespace MineSweeper.Core.Models
{
    /// <summary>
    /// - (EN) Represents a single cell on the MineSweeper board with its position, mine state, reveal state, flag state, and adjacent mine count.
    /// - (VI) Đại diện cho một ô trên bàn chơi MineSweeper với vị trí, trạng thái mìn, trạng thái mở, trạng thái cắm cờ và số lượng mìn lân cận.
    /// </summary>
    public class Cell
    {
        /// <summary>
        /// - (EN) Initializes a new instance of the Cell class with the specified row and column.
        /// - (VI) Khởi tạo một instance mới của lớp Cell với hàng và cột được chỉ định.
        /// </summary>
        public Cell(int row, int column)
        {
            Row = row;
            Column = column;
        }

        /// <summary>
        /// - (EN) Gets or sets the zero-based row index of the cell.
        /// - (VI) Lấy hoặc gán chỉ số hàng bắt đầu từ 0 của ô.
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// - (EN) Gets or sets the zero-based column index of the cell.
        /// - (VI) Lấy hoặc gán chỉ số cột bắt đầu từ 0 của ô.
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// - (EN) Gets or sets a value indicating whether this cell contains a mine.
        /// - (VI) Lấy hoặc gán giá trị cho biết ô này có chứa mìn hay không.
        /// </summary>
        public bool IsMine { get; set; }

        /// <summary>
        /// - (EN) Gets or sets a value indicating whether this cell has been revealed by the player.
        /// - (VI) Lấy hoặc gán giá trị cho biết ô này đã được người chơi mở hay chưa.
        /// </summary>
        public bool IsRevealed { get; set; }

        /// <summary>
        /// - (EN) Gets or sets a value indicating whether this cell is currently flagged by the player.
        /// - (VI) Lấy hoặc gán giá trị cho biết ô này hiện đang được người chơi đánh dấu cờ hay không.
        /// </summary>
        public bool IsFlagged { get; set; }

        /// <summary>
        /// - (EN) Gets or sets the number of mines adjacent to this cell.
        /// - (VI) Lấy hoặc gán số lượng mìn ở các ô lân cận của ô này.
        /// </summary>
        public int AdjacentMines { get; set; }

        /// <summary>
        /// - (EN) Gets or sets a value indicating whether this mine cell is the one that exploded.
        /// - (VI) Lấy hoặc gán giá trị cho biết ô mìn này có phải là ô đã phát nổ hay không.
        /// </summary>
        public bool IsExplodedMine { get; set; }

        /// <summary>
        /// - (EN) Gets or sets a value indicating whether this flagged cell is incorrect.
        /// - (VI) Lấy hoặc gán giá trị cho biết ô đang cắm cờ này có phải là cờ sai hay không.
        /// </summary>
        public bool IsIncorrectFlag { get; set; }
    }
}
