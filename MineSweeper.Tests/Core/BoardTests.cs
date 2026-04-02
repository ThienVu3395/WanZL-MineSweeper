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
        /// - Đảm bảo board được tạo đúng kích thước
        /// </summary>
        [Fact]
        public void Constructor_ShouldInitializeBoard_WithCorrectDimensions()
        {
            // Arrange - chuẩn bị dữ liệu đầu vào
            int rows = 9;
            int columns = 9;
            int mineCount = 10;

            // Act - thực thi logic
            var board = new Board(rows, columns, mineCount);

            // Assert - kiểm tra kết quả
            Assert.Equal(rows, board.Rows);
            Assert.Equal(columns, board.Columns);
            Assert.Equal(mineCount, board.MineCount);
        }

        /// <summary>
        /// Verifies that all cells are initialized and not null.
        /// - Kiểm tra toàn bộ cell đã được khởi tạo (không null)
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
        /// - Đảm bảo mỗi cell biết đúng vị trí của nó
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
        /// - Kiểm tra trạng thái mặc định của cell
        /// </summary>
        [Fact]
        public void Constructor_ShouldInitializeCellsWithDefaultState()
        {
            // Arrange
            var board = new Board(4, 4, 2);

            // Act & Assert
            foreach (var cell in board.Cells)
            {
                // Ban đầu chưa có mìn
                Assert.False(cell.IsMine);

                // Chưa mở
                Assert.False(cell.IsRevealed);

                // Chưa flag
                Assert.False(cell.IsFlagged);

                // Chưa có số mìn xung quanh
                Assert.Equal(0, cell.AdjacentMines);
            }
        }
    }
}