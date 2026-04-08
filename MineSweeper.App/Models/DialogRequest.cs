namespace MineSweeper.App.Models;

/// <summary>
/// - (EN) Represents reusable dialog content for window-level UI notifications.
/// - (VI) Đại diện cho nội dung dialog có thể tái sử dụng cho các thông báo ở mức cửa sổ từ UI.
/// </summary>
public class DialogRequest
{
    /// <summary>
    /// - (EN) Gets or sets the dialog title.
    /// - (VI) Lấy hoặc gán tiêu đề dialog.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// - (EN) Gets or sets the dialog message.
    /// - (VI) Lấy hoặc gán nội dung dialog.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}