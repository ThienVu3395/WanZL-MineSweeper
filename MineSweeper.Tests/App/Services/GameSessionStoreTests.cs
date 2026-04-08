using MineSweeper.App.Models;
using MineSweeper.App.Services;
using MineSweeper.Core.Models;

namespace MineSweeper.Tests.App.Services;

public class GameSessionStoreTests
{
    /// <summary>
    /// - (EN) Verifies that loading a game session returns null when the session file does not exist.
    /// - (VI) Kiểm tra việc tải game session sẽ trả về null khi file session không tồn tại.
    /// </summary>
    [Fact]
    public void Load_ShouldReturnNull_WhenFileDoesNotExist()
    {
        string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDirectory);

        try
        {
            string filePath = Path.Combine(tempDirectory, "game-session.json");
            var store = new GameSessionStore(filePath);

            var session = store.Load();

            Assert.Null(session);
        }
        finally
        {
            Directory.Delete(tempDirectory, true);
        }
    }

    /// <summary>
    /// - (EN) Verifies that a saved game session can be loaded back with the same persisted values.
    /// - (VI) Kiểm tra một game session đã lưu có thể được tải lại với đúng các giá trị đã persist.
    /// </summary>
    [Fact]
    public void Save_AndLoad_ShouldRoundTripSession()
    {
        string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDirectory);

        try
        {
            string filePath = Path.Combine(tempDirectory, "game-session.json");
            var store = new GameSessionStore(filePath);

            var session = new GameSessionStorage
            {
                SelectedDifficulty = DifficultyLevel.Custom,
                Rows = 5,
                Columns = 6,
                MineCount = 7,
                GameState = GameState.InProgress,
                IsFirstRevealPending = false,
                ElapsedTimeInSeconds = 25,
                CustomRows = 5,
                CustomColumns = 6,
                CustomMines = 7,
                Cells =
                [
                    new CellSessionStorage
                    {
                        Row = 0,
                        Column = 0,
                        IsMine = true,
                        IsRevealed = false,
                        IsFlagged = true,
                        AdjacentMines = 0,
                        IsExplodedMine = false,
                        IsIncorrectFlag = false
                    }
                ]
            };

            store.Save(session);

            var loaded = store.Load();

            Assert.NotNull(loaded);
            Assert.Equal(DifficultyLevel.Custom, loaded!.SelectedDifficulty);
            Assert.Equal(5, loaded.Rows);
            Assert.Equal(6, loaded.Columns);
            Assert.Equal(7, loaded.MineCount);
            Assert.Equal(GameState.InProgress, loaded.GameState);
            Assert.Equal(25, loaded.ElapsedTimeInSeconds);
            Assert.Single(loaded.Cells);
            Assert.True(loaded.Cells[0].IsFlagged);
        }
        finally
        {
            Directory.Delete(tempDirectory, true);
        }
    }

    /// <summary>
    /// - (EN) Verifies that loading returns null when the session file contains invalid JSON.
    /// - (VI) Kiểm tra việc tải sẽ trả về null khi file session chứa JSON không hợp lệ.
    /// </summary>
    [Fact]
    public void Load_ShouldReturnNull_WhenJsonIsInvalid()
    {
        string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDirectory);

        try
        {
            string filePath = Path.Combine(tempDirectory, "game-session.json");
            File.WriteAllText(filePath, "{ invalid json");

            var store = new GameSessionStore(filePath);

            var session = store.Load();

            Assert.Null(session);
        }
        finally
        {
            Directory.Delete(tempDirectory, true);
        }
    }

    /// <summary>
    /// - (EN) Verifies that clearing the store deletes the persisted session file when it exists.
    /// - (VI) Kiểm tra thao tác clear store sẽ xóa file session đã lưu khi file đó tồn tại.
    /// </summary>
    [Fact]
    public void Clear_ShouldDeleteSessionFile_WhenItExists()
    {
        string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDirectory);

        try
        {
            string filePath = Path.Combine(tempDirectory, "game-session.json");
            var store = new GameSessionStore(filePath);

            store.Save(new GameSessionStorage
            {
                Rows = 2,
                Columns = 2,
                MineCount = 1,
                GameState = GameState.InProgress
            });

            Assert.True(File.Exists(filePath));

            store.Clear();

            Assert.False(File.Exists(filePath));
        }
        finally
        {
            Directory.Delete(tempDirectory, true);
        }
    }
}