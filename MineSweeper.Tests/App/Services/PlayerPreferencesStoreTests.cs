using MineSweeper.App.Models;
using MineSweeper.App.Services;
using MineSweeper.Core.Models;

namespace MineSweeper.Tests.App.Services;

/// <summary>
/// - (EN) Contains unit tests for <see cref="PlayerPreferencesStore"/>.
/// - (VI) Chứa các unit test cho <see cref="PlayerPreferencesStore"/>.
/// </summary>
public class PlayerPreferencesStoreTests
{
    /// <summary>
    /// - (EN) Verifies that loading player preferences returns default values when the storage file does not exist.
    /// - (VI) Kiểm tra việc tải tùy chọn người chơi sẽ trả về giá trị mặc định khi file lưu trữ không tồn tại.
    /// </summary>
    [Fact]
    public void Load_ShouldReturnDefault_WhenFileDoesNotExist()
    {
        string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDirectory);

        try
        {
            string filePath = Path.Combine(tempDirectory, "player-preferences.json");
            var store = new PlayerPreferencesStore(filePath);

            var preferences = store.Load();

            Assert.NotNull(preferences);
            Assert.Equal(DifficultyLevel.Beginner, preferences.SelectedDifficulty);
            Assert.Equal(9, preferences.CustomRows);
            Assert.Equal(9, preferences.CustomColumns);
            Assert.Equal(10, preferences.CustomMines);
        }
        finally
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    /// <summary>
    /// - (EN) Verifies that loading player preferences returns default values when the storage file is empty.
    /// - (VI) Kiểm tra việc tải tùy chọn người chơi sẽ trả về giá trị mặc định khi file lưu trữ rỗng.
    /// </summary>
    [Fact]
    public void Load_ShouldReturnDefault_WhenFileIsEmpty()
    {
        string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDirectory);

        try
        {
            string filePath = Path.Combine(tempDirectory, "player-preferences.json");
            File.WriteAllText(filePath, string.Empty);

            var store = new PlayerPreferencesStore(filePath);

            var preferences = store.Load();

            Assert.NotNull(preferences);
            Assert.Equal(DifficultyLevel.Beginner, preferences.SelectedDifficulty);
            Assert.Equal(9, preferences.CustomRows);
            Assert.Equal(9, preferences.CustomColumns);
            Assert.Equal(10, preferences.CustomMines);
        }
        finally
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    /// <summary>
    /// - (EN) Verifies that loading player preferences returns default values when the JSON content is invalid.
    /// - (VI) Kiểm tra việc tải tùy chọn người chơi sẽ trả về giá trị mặc định khi nội dung JSON không hợp lệ.
    /// </summary>
    [Fact]
    public void Load_ShouldReturnDefault_WhenJsonIsInvalid()
    {
        string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDirectory);

        try
        {
            string filePath = Path.Combine(tempDirectory, "player-preferences.json");
            File.WriteAllText(filePath, "{ invalid json");

            var store = new PlayerPreferencesStore(filePath);

            var preferences = store.Load();

            Assert.NotNull(preferences);
            Assert.Equal(DifficultyLevel.Beginner, preferences.SelectedDifficulty);
            Assert.Equal(9, preferences.CustomRows);
            Assert.Equal(9, preferences.CustomColumns);
            Assert.Equal(10, preferences.CustomMines);
        }
        finally
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    /// <summary>
    /// - (EN) Verifies that saving player preferences creates the storage file with serialized content.
    /// - (VI) Kiểm tra việc lưu tùy chọn người chơi sẽ tạo file lưu trữ với nội dung đã được tuần tự hóa.
    /// </summary>
    [Fact]
    public void Save_ShouldCreateFile_WithSerializedPreferences()
    {
        string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDirectory);

        try
        {
            string filePath = Path.Combine(tempDirectory, "player-preferences.json");
            var store = new PlayerPreferencesStore(filePath);

            var preferences = new PlayerPreferencesStorage
            {
                SelectedDifficulty = DifficultyLevel.Custom,
                CustomRows = 12,
                CustomColumns = 14,
                CustomMines = 20
            };

            store.Save(preferences);

            Assert.True(File.Exists(filePath));

            string json = File.ReadAllText(filePath);
            Assert.Contains("\"SelectedDifficulty\"", json);
            Assert.Contains("\"CustomRows\": 12", json);
            Assert.Contains("\"CustomColumns\": 14", json);
            Assert.Contains("\"CustomMines\": 20", json);
        }
        finally
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    /// <summary>
    /// - (EN) Verifies that saved player preferences can be loaded back with the same values.
    /// - (VI) Kiểm tra các tùy chọn người chơi đã lưu có thể được tải lại với đúng các giá trị ban đầu.
    /// </summary>
    [Fact]
    public void Save_ThenLoad_ShouldRoundTripPreferences()
    {
        string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDirectory);

        try
        {
            string filePath = Path.Combine(tempDirectory, "player-preferences.json");
            var store = new PlayerPreferencesStore(filePath);

            var preferences = new PlayerPreferencesStorage
            {
                SelectedDifficulty = DifficultyLevel.Custom,
                CustomRows = 15,
                CustomColumns = 18,
                CustomMines = 30
            };

            store.Save(preferences);

            var loaded = store.Load();

            Assert.Equal(DifficultyLevel.Custom, loaded.SelectedDifficulty);
            Assert.Equal(15, loaded.CustomRows);
            Assert.Equal(18, loaded.CustomColumns);
            Assert.Equal(30, loaded.CustomMines);
        }
        finally
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    /// <summary>
    /// - (EN) Verifies that saving player preferences creates the missing directory automatically and does not throw.
    /// - (VI) Kiểm tra việc lưu tùy chọn người chơi sẽ tự động tạo thư mục còn thiếu và không ném ra ngoại lệ.
    /// </summary>
    [Fact]
    public void Save_ShouldNotThrow_WhenDirectoryDoesNotExist()
    {
        string rootDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        string nestedDirectory = Path.Combine(rootDirectory, "nested", "preferences");
        string filePath = Path.Combine(nestedDirectory, "player-preferences.json");

        try
        {
            var store = new PlayerPreferencesStore(filePath);

            var preferences = new PlayerPreferencesStorage
            {
                SelectedDifficulty = DifficultyLevel.Intermediate,
                CustomRows = 10,
                CustomColumns = 10,
                CustomMines = 15
            };

            var exception = Record.Exception(() => store.Save(preferences));

            Assert.Null(exception);
            Assert.True(File.Exists(filePath));
        }
        finally
        {
            if (Directory.Exists(rootDirectory))
            {
                Directory.Delete(rootDirectory, recursive: true);
            }
        }
    }
}