using System.ComponentModel;
using System.Runtime.CompilerServices;
using MineSweeper.Core.Models;

namespace MineSweeper.App.ViewModels;

/// <summary>
/// - (EN) Represents the UI state of a single cell on the MineSweeper board and acts as a bridge between the domain model (Cell) and the WPF UI.
/// - (VI) Đại diện trạng thái UI của một ô trên bàn chơi MineSweeper và đóng vai trò trung gian giữa domain model (Cell) và giao diện WPF.
/// </summary>
public class CellViewModel : INotifyPropertyChanged
{
    private readonly Cell _cell;

    //// <summary>
    /// - (EN) Initializes a new instance of the CellViewModel class with the specified domain cell.
    /// - (VI) Khởi tạo một instance mới của CellViewModel với domain cell được chỉ định.
    /// </summary>
    public CellViewModel(Cell cell)
    {
        _cell = cell;
    }

    /// <summary>
    /// - (EN) Gets the row index of the cell.
    /// - (VI) Lấy chỉ số hàng của ô.
    /// </summary>
    public int Row => _cell.Row;

    /// <summary>
    /// - (EN) Gets the column index of the cell.
    /// - (VI) Lấy chỉ số cột của ô.
    /// </summary>
    public int Column => _cell.Column;

    /// <summary>
    /// - (EN) Gets whether the cell has been revealed.
    /// - (VI) Lấy giá trị cho biết ô đã được mở hay chưa.
    /// </summary>
    public bool IsRevealed => _cell.IsRevealed;

    /// <summary>
    /// - (EN) Gets whether the cell is flagged.
    /// - (VI) Lấy giá trị cho biết ô có đang được đánh dấu cờ hay không.
    /// </summary>
    public bool IsFlagged => _cell.IsFlagged;

    /// <summary>
    /// - (EN) Gets whether the cell contains a mine.
    /// - (VI) Lấy giá trị cho biết ô có chứa mìn hay không.
    /// </summary>
    public bool IsMine => _cell.IsMine;

    /// <summary>
    /// - (EN) Gets whether the cell is hidden and not yet revealed.
    /// - (VI) Lấy giá trị cho biết ô đang bị ẩn và chưa được mở.
    /// </summary>
    public bool IsHidden => !IsRevealed;

    /// <summary>
    /// - (EN) Gets the number of adjacent mines.
    /// - (VI) Lấy số lượng mìn ở các ô lân cận.
    /// </summary>
    public int AdjacentMines => _cell.AdjacentMines;

    /// <summary>
    /// - (EN) Gets whether this cell is the exploded mine that caused the loss.
    /// - (VI) Lấy giá trị cho biết ô này có phải là ô mìn phát nổ gây thua game hay không.
    /// </summary>
    public bool IsExplodedMine => _cell.IsExplodedMine;

    /// <summary>
    /// - (EN) Gets whether this flagged cell is marked as incorrect after the game ends.
    /// - (VI) Lấy giá trị cho biết ô bị cắm cờ này có bị đánh dấu là cờ sai sau khi game kết thúc hay không.
    /// </summary>
    public bool IsIncorrectFlag => _cell.IsIncorrectFlag;

    /// <summary>
    /// - (EN) Gets the display text shown in the UI for this cell.
    /// - (VI) Lấy nội dung hiển thị trên giao diện cho ô này.
    /// </summary>
    public string DisplayText
    {
        get
        {
            // - (EN) Show incorrect flag marker after game over
            // - (VI) Hiển thị dấu cờ sai sau khi game kết thúc
            if (IsIncorrectFlag)
                return "❌";

            // - (EN) Show the exploded mine with a stronger symbol
            // - (VI) Hiển thị ô mìn phát nổ bằng biểu tượng nổi bật hơn
            if (IsExplodedMine)
                return "💥";

            // - (EN) Show flag while the cell remains hidden
            // - (VI) Hiển thị cờ khi ô vẫn còn đang ẩn
            if (!IsRevealed && IsFlagged)
                return "🚩";

            // - (EN) Hidden cells show no text
            // - (VI) Các ô chưa mở sẽ không hiển thị nội dung
            if (!IsRevealed)
                return string.Empty;

            // - (EN) Revealed mine
            // - (VI) Ô mìn đã được mở
            if (IsMine)
                return "💣";

            // - (EN) Empty revealed safe cell
            // - (VI) Ô an toàn đã mở và không có mìn lân cận
            if (AdjacentMines == 0)
                return string.Empty;

            return AdjacentMines.ToString();
        }
    }

    /// <summary>
    /// - (EN) Refreshes all UI-related properties of the cell and notifies the UI of changes.
    /// - (VI) Làm mới tất cả các thuộc tính liên quan đến UI của ô và thông báo thay đổi cho giao diện.
    /// </summary>
    public void Refresh()
    {
        OnPropertyChanged(nameof(IsRevealed));
        OnPropertyChanged(nameof(IsHidden));
        OnPropertyChanged(nameof(IsFlagged));
        OnPropertyChanged(nameof(IsMine));
        OnPropertyChanged(nameof(IsExplodedMine));
        OnPropertyChanged(nameof(IsIncorrectFlag));
        OnPropertyChanged(nameof(AdjacentMines));
        OnPropertyChanged(nameof(DisplayText));
    }

    /// <summary>
    /// - (EN) Occurs when a property value changes.
    /// - (VI) Xảy ra khi giá trị của một thuộc tính thay đổi.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// - (EN) Raises the PropertyChanged event for the specified property.
    /// - (VI) Kích hoạt sự kiện PropertyChanged cho thuộc tính được chỉ định.
    /// </summary>
    /// <param name="propertyName">
    /// - (EN) The name of the property that changed.
    /// - (VI) Tên của thuộc tính đã thay đổi.
    /// </param>
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}