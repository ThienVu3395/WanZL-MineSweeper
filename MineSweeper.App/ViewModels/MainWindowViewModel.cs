using MineSweeper.App.Extensions;
using MineSweeper.App.Helpers;
using MineSweeper.Core.Models;
using MineSweeper.Core.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;

namespace MineSweeper.App.ViewModels;

/// <summary>
/// - (EN) Represents the main view model for the MineSweeper window.
/// Responsible for exposing game data and handling user interactions.
/// - (VI) Đại diện cho view model chính của cửa sổ MineSweeper.
/// Chịu trách nhiệm cung cấp dữ liệu game cho UI và xử lý các tương tác của người dùng.
/// </summary>
public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly MineSweeperGame _game;

    private readonly RelayCommand _revealCellCommand;
    private readonly RelayCommand _toggleFlagCommand;
    private readonly RelayCommand _newGameCommand;
    private readonly RelayCommand _chordCellCommand;

    private readonly DispatcherTimer _gameTimer;
    private DateTime? _gameStartTimeUtc;
    private TimeSpan _elapsedTime;
    private bool _isTimerRunning;

    private string? _message;
    private DifficultyLevel _selectedDifficulty;

    private readonly Dictionary<DifficultyLevel, TimeSpan> _bestTimes;

    /// <summary>
    /// - (EN) Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// - (VI) Khởi tạo một instance mới của <see cref="MainWindowViewModel"/>.
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

        // Command dùng để xử lý chording khi user double-click vào ô đã mở
        _chordCellCommand = new RelayCommand(OnChordCell, CanChordCell);

        // Timer dùng để cập nhật thời gian chơi theo từng giây
        _gameTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        _gameTimer.Tick += OnGameTimerTick;

        // Hiển thị danh sách best times theo độ khó của game
        _bestTimes = new Dictionary<DifficultyLevel, TimeSpan>();

        // Khởi tạo game đầu tiên
        StartNewGameByDifficulty();
    }

    #region Commands
    /// <summary>
    /// - (EN) Gets the command used to reveal a cell when the user clicks on it.
    /// - (VI) Lấy command dùng để mở một ô khi người dùng click vào ô đó.
    /// </summary>
    public ICommand RevealCellCommand => _revealCellCommand;

    /// <summary>
    /// - (EN) Gets the command used to toggle a flag on a cell when the user right-clicks.
    /// - (VI) Lấy command dùng để bật hoặc tắt cờ trên một ô khi người dùng nhấn chuột phải.
    /// </summary>
    public ICommand ToggleFlagCommand => _toggleFlagCommand;

    /// <summary>
    /// - (EN) Gets the command used to start a new game with the selected difficulty.
    /// - (VI) Lấy command dùng để bắt đầu một ván mới theo độ khó đang được chọn.
    /// </summary>
    public ICommand NewGameCommand => _newGameCommand;

    /// <summary>
    /// - (EN) Gets the command used to perform chording on a revealed cell.
    /// - (VI) Lấy command dùng để thực hiện thao tác chording trên một ô đã mở.
    /// </summary>
    public ICommand ChordCellCommand => _chordCellCommand;
    #endregion

    #region Bindable Properties
    /// <summary>
    /// - (EN) Gets the collection of cells used for UI binding.
    /// - (VI) Lấy tập hợp các ô được dùng để bind dữ liệu lên giao diện.
    /// </summary>
    public ObservableCollection<CellViewModel> Cells { get; }

    /// <summary>
    /// - (EN) Gets the available difficulty levels displayed on the UI.
    /// - (VI) Lấy danh sách các mức độ khó được hiển thị trên giao diện.
    /// </summary>
    public ObservableCollection<DifficultyLevel> AvailableDifficulties { get; }

    /// <summary>
    /// - (EN) Gets or sets the currently selected difficulty level.
    /// - (VI) Lấy hoặc gán mức độ khó hiện đang được chọn.
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
            OnPropertyChanged(nameof(BestTime));
            OnPropertyChanged(nameof(BestTimeDisplay));
        }
    }

    /// <summary>
    /// - (EN) Gets the number of rows of the current board.
    /// - (VI) Lấy số hàng của board hiện tại.
    /// </summary>
    public int Rows => _game.Board?.Rows ?? 0;

    /// <summary>
    /// - (EN) Gets the number of columns of the current board.
    /// - (VI) Lấy số cột của board hiện tại.
    /// </summary>
    public int Columns => _game.Board?.Columns ?? 0;

    /// <summary>
    /// - (EN) Gets whether the game has finished.
    /// - (VI) Lấy giá trị cho biết trò chơi đã kết thúc hay chưa.
    /// </summary>
    public bool IsGameFinished => _game.State == GameState.Won || _game.State == GameState.Lost;

    /// <summary>
    /// - (EN) Gets the total number of mines on the current board.
    /// - (VI) Lấy tổng số mìn trên board hiện tại.
    /// </summary>
    public int TotalMines => _game.Board?.MineCount ?? 0;

    /// <summary>
    /// - (EN) Gets the number of currently flagged cells.
    /// - (VI) Lấy số ô hiện đang được cắm cờ.
    /// </summary>
    public int FlagCount => Cells.Count(cell => cell.IsFlagged);

    /// <summary>
    /// - (EN) Gets the estimated number of mines remaining based on placed flags.
    /// - (VI) Lấy số mìn còn lại được ước tính dựa trên số cờ đã đặt.
    /// </summary>
    public int RemainingMines => TotalMines - FlagCount;

    /// <summary>
    /// - (EN) Gets or sets the temporary UI message, such as warnings.
    /// - (VI) Lấy hoặc gán thông báo tạm thời trên giao diện, ví dụ như cảnh báo.
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

    /// <summary>
    /// - (EN) Gets the current game status text for UI display.
    /// - (VI) Lấy chuỗi trạng thái hiện tại của game để hiển thị trên giao diện.
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
    /// - (EN) Gets the elapsed gameplay duration.
    /// - (VI) Lấy tổng thời gian đã chơi trong ván hiện tại.
    /// </summary>
    public TimeSpan ElapsedTime => _elapsedTime;

    /// <summary>
    /// - (EN) Gets the formatted elapsed gameplay duration for UI display.
    /// - (VI) Lấy chuỗi thời gian đã chơi đã được định dạng để hiển thị trên giao diện.
    /// </summary>
    public string ElapsedTimeDisplay => _elapsedTime.ToString(@"mm\:ss");

    /// <summary>
    /// - (EN) Gets the best recorded completion time for the currently selected difficulty.
    /// - (VI) Lấy thời gian hoàn thành tốt nhất đã ghi nhận cho độ khó hiện đang được chọn.
    /// </summary>
    public TimeSpan? BestTime
    {
        get
        {
            return _bestTimes.TryGetValue(SelectedDifficulty, out var value)
                ? value
                : null;
        }
    }

    /// <summary>
    /// - (EN) Gets the formatted best completion time for UI display.
    /// - (VI) Lấy chuỗi thời gian tốt nhất đã được định dạng để hiển thị trên giao diện.
    /// </summary>
    public string BestTimeDisplay => BestTime?.ToString(@"mm\:ss") ?? "--:--";
    #endregion

    #region Events
    /// <summary>
    /// - (EN) Occurs when a property value changes.
    /// - (VI) Xảy ra khi giá trị của một thuộc tính thay đổi.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// - (EN) Occurs when the game reaches an end state such as win or loss.
    /// - (VI) Xảy ra khi game đạt đến trạng thái kết thúc như thắng hoặc thua.
    /// </summary>
    public event EventHandler<GameEndedEventArgs>? GameEnded;
    #endregion

    #region Public Methods
    /// <summary>
    /// - (EN) Starts a new game and rebuilds the UI cell collection.
    /// - (VI) Bắt đầu một ván mới và dựng lại danh sách ô cho giao diện.
    /// </summary>
    /// <param name="rows">Number of rows / Số hàng</param>
    /// <param name="columns">Number of columns / Số cột</param>
    /// <param name="mineCount">Number of mines / Số lượng mìn</param>
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

        ResetTimer();
        ClearMessage();
        RefreshGameProperties(includeBoardDimensions: true, includeTotalMines: true);
        RefreshCommandStates();
    }
    #endregion

    #region Command Handlers
    /// <summary>
    /// - (EN) Handles the reveal cell action triggered from the UI.
    /// - (VI) Xử lý thao tác mở ô được gọi từ giao diện.
    /// </summary>
    /// <param name="parameter">The clicked cell view model / CellViewModel được click</param>
    private void OnRevealCell(object? parameter)
    {
        if (parameter is not CellViewModel cellVm)
            return;

        bool shouldStartTimer = !_isTimerRunning && !cellVm.IsRevealed && !cellVm.IsFlagged;

        ClearMessage();

        if (shouldStartTimer)
        {
            StartTimer();
        }

        _game.RevealCell(cellVm.Row, cellVm.Column);

        RefreshBoardState();
        HandleEndGameNotification();
    }

    /// <summary>
    /// - (EN) Handles the right-click flag toggle action.
    /// - (VI) Xử lý thao tác bật/tắt cờ khi người dùng right-click.
    /// </summary>
    /// <param name="parameter">The clicked cell view model / CellViewModel được thao tác</param>
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

        ClearMessage();

        _game.ToggleFlag(cellVm.Row, cellVm.Column);

        RefreshBoardState();
    }

    /// <summary>
    /// - (EN) Handles the chord action triggered from the UI.
    /// - (VI) Xử lý thao tác chord được gọi từ giao diện.
    /// </summary>
    /// <param name="parameter">
    /// - (EN) The target cell view model.
    /// - (VI) CellViewModel của ô được thao tác.
    /// </param>
    private void OnChordCell(object? parameter)
    {
        if (parameter is not CellViewModel cellVm)
            return;

        ClearMessage();

        _game.ChordCell(cellVm.Row, cellVm.Column);

        RefreshBoardState();
        HandleEndGameNotification();
    }
    #endregion

    #region Command Conditions
    /// <summary>
    /// - (EN) Determines whether a cell can be revealed. Prevents interaction when the game is finished.
    /// - (VI) Xác định một ô có thể được mở hay không. Ngăn thao tác khi trò chơi đã kết thúc.
    /// </summary>
    /// <param name="parameter">- (EN) Command parameter / (VI) Tham số của command</param>
    /// <returns>- (EN) True if revealing is allowed / (VI) True nếu được phép mở ô</returns>
    private bool CanRevealCell(object? parameter)
    {
        return _game.State == GameState.InProgress;
    }

    /// <summary>
    /// - (EN) Determines whether a cell can be flagged. Prevents interaction when the game is finished.
    /// - (VI) Xác định một ô có thể được cắm cờ hay không. Ngăn thao tác khi trò chơi đã kết thúc.
    /// </summary>
    /// <param name="parameter">- (EN) Command parameter / (VI) Tham số của command</param>
    /// <returns>- (EN) True if flagging is allowed / (VI) True nếu được phép cắm cờ</returns>
    private bool CanToggleFlag(object? parameter)
    {
        return _game.State == GameState.InProgress;
    }

    /// <summary>
    /// - (EN) Determines whether chording is allowed for the selected cell.
    /// - (VI) Xác định ô được chọn có thể thực hiện chording hay không.
    /// </summary>
    /// <param name="parameter">
    /// - (EN) The target cell view model.
    /// - (VI) CellViewModel của ô cần kiểm tra.
    /// </param>
    /// <returns>- (EN) True if chording is allowed / (VI) True nếu được phép chording</returns>
    private bool CanChordCell(object? parameter)
    {
        if (_game.State != GameState.InProgress)
            return false;

        if (parameter is not CellViewModel cellVm)
            return false;

        return cellVm.IsRevealed;
    }
    #endregion

    #region Game Setup Helpers
    /// <summary>
    /// - (EN) Starts a new game based on the currently selected difficulty level.
    /// - (VI) Bắt đầu một ván mới dựa trên mức độ khó hiện đang được chọn.
    /// </summary>
    private void StartNewGameByDifficulty()
    {
        var preset = SelectedDifficulty.ToPreset();
        StartNewGame(preset.Rows, preset.Columns, preset.MineCount);
    }
    #endregion

    #region Notification Helpers
    /// <summary>
    /// - (EN) Shows a warning message when all available flags have already been used.
    /// - (VI) Hiển thị thông báo cảnh báo khi toàn bộ số cờ cho phép đã được sử dụng hết.
    /// </summary>
    private void ShowFlagLimitMessage()
    {
        Message = null;
        Message = "⚠️ All flags are used!";
    }

    /// <summary>
    /// - (EN) Raises a property changed notification for UI binding.
    /// - (VI) Phát thông báo thay đổi thuộc tính để cập nhật binding trên giao diện.
    /// </summary>
    /// <param name="propertyName">
    /// - (EN) The name of the changed property.
    /// - (VI) Tên của thuộc tính đã thay đổi.
    /// </param>
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion

    #region Private Helpers
    /// <summary>
    /// - (EN) Clears the temporary UI message.
    /// - (VI) Xóa thông báo tạm thời trên giao diện.
    /// </summary>
    private void ClearMessage()
    {
        Message = null;
    }

    /// <summary>
    /// - (EN) Refreshes all cell view models to reflect updated game state.
    /// - (VI) Làm mới toàn bộ CellViewModel để phản ánh trạng thái game mới nhất.
    /// </summary>
    private void RefreshBoard()
    {
        foreach (var cell in Cells)
        {
            cell.Refresh();
        }
    }

    /// <summary>
    /// - (EN) Refreshes the board and all related UI state after a gameplay action.
    /// - (VI) Làm mới board và toàn bộ trạng thái UI liên quan sau một thao tác gameplay.
    /// </summary>
    private void RefreshBoardState()
    {
        RefreshBoard();
        RefreshGameProperties();
        RefreshCommandStates();
    }

    /// <summary>
    /// - (EN) Raises property change notifications for game-related UI properties.
    /// - (VI) Phát thông báo thay đổi cho các thuộc tính UI liên quan đến trạng thái game.
    /// </summary>
    /// <param name="includeBoardDimensions">
    /// - (EN) Whether to raise board dimension properties.
    /// - (VI) Có raise thêm các thuộc tính kích thước board hay không.
    /// </param>
    /// <param name="includeTotalMines">
    /// - (EN) Whether to raise total mine count property.
    /// - (VI) Có raise thêm thuộc tính tổng số mìn hay không.
    /// </param>
    private void RefreshGameProperties(bool includeBoardDimensions = false, bool includeTotalMines = false)
    {
        if (includeBoardDimensions)
        {
            OnPropertyChanged(nameof(Rows));
            OnPropertyChanged(nameof(Columns));
        }

        if (includeTotalMines)
        {
            OnPropertyChanged(nameof(TotalMines));
        }

        OnPropertyChanged(nameof(GameStatus));
        OnPropertyChanged(nameof(IsGameFinished));
        OnPropertyChanged(nameof(FlagCount));
        OnPropertyChanged(nameof(RemainingMines));
        OnPropertyChanged(nameof(ElapsedTime));
        OnPropertyChanged(nameof(ElapsedTimeDisplay));
        OnPropertyChanged(nameof(BestTime));
        OnPropertyChanged(nameof(BestTimeDisplay));
    }

    /// <summary>
    /// - (EN) Forces all gameplay commands to re-evaluate their executable state.
    /// - (VI) Yêu cầu tất cả command gameplay đánh giá lại trạng thái có thể thực thi.
    /// </summary>
    private void RefreshCommandStates()
    {
        _revealCellCommand.RaiseCanExecuteChanged();
        _toggleFlagCommand.RaiseCanExecuteChanged();
        _chordCellCommand.RaiseCanExecuteChanged();
    }

    /// <summary>
    /// - (EN) Raises a game-ended notification when the current game has been won or lost.
    /// - (VI) Phát thông báo kết thúc game khi người chơi đã thắng hoặc thua.
    /// </summary>
    private void HandleEndGameNotification()
    {
        if (_game.State == GameState.Won || _game.State == GameState.Lost)
        {
            StopTimer();
            UpdateBestTimeIfNeeded();
            GameEnded?.Invoke(this, new GameEndedEventArgs(_game.State));
        }
    }

    /// <summary>
    /// - (EN) Starts the gameplay timer if it is not already running.
    /// - (VI) Khởi động timer của ván chơi nếu timer chưa chạy.
    /// </summary>
    private void StartTimer()
    {
        if (_isTimerRunning)
            return;

        _gameStartTimeUtc = DateTime.UtcNow - _elapsedTime;
        _gameTimer.Start();
        _isTimerRunning = true;

        UpdateElapsedTime();
    }

    /// <summary>
    /// - (EN) Stops the gameplay timer if it is currently running.
    /// - (VI) Dừng timer của ván chơi nếu timer đang chạy.
    /// </summary>
    private void StopTimer()
    {
        if (!_isTimerRunning)
            return;

        UpdateElapsedTime();
        _gameTimer.Stop();
        _isTimerRunning = false;
    }

    /// <summary>
    /// - (EN) Resets the gameplay timer back to zero.
    /// - (VI) Đặt lại timer của ván chơi về 0.
    /// </summary>
    private void ResetTimer()
    {
        _gameTimer.Stop();
        _isTimerRunning = false;
        _gameStartTimeUtc = null;
        _elapsedTime = TimeSpan.Zero;

        OnPropertyChanged(nameof(ElapsedTime));
        OnPropertyChanged(nameof(ElapsedTimeDisplay));
    }

    /// <summary>
    /// - (EN) Handles the periodic timer tick and refreshes elapsed time.
    /// - (VI) Xử lý nhịp tick của timer và cập nhật thời gian đã trôi qua.
    /// </summary>
    /// <param name="sender">- (EN) Event sender / (VI) Đối tượng phát sự kiện</param>
    /// <param name="e">- (EN) Event data / (VI) Dữ liệu sự kiện</param>
    private void OnGameTimerTick(object? sender, EventArgs e)
    {
        UpdateElapsedTime();
    }

    /// <summary>
    /// - (EN) Updates the elapsed gameplay duration based on the stored start time.
    /// - (VI) Cập nhật thời lượng ván chơi dựa trên thời điểm bắt đầu đã lưu.
    /// </summary>
    private void UpdateElapsedTime()
    {
        if (_gameStartTimeUtc is null)
            return;

        _elapsedTime = DateTime.UtcNow - _gameStartTimeUtc.Value;

        OnPropertyChanged(nameof(ElapsedTime));
        OnPropertyChanged(nameof(ElapsedTimeDisplay));
    }

    /// <summary>
    /// - (EN) Updates the best completion time for the current difficulty when the player wins.
    /// - (VI) Cập nhật thời gian tốt nhất cho độ khó hiện tại khi người chơi thắng.
    /// </summary>
    private void UpdateBestTimeIfNeeded()
    {
        if (_game.State != GameState.Won)
            return;

        if (SelectedDifficulty == DifficultyLevel.Custom)
            return;

        if (!_bestTimes.TryGetValue(SelectedDifficulty, out var currentBest) || _elapsedTime < currentBest)
        {
            _bestTimes[SelectedDifficulty] = _elapsedTime;
            OnPropertyChanged(nameof(BestTime));
            OnPropertyChanged(nameof(BestTimeDisplay));
        }
    }
    #endregion
}