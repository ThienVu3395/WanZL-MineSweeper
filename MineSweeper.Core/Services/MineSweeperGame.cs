using MineSweeper.Core.Models;

namespace MineSweeper.Core.Services
{
    /// <summary>
    /// Represents the main game service responsible for managing
    /// the lifecycle and state of a MineSweeper session.
    /// </summary>
    public class MineSweeperGame
    {
        /// <summary>
        /// Starts a new game with the specified board dimensions and mine count.
        /// </summary>
        /// <param name="rows">The number of rows for the new board.</param>
        /// <param name="columns">The number of columns for the new board.</param>
        /// <param name="mineCount">The number of mines configured for the new board.</param>
        public void StartNewGame(int rows, int columns, int mineCount)
        {
            Board = new Board(rows, columns, mineCount);
            State = GameState.InProgress;
        }

        /// <summary>
        /// Gets the current board instance for the active game.
        /// </summary>
        public Board? Board { get; private set; }

        /// <summary>
        /// Gets the current state of the game session.
        /// </summary>
        public GameState State { get; private set; } = GameState.NotStarted;
    }
}
