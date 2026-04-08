namespace MineSweeper.Core.Models
{
    /// <summary>
    /// - (EN) Represents the current lifecycle state of a MineSweeper game session.
    /// - (VI) Chứa trạng thái của game
    /// </summary>
    public enum GameState
    {
        /// <summary>
        /// - (EN) The game has not started yet.
        /// - (VI) Game chưa bắt đầu
        /// </summary>
        NotStarted = 0,

        /// <summary>
        /// - (EN) The game is currently in progress.
        /// - (VI) Game đang bắt đầu
        /// </summary>
        InProgress = 1,

        /// <summary>
        /// - (EN) The player has successfully cleared the board and won the game.
        /// - (VI) Game đã thắng
        /// </summary>
        Won = 2,

        /// <summary>
        ///  - (EN) The player has revealed a mine and lost the game.
        ///  - (VI) Game đã thua
        /// </summary>
        Lost = 3
    }
}
