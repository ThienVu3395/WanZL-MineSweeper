namespace MineSweeper.App.Models
{
    /// <summary>
    /// - (EN) Represents persisted player statistics data.
    /// - (VI) Đại diện cho dữ liệu thống kê người chơi được lưu bền vững.
    /// </summary>
    public class PlayerStatisticsStorage
    {
        /// <summary>
        /// - (EN) Gets or sets best completion times in seconds keyed by difficulty name.
        /// - (VI) Lấy hoặc gán best time theo giây, được ánh xạ theo tên độ khó.
        /// </summary>
        public Dictionary<string, double> BestTimesInSeconds { get; set; } = new();
    }
}
