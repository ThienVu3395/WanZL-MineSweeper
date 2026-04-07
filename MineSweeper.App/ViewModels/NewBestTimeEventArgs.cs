using MineSweeper.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper.App.ViewModels
{
    /// <summary>
    /// - (EN) Provides data for a notification raised when a new best time is achieved.
    /// - (VI) Cung cấp dữ liệu cho thông báo được phát ra khi đạt được best time mới.
    /// </summary>
    public class NewBestTimeEventArgs : EventArgs
    {
        /// <summary>
        /// - (EN) Initializes a new instance of the <see cref="NewBestTimeEventArgs"/> class.
        /// - (VI) Khởi tạo một instance mới của <see cref="NewBestTimeEventArgs"/>.
        /// </summary>
        /// <param name="difficulty">
        /// - (EN) The difficulty where the new record was achieved.
        /// - (VI) Độ khó mà kỷ lục mới được thiết lập.
        /// </param>
        /// <param name="bestTime">
        /// - (EN) The newly achieved best completion time.
        /// - (VI) Thời gian hoàn thành tốt nhất mới đạt được.
        /// </param>
        /// <param name="isFirstRecord">
        /// - (EN) Indicates whether this is the first recorded best time for the difficulty.
        /// - (VI) Cho biết đây có phải là best time đầu tiên được ghi nhận cho độ khó này hay không.
        /// </param>
        public NewBestTimeEventArgs(DifficultyLevel difficulty, TimeSpan bestTime, bool isFirstRecord)
        {
            Difficulty = difficulty;
            BestTime = bestTime;
            IsFirstRecord = isFirstRecord;
        }

        /// <summary>
        /// - (EN) Gets the difficulty where the new record was achieved.
        /// - (VI) Lấy độ khó mà kỷ lục mới được thiết lập.
        /// </summary>
        public DifficultyLevel Difficulty { get; }

        /// <summary>
        /// - (EN) Gets the newly achieved best completion time.
        /// - (VI) Lấy thời gian hoàn thành tốt nhất mới đạt được.
        /// </summary>
        public TimeSpan BestTime { get; }

        /// <summary>
        /// - (EN) Gets whether this is the first recorded best time for the difficulty.
        /// - (VI) Lấy giá trị cho biết đây có phải là best time đầu tiên của độ khó này hay không.
        /// </summary>
        public bool IsFirstRecord { get; }
    }
}
