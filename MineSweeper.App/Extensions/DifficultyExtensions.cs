using MineSweeper.Core.Models;

namespace MineSweeper.App.Extensions
{
    public static class DifficultyExtensions
    {
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
    }
}
