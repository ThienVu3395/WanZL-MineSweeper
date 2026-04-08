using MineSweeper.Core.Models;

namespace MineSweeper.App.Models;

/// <summary>
/// - (EN) Represents the persisted state of an in-progress MineSweeper session.
/// - (VI) Đại diện cho trạng thái được lưu của một phiên chơi MineSweeper đang diễn ra.
/// </summary>
public class GameSessionStorage
{
    /// <summary>
    /// - (EN) Gets or sets the selected difficulty.
    /// - (VI) Lấy hoặc gán độ khó đang được chọn.
    /// </summary>
    public DifficultyLevel SelectedDifficulty { get; set; }

    /// <summary>
    /// - (EN) Gets or sets the board row count.
    /// - (VI) Lấy hoặc gán số hàng của board.
    /// </summary>
    public int Rows { get; set; }

    /// <summary>
    /// - (EN) Gets or sets the board column count.
    /// - (VI) Lấy hoặc gán số cột của board.
    /// </summary>
    public int Columns { get; set; }

    /// <summary>
    /// - (EN) Gets or sets the configured mine count.
    /// - (VI) Lấy hoặc gán số lượng mìn đã cấu hình.
    /// </summary>
    public int MineCount { get; set; }

    /// <summary>
    /// - (EN) Gets or sets the current game state.
    /// - (VI) Lấy hoặc gán trạng thái hiện tại của game.
    /// </summary>
    public GameState GameState { get; set; }

    /// <summary>
    /// - (EN) Gets or sets whether the first reveal is still pending.
    /// - (VI) Lấy hoặc gán giá trị cho biết lần mở ô đầu tiên còn đang chờ hay không.
    /// </summary>
    public bool IsFirstRevealPending { get; set; }

    /// <summary>
    /// - (EN) Gets or sets the elapsed time in seconds.
    /// - (VI) Lấy hoặc gán thời gian đã chơi tính theo giây.
    /// </summary>
    public int ElapsedTimeInSeconds { get; set; }

    /// <summary>
    /// - (EN) Gets or sets the custom row count.
    /// - (VI) Lấy hoặc gán số hàng custom.
    /// </summary>
    public int CustomRows { get; set; }

    /// <summary>
    /// - (EN) Gets or sets the custom column count.
    /// - (VI) Lấy hoặc gán số cột custom.
    /// </summary>
    public int CustomColumns { get; set; }

    /// <summary>
    /// - (EN) Gets or sets the custom mine count.
    /// - (VI) Lấy hoặc gán số lượng mìn custom.
    /// </summary>
    public int CustomMines { get; set; }

    /// <summary>
    /// - (EN) Gets or sets the persisted cells.
    /// - (VI) Lấy hoặc gán danh sách các ô đã được lưu.
    /// </summary>
    public List<CellSessionStorage> Cells { get; set; } = new();
}