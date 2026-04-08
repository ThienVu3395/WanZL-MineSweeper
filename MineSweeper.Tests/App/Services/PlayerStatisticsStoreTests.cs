using MineSweeper.App.Services;
using MineSweeper.Core.Models;

namespace MineSweeper.Tests.App.Services;

/// <summary>
/// - (EN) Contains unit tests for <see cref="PlayerStatisticsStore"/>.
/// - (VI) Chứa các unit test cho <see cref="PlayerStatisticsStore"/>.
/// </summary>
public class PlayerStatisticsStoreTests
{
    /// <summary>
    /// - (EN) Verifies that loading best times returns an empty dictionary when the storage file does not exist.
    /// - (VI) Kiểm tra việc tải best time sẽ trả về dictionary rỗng khi file lưu trữ không tồn tại.
    /// </summary>
    [Fact]
    public void LoadBestTimes_ShouldReturnEmpty_WhenFileDoesNotExist()
    {
        string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDirectory);

        try
        {
            string filePath = Path.Combine(tempDirectory, "best-times.json");
            var store = new PlayerStatisticsStore(filePath);

            var bestTimes = store.LoadBestTimes();

            Assert.NotNull(bestTimes);
            Assert.Empty(bestTimes);
        }
        finally
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    /// <summary>
    /// - (EN) Verifies that loading best times returns an empty dictionary when the storage file is empty.
    /// - (VI) Kiểm tra việc tải best time sẽ trả về dictionary rỗng khi file lưu trữ rỗng.
    /// </summary>
    [Fact]
    public void LoadBestTimes_ShouldReturnEmpty_WhenFileIsEmpty()
    {
        string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDirectory);

        try
        {
            string filePath = Path.Combine(tempDirectory, "best-times.json");
            File.WriteAllText(filePath, string.Empty);

            var store = new PlayerStatisticsStore(filePath);

            var bestTimes = store.LoadBestTimes();

            Assert.NotNull(bestTimes);
            Assert.Empty(bestTimes);
        }
        finally
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    /// <summary>
    /// - (EN) Verifies that loading best times returns an empty dictionary when the JSON content is invalid.
    /// - (VI) Kiểm tra việc tải best time sẽ trả về dictionary rỗng khi nội dung JSON không hợp lệ.
    /// </summary>
    [Fact]
    public void LoadBestTimes_ShouldReturnEmpty_WhenJsonIsInvalid()
    {
        string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDirectory);

        try
        {
            string filePath = Path.Combine(tempDirectory, "best-times.json");
            File.WriteAllText(filePath, "{ invalid json");

            var store = new PlayerStatisticsStore(filePath);

            var bestTimes = store.LoadBestTimes();

            Assert.NotNull(bestTimes);
            Assert.Empty(bestTimes);
        }
        finally
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    /// <summary>
    /// - (EN) Verifies that loading best times ignores unsupported difficulty keys from persisted data.
    /// - (VI) Kiểm tra việc tải best time sẽ bỏ qua các khóa độ khó không được hỗ trợ trong dữ liệu đã lưu.
    /// </summary>
    [Fact]
    public void LoadBestTimes_ShouldIgnoreUnsupportedDifficultyKeys()
    {
        string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDirectory);

        try
        {
            string filePath = Path.Combine(tempDirectory, "best-times.json");
            string json =
                """
                {
                  "BestTimesInSeconds": {
                    "Beginner": 42,
                    "Impossible": 11,
                    "UnknownValue": 99
                  }
                }
                """;

            File.WriteAllText(filePath, json);

            var store = new PlayerStatisticsStore(filePath);

            var bestTimes = store.LoadBestTimes();

            Assert.Single(bestTimes);
            Assert.Equal(TimeSpan.FromSeconds(42), bestTimes[DifficultyLevel.Beginner]);
        }
        finally
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    /// <summary>
    /// - (EN) Verifies that loading best times ignores persisted values for Custom difficulty.
    /// - (VI) Kiểm tra việc tải best time sẽ bỏ qua các giá trị đã lưu cho độ khó Custom.
    /// </summary>
    [Fact]
    public void LoadBestTimes_ShouldIgnoreCustomDifficulty()
    {
        string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDirectory);

        try
        {
            string filePath = Path.Combine(tempDirectory, "best-times.json");
            string json =
                """
                {
                  "BestTimesInSeconds": {
                    "Beginner": 42,
                    "Custom": 77
                  }
                }
                """;

            File.WriteAllText(filePath, json);

            var store = new PlayerStatisticsStore(filePath);

            var bestTimes = store.LoadBestTimes();

            Assert.Single(bestTimes);
            Assert.DoesNotContain(DifficultyLevel.Custom, bestTimes.Keys);
            Assert.Equal(TimeSpan.FromSeconds(42), bestTimes[DifficultyLevel.Beginner]);
        }
        finally
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    /// <summary>
    /// - (EN) Verifies that loading best times ignores negative persisted duration values.
    /// - (VI) Kiểm tra việc tải best time sẽ bỏ qua các giá trị thời lượng âm đã được lưu.
    /// </summary>
    [Fact]
    public void LoadBestTimes_ShouldIgnoreNegativeValues()
    {
        string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDirectory);

        try
        {
            string filePath = Path.Combine(tempDirectory, "best-times.json");
            string json =
                """
                {
                  "BestTimesInSeconds": {
                    "Beginner": -5,
                    "Intermediate": 90
                  }
                }
                """;

            File.WriteAllText(filePath, json);

            var store = new PlayerStatisticsStore(filePath);

            var bestTimes = store.LoadBestTimes();

            Assert.Single(bestTimes);
            Assert.DoesNotContain(DifficultyLevel.Beginner, bestTimes.Keys);
            Assert.Equal(TimeSpan.FromSeconds(90), bestTimes[DifficultyLevel.Intermediate]);
        }
        finally
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    /// <summary>
    /// - (EN) Verifies that saving best times excludes Custom difficulty from persisted storage.
    /// - (VI) Kiểm tra việc lưu best time sẽ loại trừ độ khó Custom khỏi dữ liệu lưu trữ.
    /// </summary>
    [Fact]
    public void SaveBestTimes_ShouldExcludeCustomDifficulty()
    {
        string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDirectory);

        try
        {
            string filePath = Path.Combine(tempDirectory, "best-times.json");
            var store = new PlayerStatisticsStore(filePath);

            var bestTimes = new Dictionary<DifficultyLevel, TimeSpan>
            {
                [DifficultyLevel.Beginner] = TimeSpan.FromSeconds(42),
                [DifficultyLevel.Custom] = TimeSpan.FromSeconds(77)
            };

            store.SaveBestTimes(bestTimes);

            string json = File.ReadAllText(filePath);

            Assert.Contains("\"Beginner\"", json);
            Assert.DoesNotContain("\"Custom\"", json);
        }
        finally
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    /// <summary>
    /// - (EN) Verifies that valid saved best times can be loaded back with the same values.
    /// - (VI) Kiểm tra các best time hợp lệ đã lưu có thể được tải lại với đúng các giá trị ban đầu.
    /// </summary>
    [Fact]
    public void SaveBestTimes_ThenLoadBestTimes_ShouldRoundTripValidValues()
    {
        string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDirectory);

        try
        {
            string filePath = Path.Combine(tempDirectory, "best-times.json");
            var store = new PlayerStatisticsStore(filePath);

            var bestTimes = new Dictionary<DifficultyLevel, TimeSpan>
            {
                [DifficultyLevel.Beginner] = TimeSpan.FromSeconds(42),
                [DifficultyLevel.Intermediate] = TimeSpan.FromSeconds(90),
                [DifficultyLevel.Expert] = TimeSpan.FromSeconds(180)
            };

            store.SaveBestTimes(bestTimes);

            var loaded = store.LoadBestTimes();

            Assert.Equal(3, loaded.Count);
            Assert.Equal(TimeSpan.FromSeconds(42), loaded[DifficultyLevel.Beginner]);
            Assert.Equal(TimeSpan.FromSeconds(90), loaded[DifficultyLevel.Intermediate]);
            Assert.Equal(TimeSpan.FromSeconds(180), loaded[DifficultyLevel.Expert]);
        }
        finally
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    /// <summary>
    /// - (EN) Verifies that saving best times creates the missing directory automatically and does not throw.
    /// - (VI) Kiểm tra việc lưu best time sẽ tự động tạo thư mục còn thiếu và không ném ra ngoại lệ.
    /// </summary>
    [Fact]
    public void SaveBestTimes_ShouldNotThrow_WhenDirectoryDoesNotExist()
    {
        string rootDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        string nestedDirectory = Path.Combine(rootDirectory, "nested", "stats");
        string filePath = Path.Combine(nestedDirectory, "best-times.json");

        try
        {
            var store = new PlayerStatisticsStore(filePath);

            var bestTimes = new Dictionary<DifficultyLevel, TimeSpan>
            {
                [DifficultyLevel.Beginner] = TimeSpan.FromSeconds(50)
            };

            var exception = Record.Exception(() => store.SaveBestTimes(bestTimes));

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