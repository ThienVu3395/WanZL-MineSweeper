using System.ComponentModel;
using System.Runtime.CompilerServices;
using MineSweeper.Core.Models;

namespace MineSweeper.App.ViewModels;

/// <summary>
/// Represents the UI state of a single cell on the MineSweeper board.
/// Acts as a bridge between the domain model (Cell) and the WPF UI.
/// </summary>
public class CellViewModel : INotifyPropertyChanged
{
    private readonly Cell _cell;

    /// <summary>
    /// Initializes a new instance of the <see cref="CellViewModel"/> class.
    /// </summary>
    /// <param name="cell">The underlying domain cell.</param>
    public CellViewModel(Cell cell)
    {
        _cell = cell;
    }

    /// <summary>
    /// Gets the row index of the cell.
    /// </summary>
    public int Row => _cell.Row;

    /// <summary>
    /// Gets the column index of the cell.
    /// </summary>
    public int Column => _cell.Column;

    /// <summary>
    /// Gets whether the cell has been revealed.
    /// </summary>
    public bool IsRevealed => _cell.IsRevealed;

    /// <summary>
    /// Gets whether the cell is flagged.
    /// </summary>
    public bool IsFlagged => _cell.IsFlagged;

    /// <summary>
    /// Gets whether the cell contains a mine.
    /// </summary>
    public bool IsMine => _cell.IsMine;

    /// <summary>
    /// Gets whether the cell is hidden and not yet revealed.
    /// </summary>
    public bool IsHidden => !IsRevealed;

    /// <summary>
    /// Gets the number of adjacent mines.
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
    /// Refreshes all UI-related properties of the cell.
    /// Should be called after any game state change.
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

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises property changed notification.
    /// </summary>
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}