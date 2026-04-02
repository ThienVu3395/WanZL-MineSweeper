namespace MineSweeper.Core.Models
{
    /// <summary>
    /// Represents the supported difficulty presets for the MineSweeper game.
    /// - Cấu hình độ khó cho game (có thể sử dụng sau)
    /// </summary>
    public enum DifficultyLevel
    {
        /// <summary>
        /// Entry-level difficulty intended for new players.
        /// </summary>
        Beginner = 0,

        /// <summary>
        /// Standard difficulty that provides a moderate challenge.
        /// </summary>
        Intermediate = 1,

        /// <summary>
        /// Advanced difficulty for experienced players.
        /// </summary>
        Expert = 2,

        /// <summary>
        /// Custom difficulty defined by user-selected board settings.
        /// </summary>
        Custom = 3
    }
}
