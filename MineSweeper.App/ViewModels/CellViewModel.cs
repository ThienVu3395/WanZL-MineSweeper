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
    /// Gets the display text shown in the UI for this cell.
    /// </summary>
    public string DisplayText
    {
        get
        {
            // Nếu chưa mở nhưng đã cắm cờ
            if (!IsRevealed && IsFlagged)
                return "🚩";

            // Nếu chưa mở thì không hiển thị gì
            if (!IsRevealed)
                return string.Empty;

            // Nếu là mìn
            if (IsMine)
                return "💣";

            // Nếu không có mìn xung quanh
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