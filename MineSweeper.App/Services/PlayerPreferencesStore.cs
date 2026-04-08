using MineSweeper.App.Models;
using System.IO;
using System.Text.Json;

namespace MineSweeper.App.Services
{
    /// <summary>
    /// - (EN) Provides local JSON persistence for player preferences such as difficulty
    /// and custom board configuration.
    /// - (VI) Cung cấp cơ chế lưu trữ JSON cục bộ cho các tùy chọn người chơi
    /// như độ khó và cấu hình board custom.
    /// </summary>
    public class PlayerPreferencesStore
    {
        /// <summary>
        /// - (EN) Initializes a new instance of the <see cref="PlayerPreferencesStore"/> class.
        /// - (VI) Khởi tạo một instance mới của <see cref="PlayerPreferencesStore"/>.
        /// </summary>
        /// <param name="filePath">
        /// - (EN) The storage file path.
        /// - (VI) Đường dẫn file lưu trữ.
        /// </param>
        public PlayerPreferencesStore(string filePath)
        {
            FilePath = filePath;
        }

        /// <summary>
        /// - (EN) Gets the underlying storage file path.
        /// - (VI) Lấy đường dẫn file lưu trữ bên dưới.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// - (EN) Gets the default local file path used to store player preferences.
        /// - (VI) Lấy đường dẫn file cục bộ mặc định dùng để lưu tùy chọn người chơi.
        /// </summary>
        public static string GetDefaultFilePath()
        {
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string applicationFolder = Path.Combine(appDataFolder, "MineSweeper");

            return Path.Combine(applicationFolder, "player-preferences.json");
        }

        /// <summary>
        /// - (EN) Loads player preferences from local storage.
        /// Returns default values when the file is missing, invalid, or unreadable.
        /// - (VI) Tải tùy chọn người chơi từ local storage.
        /// Trả về giá trị mặc định nếu file không tồn tại, không hợp lệ, hoặc không thể đọc được.
        /// </summary>
        public PlayerPreferencesStorage Load()
        {
            try
            {
                if (!File.Exists(FilePath))
                {
                    return new PlayerPreferencesStorage();
                }

                string json = File.ReadAllText(FilePath);

                if (string.IsNullOrWhiteSpace(json))
                {
                    return new PlayerPreferencesStorage();
                }

                return JsonSerializer.Deserialize<PlayerPreferencesStorage>(json)
                       ?? new PlayerPreferencesStorage();
            }
            catch
            {
                return new PlayerPreferencesStorage();
            }
        }

        /// <summary>
        /// - (EN) Saves player preferences to local storage.
        /// Persistence failures are intentionally ignored so gameplay is not interrupted.
        /// - (VI) Lưu tùy chọn người chơi vào local storage.
        /// Các lỗi khi lưu được cố ý bỏ qua để không làm gián đoạn trải nghiệm chơi game.
        /// </summary>
        /// <param name="preferences">
        /// - (EN) The player preferences to persist.
        /// - (VI) Tùy chọn người chơi cần được lưu.
        /// </param>
        public void Save(PlayerPreferencesStorage preferences)
        {
            try
            {
                string? directory = Path.GetDirectoryName(FilePath);

                if (!string.IsNullOrWhiteSpace(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string json = JsonSerializer.Serialize(preferences, new JsonSerializerOptions
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
