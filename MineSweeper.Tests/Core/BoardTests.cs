using MineSweeper.Core.Models;

namespace MineSweeper.Tests.Core
{
    /// <summary>
    /// Contains unit tests for verifying the behavior of the Board model.
    /// Focuses on initialization and structural correctness.
    /// </summary>
    public class BoardTests
    {
        /// <summary>
        /// Verifies that the board is created with the correct dimensions.
        /// </summary>
        [Fact]
        public void Constructor_ShouldInitializeBoard_WithCorrectDimensions()
        {
            // Arrange
            int rows = 9;
            int columns = 9;
            int mineCount = 10;

            // Act
            var board = new Board(rows, columns, mineCount);

            // Assert
            Assert.Equal(rows, board.Rows);
            Assert.Equal(columns, board.Columns);
            Assert.Equal(mineCount, board.MineCount);
        }

        /// <summary>
        /// Verifies that all cells are initialized and not null.
        /// </summary>
        [Fact]
        public void Constructor_ShouldInitializeAllCells()
        {
            // Arrange
            int rows = 5;
            int columns = 5;
            int mineCount = 5;

            // Act
            var board = new Board(rows, columns, mineCount);

            // Assert
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    Assert.NotNull(board.Cells[row, col]);
                }
            }
        }

        /// <summary>
        /// Verifies that each cell has the correct row and column indices.
        /// </summary>
        [Fact]
        public void Constructor_ShouldAssignCorrectCellCoordinates()
        {
            // Arrange
            int rows = 3;
            int columns = 3;
            int mineCount = 1;

            // Act
            var board = new Board(rows, columns, mineCount);

            // Assert
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    var cell = board.Cells[row, col];

                    Assert.Equal(row, cell.Row);
                    Assert.Equal(col, cell.Column);
                }
            }
        }

        /// <summary>
        /// Verifies that all cells are initialized with default state.
        /// </summary>
        [Fact]
        public void Constructor_ShouldInitializeCellsWithDefaultState()
        {
            // Arrange
            var board = new Board(4, 4, 2);

            // Act & Assert
            foreach (var cell in board.Cells)
            {
                Assert.False(cell.IsMine);
                Assert.False(cell.IsRevealed);
                Assert.False(cell.IsFlagged);
                Assert.Equal(0, cell.AdjacentMines);
            }
        }
    }
}