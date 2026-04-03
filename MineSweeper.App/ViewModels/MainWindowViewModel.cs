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
    private readonly RelayCommand _revealCellCommand;
    private readonly RelayCommand _toggleFlagCommand;
    private readonly RelayCommand _newGameCommand;

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
        _revealCellCommand = new RelayCommand(OnRevealCell, CanRevealCell);

        // Command dùng để xử lý khi user right-click vào cell
        _toggleFlagCommand = new RelayCommand(OnToggleFlag, CanToggleFlag);

        // Command dùng để bắt đầu game mới
        _newGameCommand = new RelayCommand(_ => StartNewGameByDifficulty());

        // Khởi tạo game đầu tiên
        StartNewGameByDifficulty();
    }

    /// <summary>
    /// Gets the collection of cells used for UI binding.
    /// </summary>
    public ObservableCollection<CellViewModel> Cells { get; }

    /// <summary>
    /// Gets the available difficulty levels for the UI.
    /// </summary>
    public ObservableCollection<DifficultyLevel> AvailableDifficulties { get; }

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
    /// Gets whether the game is finished.
    /// </summary>
    public bool IsGameFinished => _game.State == GameState.Won || _game.State == GameState.Lost;

    /// <summary>
    /// Gets the total number of mines on the current board.
    /// </summary>
    public int TotalMines => _game.Board?.MineCount ?? 0;

    /// <summary>
    /// Gets the number of currently flagged cells.
    /// </summary>
    public int FlagCount => Cells.Count(cell => cell.IsFlagged);

    /// <summary>
    /// Gets the estimated number of mines remaining based on placed flags.
    /// </summary>
    public int RemainingMines => TotalMines - FlagCount;

    private string? _message;

    /// <summary>
    /// Gets or sets temporary UI message (e.g., warnings).
    /// </summary>
    public string? Message
    {
        get => _message;
        private set
        {
            _message = value;
            OnPropertyChanged();
        }
    }

    private void ShowFlagLimitMessage()
    {
        Message = null;
        Message = "⚠️ All flags are used!";
    }

    /// <summary>
    /// Gets the current game status text for UI display.
    /// </summary>
    public string GameStatus
    {
        get
        {
            return _game.State switch
            {
                GameState.NotStarted => "Game not started",
                GameState.InProgress => "Game in progress",
                GameState.Won => "🎉 You won!",
                GameState.Lost => "💥 You hit a mine!",
                _ => string.Empty
            };
        }
    }

    /// <summary>
    /// Command used to reveal a cell when user clicks on it.
    /// </summary>
    public ICommand RevealCellCommand => _revealCellCommand;

    /// <summary>
    /// Command used to toggle flag on a cell (right-click).
    /// </summary>
    public ICommand ToggleFlagCommand => _toggleFlagCommand;

    /// <summary>
    /// Command used to start a new game with the selected difficulty.
    /// </summary>
    public ICommand NewGameCommand => _newGameCommand;

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

        Message = null;

        // Notify UI update
        OnPropertyChanged(nameof(Rows));
        OnPropertyChanged(nameof(Columns));
        OnPropertyChanged(nameof(GameStatus));
        OnPropertyChanged(nameof(IsGameFinished));
        OnPropertyChanged(nameof(TotalMines));
        OnPropertyChanged(nameof(FlagCount));
        OnPropertyChanged(nameof(RemainingMines));

        // Cập nhật trạng thái enable/disable của command
        _revealCellCommand.RaiseCanExecuteChanged();
        _toggleFlagCommand.RaiseCanExecuteChanged();
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
    /// Gets the description of the selected difficulty (board size and mine count).
    /// </summary>
    public string DifficultyDescription
    {
        get
        {
            return SelectedDifficulty switch
            {
                DifficultyLevel.Beginner => "9 x 9 | 10 mines",
                DifficultyLevel.Intermediate => "16 x 16 | 40 mines",
                DifficultyLevel.Expert => "16 x 30 | 99 mines",
                _ => string.Empty
            };
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

        _game.RevealCell(cellVm.Row, cellVm.Column);

        RefreshBoard();
        OnPropertyChanged(nameof(GameStatus));
        OnPropertyChanged(nameof(IsGameFinished));
        OnPropertyChanged(nameof(FlagCount));
        OnPropertyChanged(nameof(RemainingMines));

        // Cập nhật trạng thái enable/disable của command
        _revealCellCommand.RaiseCanExecuteChanged();
        _toggleFlagCommand.RaiseCanExecuteChanged();

        // Nếu game kết thúc thì hiển thị message
        if (_game.State == GameState.Won)
        {
            System.Windows.MessageBox.Show("Congratulations! You cleared all cells!", "Victory 🎉");
        }
        else if (_game.State == GameState.Lost)
        {
            System.Windows.MessageBox.Show("Boom! You hit a mine.", "Game Over 💥");
        }
    }

    /// <summary>
    /// Handles the right-click flag toggle action.
    /// </summary>
    private void OnToggleFlag(object? parameter)
    {
        if (parameter is not CellViewModel cellVm)
            return;

        // Không cho flag nếu đã đạt max
        if (!cellVm.IsFlagged && FlagCount >= TotalMines)
        {
            ShowFlagLimitMessage();
            return;
        }

        // clear message
        Message = null;

        _game.ToggleFlag(cellVm.Row, cellVm.Column);

        RefreshBoard();
        OnPropertyChanged(nameof(FlagCount));
        OnPropertyChanged(nameof(RemainingMines));
        OnPropertyChanged(nameof(GameStatus));
        OnPropertyChanged(nameof(IsGameFinished));

        // Cập nhật trạng thái command sau khi toggle flag
        _revealCellCommand.RaiseCanExecuteChanged();
        _toggleFlagCommand.RaiseCanExecuteChanged();
    }

    /// <summary>
    /// Determines whether a cell can be revealed.
    /// Prevents interaction when the game is finished.
    /// </summary>
    private bool CanRevealCell(object? parameter)
    {
        return _game.State == GameState.InProgress;
    }

    /// <summary>
    /// Determines whether a cell can be flagged.
    /// Prevents interaction when the game is finished.
    /// </summary>
    private bool CanToggleFlag(object? parameter)
    {
        return _game.State == GameState.InProgress;
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