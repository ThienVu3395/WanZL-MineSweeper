using MineSweeper.Core.Models;

namespace MineSweeper.App.Extensions
{
    /// <summary>
    /// - (EN) Provides display text and predefined board presets for supported MineSweeper difficulty levels.
    /// - (VI) Cung cấp chuỗi hiển thị và cấu hình board định sẵn cho các mức độ khó được hỗ trợ trong MineSweeper.
    /// </summary>
    public static class DifficultyExtensions
    {
        /// <summary>
        /// - (EN) Converts a difficulty level into a user-friendly display string for the UI.
        /// - (VI) Chuyển một mức độ khó thành chuỗi hiển thị thân thiện với người dùng trên giao diện.
        /// </summary>
        /// <param name="difficulty">
        /// - (EN) The selected difficulty level.
        /// - (VI) Mức độ khó được chọn.
        /// </param>
        /// <returns>
        /// - (EN) A formatted display string containing difficulty name, board size, and mine count.
        /// - (VI) Chuỗi hiển thị đã được định dạng, bao gồm tên độ khó, kích thước board, và số lượng mìn.
        /// </returns>
        public static string ToDisplayString(this DifficultyLevel difficulty)
        {
            return difficulty switch
            {
                DifficultyLevel.Beginner => "Beginner (9x9 | 10 mines)",
                DifficultyLevel.Intermediate => "Intermediate (16x16 | 40 mines)",
                DifficultyLevel.Expert => "Expert (16x30 | 99 mines)",
                _ => difficulty.ToString()
            };
        }

        /// <summary>
        /// - (EN) Converts a difficulty level into its corresponding board preset.
        /// - (VI) Chuyển một mức độ khó thành cấu hình board tương ứng.
        /// </summary>
        /// <param name="difficulty">
        /// - (EN) The selected difficulty level.
        /// - (VI) Mức độ khó được chọn.
        /// </param>
        /// <returns>
        /// - (EN) The matching preset containing board rows, columns, and mine count.
        /// - (VI) Preset tương ứng, bao gồm số hàng, số cột, và số lượng mìn của board.
        /// </returns>
        public static DifficultyPreset ToPreset(this DifficultyLevel difficulty)
        {
            return difficulty switch
            {
                DifficultyLevel.Beginner => new DifficultyPreset(9, 9, 10),
                DifficultyLevel.Intermediate => new DifficultyPreset(16, 16, 40),
                DifficultyLevel.Expert => new DifficultyPreset(16, 30, 99),
                _ => new DifficultyPreset(9, 9, 10)
            };
        }

        /// <summary>
        /// - (EN) Represents a predefined board configuration for a MineSweeper difficulty level.
        /// - (VI) Đại diện cho một cấu hình board được định nghĩa sẵn cho một mức độ khó của MineSweeper.
        /// </summary>
        public class DifficultyPreset
        {
            /// <summary>
            /// - (EN) Initializes a new instance of the <see cref="DifficultyPreset"/> class.
            /// - (VI) Khởi tạo một instance mới của <see cref="DifficultyPreset"/>.
            /// </summary>
            /// <param name="rows">- (EN) Number of rows / (VI) Số hàng</param>
            /// <param name="columns">- (EN) Number of columns / (VI) Số cột</param>
            /// <param name="mineCount">- (EN) Number of mines / (VI) Số lượng mìn</param>
            public DifficultyPreset(int rows, int columns, int mineCount)
            {
                Rows = rows;
                Columns = columns;
                MineCount = mineCount;
            }

            /// <summary>
            /// - (EN) Gets the number of rows for the board.
            /// - (VI) Lấy số hàng của board.
            /// </summary>
            public int Rows { get; }

            /// <summary>
            /// - (EN) Gets the number of columns for the board.
            /// - (VI) Lấy số cột của board.
            /// </summary>
            public int Columns { get; }

            /// <summary>
            /// - (EN) Gets the number of mines for the board.
            /// - (VI) Lấy số lượng mìn của board.
            /// </summary>
            public int MineCount { get; }
        }
    }
}
