using MineSweeper.App.Models;
using MineSweeper.Core.Models;
using System.IO;
using System.Text.Json;

namespace MineSweeper.App.Services
{
    /// <summary>
    /// - (EN) Provides local JSON persistence for player statistics such as best times.
    /// - (VI) Cung cấp cơ chế lưu trữ JSON cục bộ cho thống kê người chơi như best time.
    /// </summary>
    public class PlayerStatisticsStore
    {
        /// <summary>
        /// - (EN) Initializes a new instance of the <see cref="PlayerStatisticsStore"/> class.
        /// - (VI) Khởi tạo một instance mới của <see cref="PlayerStatisticsStore"/>.
        /// </summary>
        /// <param name="filePath">
        /// - (EN) The storage file path.
        /// - (VI) Đường dẫn file lưu trữ.
        /// </param>
        public PlayerStatisticsStore(string filePath)
        {
            FilePath = filePath;
        }

        /// <summary>
        /// - (EN) Gets the underlying storage file path.
        /// - (VI) Lấy đường dẫn file lưu trữ bên dưới.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// - (EN) Gets the default local file path used to store player statistics.
        /// - (VI) Lấy đường dẫn file cục bộ mặc định dùng để lưu thống kê người chơi.
        /// </summary>
        /// <returns>
        /// - (EN) The full local path for the player statistics file.
        /// - (VI) Đường dẫn cục bộ đầy đủ tới file thống kê người chơi.
        /// </returns>
        public static string GetDefaultFilePath()
        {
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string applicationFolder = Path.Combine(appDataFolder, "MineSweeper");

            return Path.Combine(applicationFolder, "best-times.json");
        }

        /// <summary>
        /// - (EN) Loads best times from local storage.
        /// Returns an empty dictionary when the file is missing, invalid, or unreadable.
        /// - (VI) Tải best time từ local storage.
        /// Trả về dictionary rỗng nếu file không tồn tại, không hợp lệ, hoặc không thể đọc được.
        /// </summary>
        /// <returns>
        /// - (EN) A dictionary of best times keyed by difficulty.
        /// - (VI) Dictionary chứa best time được ánh xạ theo độ khó.
        /// </returns>
        public Dictionary<DifficultyLevel, TimeSpan> LoadBestTimes()
        {
            try
            {
                if (!File.Exists(FilePath))
                {
                    return new Dictionary<DifficultyLevel, TimeSpan>();
                }

                string json = File.ReadAllText(FilePath);

                if (string.IsNullOrWhiteSpace(json))
                {
                    return new Dictionary<DifficultyLevel, TimeSpan>();
                }

                var storage = JsonSerializer.Deserialize<PlayerStatisticsStorage>(json);

                if (storage?.BestTimesInSeconds == null)
                {
                    return new Dictionary<DifficultyLevel, TimeSpan>();
                }

                var result = new Dictionary<DifficultyLevel, TimeSpan>();

                foreach (var pair in storage.BestTimesInSeconds)
                {
                    if (!Enum.TryParse<DifficultyLevel>(pair.Key, ignoreCase: true, out var difficulty))
                    {
                        continue;
                    }

                    if (difficulty == DifficultyLevel.Custom)
                    {
                        continue;
                    }

                    if (pair.Value < 0)
                    {
                        continue;
                    }

                    result[difficulty] = TimeSpan.FromSeconds(pair.Value);
                }

                return result;
            }
            catch
            {
                return new Dictionary<DifficultyLevel, TimeSpan>();
            }
        }

        /// <summary>
        /// - (EN) Saves best times to local storage.
        /// Persistence failures are intentionally ignored so gameplay is not interrupted.
        /// - (VI) Lưu best time vào local storage.
        /// Các lỗi khi lưu được cố ý bỏ qua để không làm gián đoạn trải nghiệm chơi game.
        /// </summary>
        /// <param name="bestTimes">
        /// - (EN) Best times keyed by difficulty.
        /// - (VI) Best time được ánh xạ theo độ khó.
        /// </param>
        public void SaveBestTimes(Dictionary<DifficultyLevel, TimeSpan> bestTimes)
        {
            try
            {
                string? directory = Path.GetDirectoryName(FilePath);

                if (!string.IsNullOrWhiteSpace(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var storage = new PlayerStatisticsStorage
                {
                    BestTimesInSeconds = bestTimes
                        .Where(x => x.Key != DifficultyLevel.Custom)
                        .ToDictionary(
                            x => x.Key.ToString(),
                            x => x.Value.TotalSeconds)
                };

                string json = JsonSerializer.Serialize(storage, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(FilePath, json);
            }
            catch
            {
                // Intentionally ignored.
            }
        }
    }
}
