using MineSweeper.App.Models;
using System.IO;
using System.Text.Json;

namespace MineSweeper.App.Services;

/// <summary>
/// - (EN) Provides file-based persistence for an in-progress MineSweeper session.
/// - (VI) Cung cấp cơ chế lưu file cho phiên chơi MineSweeper đang diễn ra.
/// </summary>
public class GameSessionStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    /// <summary>
    /// - (EN) Initializes a new instance of the <see cref="GameSessionStore"/> class.
    /// - (VI) Khởi tạo một instance mới của <see cref="GameSessionStore"/>.
    /// </summary>
    /// <param name="filePath">
    /// - (EN) The full file path used for session persistence.
    /// - (VI) Đường dẫn đầy đủ của file dùng để lưu session.
    /// </param>
    public GameSessionStore(string filePath)
    {
        FilePath = filePath;
    }

    /// <summary>
    /// - (EN) Gets the full file path used by this store.
    /// - (VI) Lấy đường dẫn đầy đủ được store này sử dụng.
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// - (EN) Loads the persisted game session if available and valid.
    /// - (VI) Tải game session đã lưu nếu tồn tại và hợp lệ.
    /// </summary>
    public GameSessionStorage? Load()
    {
        if (!File.Exists(FilePath))
            return null;

        try
        {
            string json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<GameSessionStorage>(json, JsonOptions);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// - (EN) Saves the specified game session to disk.
    /// - (VI) Lưu game session được chỉ định xuống đĩa.
    /// </summary>
    /// <param name="session">
    /// - (EN) The session to persist.
    /// - (VI) Session cần lưu.
    /// </param>
    public void Save(GameSessionStorage session)
    {
        string? directory = Path.GetDirectoryName(FilePath);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string json = JsonSerializer.Serialize(session, JsonOptions);
        File.WriteAllText(FilePath, json);
    }

    /// <summary>
    /// - (EN) Deletes the persisted session file if it exists.
    /// - (VI) Xóa file session đã lưu nếu nó tồn tại.
    /// </summary>
    public void Clear()
    {
        if (File.Exists(FilePath))
        {
            File.Delete(FilePath);
        }
    }

    /// <summary>
    /// - (EN) Gets the default file path for storing an in-progress game session.
    /// - (VI) Lấy đường dẫn file mặc định để lưu phiên chơi đang diễn ra.
    /// </summary>
    public static string GetDefaultFilePath()
    {
        string folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "MineSweeper");

        return Path.Combine(folder, "game-session.json");
    }
}