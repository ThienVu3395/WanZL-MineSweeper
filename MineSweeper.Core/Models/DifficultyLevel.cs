namespace MineSweeper.Core.Models
{
    /// <summary>
    /// - (EN) Represents the supported difficulty presets for the MineSweeper game.
    /// - (VI) Cấu hình độ khó cho game
    /// </summary>
    public enum DifficultyLevel
    {
        /// <summary>
        /// - (EN) Entry-level difficulty intended for new players.
        /// - (VI) Dễ
        /// </summary>
        Beginner = 0,

        /// <summary>
        /// - (EN) Standard difficulty that provides a moderate challenge.
        /// - (VI) Trung bình
        /// </summary>
        Intermediate = 1,

        /// <summary>
        /// - (EN) Advanced difficulty for experienced players.
        /// - (VI) Khó
        /// </summary>
        Expert = 2,

        /// <summary>
        /// - (EN) Custom difficulty defined by user-selected board settings.
        /// - (VI) Tùy chọn
        /// </summary>
        Custom = 3
    }
}
