namespace MineSweeper.App.Models;

/// <summary>
/// - (EN) Represents the persisted best-time data stored on disk.
/// - (VI) Đại diện cho dữ liệu best time được lưu bền vững trên ổ đĩa.
/// </summary>
public class BestTimesStorage
{
    /// <summary>
    /// - (EN) Gets or sets the best times in total seconds keyed by difficulty name.
    /// - (VI) Lấy hoặc gán danh sách best time theo tổng số giây, được ánh xạ theo tên độ khó.
    /// </summary>
    public Dictionary<string, double> BestTimesInSeconds { get; set; } = new();
}