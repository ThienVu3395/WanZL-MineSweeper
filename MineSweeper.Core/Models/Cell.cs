namespace MineSweeper.Core.Models
{
    /// <summary>
    /// Represents a single cell on the MineSweeper board.
    /// A cell can contain a mine, be revealed, be flagged,
    /// and store the number of adjacent mines.
    /// </summary>
    public class Cell
    {
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
        /// </summary>
        public bool IsMine { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this cell has been revealed by the player.
        /// </summary>
        public bool IsRevealed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this cell is currently flagged by the player.
        /// </summary>
        public bool IsFlagged { get; set; }

        /// <summary>
        /// Gets or sets the number of mines adjacent to this cell.
        /// </summary>
        public int AdjacentMines { get; set; }
    }
}
