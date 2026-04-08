namespace MineSweeper.App.Models
{
    /// <summary>
    /// - (EN) Represents the persisted state of a single board cell for session restore.
    /// - (VI) Đại diện cho trạng thái được lưu của một ô trên board để khôi phục phiên chơi.
    /// </summary>
    public class CellSessionStorage
    {
        /// <summary>
        /// - (EN) Gets or sets the row index.
        /*  - (VI) Lấy hoặc gán chỉ số hàng. */
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// - (EN) Gets or sets the column index.
        /// - (VI) Lấy hoặc gán chỉ số cột.
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// - (EN) Gets or sets whether the cell contains a mine.
        /// - (VI) Lấy hoặc gán giá trị cho biết ô có chứa mìn hay không.
        /// </summary>
        public bool IsMine { get; set; }

        /// <summary>
        /// - (EN) Gets or sets whether the cell has been revealed.
        /// - (VI) Lấy hoặc gán giá trị cho biết ô đã được mở hay chưa.
        /// </summary>
        public bool IsRevealed { get; set; }

        /// <summary>
        /// - (EN) Gets or sets whether the cell is flagged.
        /// - (VI) Lấy hoặc gán giá trị cho biết ô có đang bị cắm cờ hay không.
        /// </summary>
        public bool IsFlagged { get; set; }

        /// <summary>
        /// - (EN) Gets or sets the adjacent mine count.
        /// - (VI) Lấy hoặc gán số lượng mìn lân cận.
        /// </summary>
        public int AdjacentMines { get; set; }

        /// <summary>
        /// - (EN) Gets or sets whether this cell is the exploded mine.
        /// - (VI) Lấy hoặc gán giá trị cho biết ô này có phải là ô mìn phát nổ hay không.
        /// </summary>
        public bool IsExplodedMine { get; set; }

        /// <summary>
        /// - (EN) Gets or sets whether this flagged cell is incorrect.
        /// - (VI) Lấy hoặc gán giá trị cho biết ô cắm cờ này có phải là cờ sai hay không.
        /// </summary>
        public bool IsIncorrectFlag { get; set; }
    }
}
