namespace MineSweeper.Core.Models
{
    /// <summary>
    /// Represents the current lifecycle state of a MineSweeper game session.
    /// - Chứa trạng thái của game
    /// </summary>
    public enum GameState
    {
        /// <summary>
        /// The game has not started yet.
        /// </summary>
        NotStarted = 0,

        /// <summary>
        /// The game is currently in progress.
        /// </summary>
        InProgress = 1,

        /// <summary>
        /// The player has successfully cleared the board and won the game.
        /// </summary>
        Won = 2,

        /// <summary>
        /// The player has revealed a mine and lost the game.
        /// </summary>
        Lost = 3
    }
}
