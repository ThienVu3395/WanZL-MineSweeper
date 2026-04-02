using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MineSweeper.App.Helpers;
using MineSweeper.Core.Models;
using MineSweeper.Core.Services;

namespace MineSweeper.App.ViewModels;

/// <summary>
/// Represents the main view model for the MineSweeper window.
/// Responsible for exposing game data and handling user interactions.
/// </summary>
public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly MineSweeperGame _game;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    public MainWindowViewModel()
    {
        _game = new MineSweeperGame();

        Cells = new ObservableCollection<CellViewModel>();

        // Command dùng để xử lý khi user click vào cell
        RevealCellCommand = new RelayCommand(OnRevealCell);

        // Khởi tạo game mặc định
        StartNewGame(9, 9, 10);
    }

    /// <summary>
    /// Gets the collection of cells used for UI binding.
    /// </summary>
    public ObservableCollection<CellViewModel> Cells { get; }

    /// <summary>
    /// Gets the number of rows of the current board.
    /// </summary>
    public int Rows => _game.Board?.Rows ?? 0;

    /// <summary>
    /// Gets the number of columns of the current board.
    /// </summary>
    public int Columns => _game.Board?.Columns ?? 0;

    /// <summary>
    /// Gets the current game status as a string for UI display.
    /// </summary>
    public string GameStatus => _game.State.ToString();

    /// <summary>
    /// Command used to reveal a cell when user clicks on it.
    /// </summary>
    public ICommand RevealCellCommand { get; }

    /// <summary>
    /// Starts a new game and rebuilds the UI cell collection.
    /// </summary>
    /// <param name="rows">Number of rows.</param>
    /// <param name="columns">Number of columns.</param>
    /// <param name="mineCount">Number of mines.</param>
    public void StartNewGame(int rows, int columns, int mineCount)
    {
        // Gọi logic từ Core
        _game.StartNewGame(rows, columns, mineCount);

        // Clear UI cũ
        Cells.Clear();

        // Build lại danh sách cell cho UI
        foreach (var cell in _game.Board!.Cells)
        {
            Cells.Add(new CellViewModel(cell));
        }

        // Notify UI update
        OnPropertyChanged(nameof(Rows));
        OnPropertyChanged(nameof(Columns));
        OnPropertyChanged(nameof(GameStatus));
    }

    /// <summary>
    /// Handles the reveal cell action triggered from the UI.
    /// </summary>
    /// <param name="parameter">The clicked cell view model.</param>
    private void OnRevealCell(object? parameter)
    {
        if (parameter is not CellViewModel cellVm)
            return;

        // Nếu game đã kết thúc thì không cho click nữa
        if (_game.State == GameState.Won || _game.State == GameState.Lost)
            return;

        // Gọi logic reveal từ core
        _game.RevealCell(cellVm.Row, cellVm.Column);

        // Refresh lại toàn bộ UI
        RefreshBoard();

        // Update trạng thái game (Won / Lost)
        OnPropertyChanged(nameof(GameStatus));
    }

    /// <summary>
    /// Refreshes all cell view models to reflect updated game state.
    /// </summary>
    private void RefreshBoard()
    {
        foreach (var cell in Cells)
        {
            cell.Refresh();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises property changed notification for UI binding.
    /// </summary>
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}