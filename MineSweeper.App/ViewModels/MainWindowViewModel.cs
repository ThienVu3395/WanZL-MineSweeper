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

    private DifficultyLevel _selectedDifficulty;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    public MainWindowViewModel()
    {
        _game = new MineSweeperGame();

        Cells = new ObservableCollection<CellViewModel>();

        // Danh sách độ khó hiển thị trên UI
        AvailableDifficulties = new ObservableCollection<DifficultyLevel>
        {
            DifficultyLevel.Beginner,
            DifficultyLevel.Intermediate,
            DifficultyLevel.Expert
        };

        // Mặc định chọn Beginner
        _selectedDifficulty = DifficultyLevel.Beginner;

        // Command dùng để xử lý khi user click vào cell
        RevealCellCommand = new RelayCommand(OnRevealCell);

        // Command dùng để xử lý khi user right-click vào cell
        ToggleFlagCommand = new RelayCommand(OnToggleFlag);

        NewGameCommand = new RelayCommand(_ => StartNewGameByDifficulty());

        // Khởi tạo game đầu tiên
        StartNewGameByDifficulty();
    }

    /// <summary>
    /// Gets the collection of cells used for UI binding.
    /// </summary>
    public ObservableCollection<CellViewModel> Cells { get; }

    /// <summary>
    /// Gets or sets the currently selected difficulty level.
    /// </summary>
    public DifficultyLevel SelectedDifficulty
    {
        get => _selectedDifficulty;
        set
        {
            if (_selectedDifficulty == value)
                return;

            _selectedDifficulty = value;
            OnPropertyChanged();
        }
    }

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
    /// Command used to toggle flag on a cell (right-click).
    /// </summary>
    public ICommand ToggleFlagCommand { get; }

    /// <summary>
    /// Command used to start a new game with the selected difficulty.
    /// </summary>
    public ICommand NewGameCommand { get; }

    /// <summary>
    /// Gets the available difficulty levels for the UI.
    /// </summary>
    public ObservableCollection<DifficultyLevel> AvailableDifficulties { get; }

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
    /// Starts a new game based on the selected difficulty level.
    /// </summary>
    private void StartNewGameByDifficulty()
    {
        // Map độ khó sang cấu hình board
        switch (SelectedDifficulty)
        {
            case DifficultyLevel.Beginner:
                StartNewGame(9, 9, 10);
                break;

            case DifficultyLevel.Intermediate:
                StartNewGame(16, 16, 40);
                break;

            case DifficultyLevel.Expert:
                StartNewGame(16, 30, 99);
                break;

            default:
                StartNewGame(9, 9, 10);
                break;
        }
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

    /// <summary>
    /// Handles the right-click flag toggle action.
    /// </summary>
    private void OnToggleFlag(object? parameter)
    {
        if (parameter is not CellViewModel cellVm)
            return;

        _game.ToggleFlag(cellVm.Row, cellVm.Column);

        RefreshBoard();
    }
}