using MineSweeper.Core.Models;

namespace MineSweeper.Tests.Core.Models
{
    /// <summary>
    /// - (EN) Contains unit tests for Board, focusing on initialization behavior and structural correctness.
    /// - (VI) Chứa các bài kiểm thử đơn vị cho Board, tập trung vào hành vi khởi tạo và tính đúng đắn của cấu trúc.
    /// </summary>
    public class BoardTests
    {
        /// <summary>
        /// - (EN) Should initialize the board with the correct dimensions and mine count.
        /// - (VI) Phải khởi tạo board với đúng kích thước và số lượng mìn.
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
        /// - (EN) Should initialize all cells in the board and ensure none of them are null.
        /// - (VI) Phải khởi tạo toàn bộ các ô trên board và đảm bảo không ô nào bị null.
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
        /// - (EN) Should assign the correct row and column coordinates to each cell.
        /// - (VI) Phải gán đúng tọa độ hàng và cột cho từng ô.
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
        /// - (EN) Should initialize all cells with the default state values.
        /// - (VI) Phải khởi tạo tất cả các ô với các giá trị trạng thái mặc định.
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