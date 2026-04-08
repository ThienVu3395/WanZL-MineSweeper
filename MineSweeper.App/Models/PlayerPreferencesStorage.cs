using MineSweeper.Core.Models;

namespace MineSweeper.App.Models
{
    /// <summary>
    /// - (EN) Represents persisted player preferences for the MineSweeper application.
    /// - (VI) Đại diện cho các tùy chọn người chơi được lưu bền vững cho ứng dụng MineSweeper.
    /// </summary>
    public class PlayerPreferencesStorage
    {
        /// <summary>
        /// - (EN) Gets or sets the last selected difficulty.
        /// - (VI) Lấy hoặc gán độ khó được chọn gần nhất.
        /// </summary>
        public DifficultyLevel SelectedDifficulty { get; set; } = DifficultyLevel.Beginner;

        /// <summary>
        /// - (EN) Gets or sets the persisted custom board row count.
        /// - (VI) Lấy hoặc gán số hàng custom được lưu.
        /// </summary>
        public int CustomRows { get; set; } = 9;

        /// <summary>
        /// - (EN) Gets or sets the persisted custom board column count.
        /// - (VI) Lấy hoặc gán số cột custom được lưu.
        /// </summary>
        public int CustomColumns { get; set; } = 9;

        /// <summary>
        /// - (EN) Gets or sets the persisted custom mine count.
        /// - (VI) Lấy hoặc gán số lượng mìn custom được lưu.
        /// </summary>
        public int CustomMines { get; set; } = 10;
    }
}
