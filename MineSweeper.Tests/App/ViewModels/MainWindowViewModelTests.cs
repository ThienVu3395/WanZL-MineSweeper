using MineSweeper.App.ViewModels;
using MineSweeper.Core.Models;
using System.Reflection;
using MineSweeper.Core.Services;

namespace MineSweeper.Tests.App.ViewModels;

/// <summary>
/// - (EN) Contains unit tests for <see cref="MainWindowViewModel"/>.
/// These tests verify MVVM behavior, command execution, state transitions,
/// counter updates, and property change notifications.
/// - (VI) Chứa các unit test cho <see cref="MainWindowViewModel"/>.
/// Các test này dùng để kiểm tra hành vi MVVM, việc thực thi command,
/// thay đổi trạng thái trò chơi, cập nhật bộ đếm, và thông báo thay đổi thuộc tính.
/// </summary>
public class MainWindowViewModelTests
{
    #region Initialize
    /// <summary>
    /// - (EN) Verifies that the constructor initializes a default beginner game correctly.
    /// - (VI) Kiểm tra constructor có khởi tạo đúng một ván chơi mặc định ở mức Beginner hay không.
    /// </summary>
    [Fact]
    public void Constructor_ShouldInitializeDefaultBeginnerGame()
    {
        // Arrange & Act
        var vm = new MainWindowViewModel();

        // Assert
        Assert.NotNull(vm.Cells);
        Assert.NotEmpty(vm.Cells);
        Assert.Equal(9, vm.Rows);
        Assert.Equal(9, vm.Columns);
        Assert.Equal(10, vm.TotalMines);
        Assert.Equal(DifficultyLevel.Beginner, vm.SelectedDifficulty);
        Assert.Equal("Game in progress", vm.GameStatus);
        Assert.False(vm.IsGameFinished);
    }

    /// <summary>
    /// - (EN) Verifies that reveal and flag commands are disabled once the game has finished.
    /// - (VI) Kiểm tra các command reveal và flag sẽ bị vô hiệu hóa khi trò chơi đã kết thúc.
    /// </summary>
    [Fact]
    public void Commands_ShouldBeDisabled_WhenGameIsFinished()
    {
        // Arrange
        var vm = new MainWindowViewModel();

        vm.StartNewGame(1, 2, 1);

        var firstCell = vm.Cells.Single(c => c.Row == 0 && c.Column == 0);
        var secondCell = vm.Cells.Single(c => c.Row == 0 && c.Column == 1);

        // First reveal is guaranteed safe
        vm.RevealCellCommand.Execute(firstCell);

        // Find the actual mine cell after deferred placement
        var game = GetInternalGame(vm);
        var mineCell = game.Board!.Cells[0, 0].IsMine ? firstCell : secondCell;

        // Act
        vm.RevealCellCommand.Execute(mineCell);

        // Assert
        Assert.True(vm.IsGameFinished);
        Assert.False(vm.RevealCellCommand.CanExecute(mineCell));
        Assert.False(vm.ToggleFlagCommand.CanExecute(mineCell));
    }

    /// <summary>
    /// - (EN) Verifies that changing the selected difficulty raises PropertyChanged
    /// for SelectedDifficulty and related best-time display properties.
    /// - (VI) Kiểm tra khi thay đổi độ khó được chọn thì PropertyChanged sẽ được raise
    /// cho SelectedDifficulty và các thuộc tính hiển thị best time liên quan.
    /// </summary>
    [Fact]
    public void SelectedDifficulty_ShouldRaisePropertyChanged_ForSelectedDifficulty()
    {
        // Arrange
        var vm = new MainWindowViewModel();
        var changedProperties = new List<string>();

        vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is not null)
            {
                changedProperties.Add(e.PropertyName);
            }
        };

        // Act
        vm.SelectedDifficulty = DifficultyLevel.Intermediate;

        // Assert
        Assert.Contains(nameof(MainWindowViewModel.SelectedDifficulty), changedProperties);
        Assert.Contains(nameof(MainWindowViewModel.BestTime), changedProperties);
        Assert.Contains(nameof(MainWindowViewModel.BestTimeDisplay), changedProperties);
    }
    #endregion

    #region NewGameCommand
    /// <summary>
    /// - (EN) Verifies that executing the new game command applies the currently selected difficulty.
    /// - (VI) Kiểm tra khi thực thi command tạo game mới thì độ khó đang được chọn sẽ được áp dụng đúng.
    /// </summary>
    [Fact]
    public void NewGameCommand_ShouldApplySelectedDifficulty()
    {
        // Arrange
        var vm = new MainWindowViewModel();
        vm.SelectedDifficulty = DifficultyLevel.Expert;

        // Act
        vm.NewGameCommand.Execute(null);

        // Assert
        Assert.Equal(16, vm.Rows);
        Assert.Equal(30, vm.Columns);
        Assert.Equal(99, vm.TotalMines);
        Assert.Equal(16 * 30, vm.Cells.Count);
    }

    /// <summary>
    /// - (EN) Verifies that starting a new game clears any previous warning message.
    /// - (VI) Kiểm tra khi bắt đầu game mới thì các thông báo cảnh báo trước đó sẽ được xóa.
    /// </summary>
    [Fact]
    public void NewGame_ShouldClearPreviousWarningMessage()
    {
        // Arrange
        var vm = new MainWindowViewModel();

        foreach (var cell in vm.Cells.Take(vm.TotalMines))
        {
            vm.ToggleFlagCommand.Execute(cell);
        }

        var extraCell = vm.Cells.Skip(vm.TotalMines).First();
        vm.ToggleFlagCommand.Execute(extraCell);

        Assert.NotNull(vm.Message);

        // Act
        vm.NewGameCommand.Execute(null);

        // Assert
        Assert.Null(vm.Message);
    }

    /// <summary>
    /// - (EN) Verifies that starting a new game raises PropertyChanged for all board-related properties.
    /// - (VI) Kiểm tra khi bắt đầu game mới thì PropertyChanged sẽ được raise cho toàn bộ các thuộc tính liên quan đến board.
    /// </summary>
    [Fact]
    public void StartNewGame_ShouldRaisePropertyChanged_ForBoardRelatedProperties()
    {
        // Arrange
        var vm = new MainWindowViewModel();
        var changedProperties = new List<string>();

        vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is not null)
            {
                changedProperties.Add(e.PropertyName);
            }
        };

        // Act
        vm.StartNewGame(16, 16, 40);

        // Assert
        Assert.Contains(nameof(MainWindowViewModel.Rows), changedProperties);
        Assert.Contains(nameof(MainWindowViewModel.Columns), changedProperties);
        Assert.Contains(nameof(MainWindowViewModel.GameStatus), changedProperties);
        Assert.Contains(nameof(MainWindowViewModel.IsGameFinished), changedProperties);
        Assert.Contains(nameof(MainWindowViewModel.TotalMines), changedProperties);
        Assert.Contains(nameof(MainWindowViewModel.FlagCount), changedProperties);
        Assert.Contains(nameof(MainWindowViewModel.RemainingMines), changedProperties);
    }
    #endregion

    #region ToggleFlagCommand
    /// <summary>
    /// - (EN) Verifies that flagging a cell increases the flag counter and updates remaining mines.
    /// - (VI) Kiểm tra khi gắn cờ một ô thì số lượng cờ sẽ tăng và số mìn còn lại sẽ được cập nhật.
    /// </summary>
    [Fact]
    public void ToggleFlagCommand_ShouldFlagCell_AndIncreaseFlagCount()
    {
        // Arrange
        var vm = new MainWindowViewModel();
        var cell = vm.Cells.First();

        // Act
        vm.ToggleFlagCommand.Execute(cell);

        // Assert
        Assert.True(cell.IsFlagged);
        Assert.Equal(1, vm.FlagCount);
        Assert.Equal(vm.TotalMines - 1, vm.RemainingMines);
    }

    /// <summary>
    /// - (EN) Verifies that toggling the same cell twice removes the flag and restores the counters.
    /// - (VI) Kiểm tra khi toggle cùng một ô hai lần thì cờ sẽ được gỡ và các bộ đếm sẽ trở về giá trị ban đầu.
    /// </summary>
    [Fact]
    public void ToggleFlagCommand_Twice_ShouldUnflagCell_AndRestoreCounters()
    {
        // Arrange
        var vm = new MainWindowViewModel();
        var cell = vm.Cells.First();

        // Act
        vm.ToggleFlagCommand.Execute(cell);
        vm.ToggleFlagCommand.Execute(cell);

        // Assert
        Assert.False(cell.IsFlagged);
        Assert.Equal(0, vm.FlagCount);
        Assert.Equal(vm.TotalMines, vm.RemainingMines);
    }

    /// <summary>
    /// - (EN) Verifies that the player cannot place more flags than the total mine count.
    /// - (VI) Kiểm tra người chơi không thể đặt số cờ vượt quá tổng số mìn của bàn chơi.
    /// </summary>
    [Fact]
    public void ToggleFlagCommand_ShouldNotAllowMoreThanTotalMines()
    {
        // Arrange
        var vm = new MainWindowViewModel();

        foreach (var cell in vm.Cells.Take(vm.TotalMines))
        {
            vm.ToggleFlagCommand.Execute(cell);
        }

        var extraCell = vm.Cells.Skip(vm.TotalMines).First();

        // Act
        vm.ToggleFlagCommand.Execute(extraCell);

        // Assert
        Assert.Equal(vm.TotalMines, vm.FlagCount);
        Assert.Equal(0, vm.RemainingMines);
        Assert.False(extraCell.IsFlagged);
        Assert.NotNull(vm.Message);
        Assert.Contains("flags", vm.Message!, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// - (EN) Verifies that toggling a flag raises PropertyChanged for counter-related properties.
    /// - (VI) Kiểm tra thao tác toggle cờ sẽ raise PropertyChanged cho các thuộc tính liên quan đến bộ đếm.
    /// </summary>
    [Fact]
    public void ToggleFlagCommand_ShouldRaisePropertyChanged_ForCounters()
    {
        // Arrange
        var vm = new MainWindowViewModel();
        var changedProperties = new List<string>();
        var cell = vm.Cells.First();

        vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is not null)
            {
                changedProperties.Add(e.PropertyName);
            }
        };

        // Act
        vm.ToggleFlagCommand.Execute(cell);

        // Assert
        Assert.Contains(nameof(MainWindowViewModel.FlagCount), changedProperties);
        Assert.Contains(nameof(MainWindowViewModel.RemainingMines), changedProperties);
        Assert.Contains(nameof(MainWindowViewModel.GameStatus), changedProperties);
        Assert.Contains(nameof(MainWindowViewModel.IsGameFinished), changedProperties);
    }

    /// <summary>
    /// - (EN) Verifies that a valid flag toggle clears the previous warning message.
    /// - (VI) Kiểm tra một thao tác toggle cờ hợp lệ sẽ xóa thông báo cảnh báo trước đó.
    /// </summary>
    [Fact]
    public void ValidToggleFlag_ShouldClearPreviousWarningMessage()
    {
        // Arrange
        var vm = new MainWindowViewModel();

        foreach (var cell in vm.Cells.Take(vm.TotalMines))
        {
            vm.ToggleFlagCommand.Execute(cell);
        }

        var extraCell = vm.Cells.Skip(vm.TotalMines).First();
        vm.ToggleFlagCommand.Execute(extraCell);

        Assert.NotNull(vm.Message);

        var flaggedCell = vm.Cells.First(c => c.IsFlagged);

        // Act
        vm.ToggleFlagCommand.Execute(flaggedCell);

        // Assert
        Assert.Null(vm.Message);
    }
    #endregion

    #region RevealCellCommand
    /// <summary>
    /// - (EN) Verifies that the reveal command can execute while the game is still in progress.
    /// - (VI) Kiểm tra command reveal có thể được thực thi khi trò chơi vẫn đang diễn ra.
    /// </summary>
    [Fact]
    public void RevealCellCommand_ShouldBeExecutable_WhileGameInProgress()
    {
        // Arrange
        var vm = new MainWindowViewModel();
        var cell = vm.Cells.First();

        // Act
        var canExecute = vm.RevealCellCommand.CanExecute(cell);

        // Assert
        Assert.True(canExecute);
    }

    /// <summary>
    /// - (EN) Verifies that revealing a cell raises PropertyChanged for game-state-related properties.
    /// - (VI) Kiểm tra khi mở một ô thì PropertyChanged sẽ được raise cho các thuộc tính liên quan đến trạng thái trò chơi.
    /// </summary>
    [Fact]
    public void RevealCellCommand_ShouldRaisePropertyChanged_ForGameStateProperties()
    {
        // Arrange
        var vm = new MainWindowViewModel();
        var changedProperties = new List<string>();
        var cell = vm.Cells.First();

        vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is not null)
            {
                changedProperties.Add(e.PropertyName);
            }
        };

        // Act
        vm.RevealCellCommand.Execute(cell);

        // Assert
        Assert.Contains(nameof(MainWindowViewModel.GameStatus), changedProperties);
        Assert.Contains(nameof(MainWindowViewModel.IsGameFinished), changedProperties);
        Assert.Contains(nameof(MainWindowViewModel.FlagCount), changedProperties);
        Assert.Contains(nameof(MainWindowViewModel.RemainingMines), changedProperties);
    }
    #endregion

    #region Timer

    /// <summary>
    /// - (EN) Verifies that the timer display is initialized to zero.
    /// - (VI) Kiểm tra bộ hiển thị timer được khởi tạo ở giá trị 0.
    /// </summary>
    [Fact]
    public void Constructor_ShouldInitializeElapsedTimeDisplay_ToZero()
    {
        // Arrange & Act
        var vm = new MainWindowViewModel();

        // Assert
        Assert.Equal(TimeSpan.Zero, vm.ElapsedTime);
        Assert.Equal("00:00", vm.ElapsedTimeDisplay);
    }

    /// <summary>
    /// - (EN) Verifies that the first reveal starts the gameplay timer.
    /// - (VI) Kiểm tra lần mở ô đầu tiên sẽ khởi động timer của ván chơi.
    /// </summary>
    [Fact]
    public void RevealCellCommand_ShouldStartTimer_OnFirstReveal()
    {
        // Arrange
        var vm = new MainWindowViewModel();
        var cell = vm.Cells.First();

        // Act
        vm.RevealCellCommand.Execute(cell);

        // Assert
        bool isTimerRunning = GetPrivateFieldValue<bool>(vm, "_isTimerRunning");
        Assert.True(isTimerRunning);
    }

    /// <summary>
    /// - (EN) Verifies that starting a new game resets the elapsed time back to zero.
    /// - (VI) Kiểm tra khi bắt đầu game mới thì thời gian đã chơi sẽ được reset về 0.
    /// </summary>
    [Fact]
    public void StartNewGame_ShouldResetElapsedTime()
    {
        // Arrange
        var vm = new MainWindowViewModel();

        SetPrivateFieldValue(vm, "_elapsedTime", TimeSpan.FromSeconds(95));
        SetPrivateFieldValue(vm, "_isTimerRunning", true);
        SetPrivateFieldValue(vm, "_gameStartTimeUtc", DateTime.UtcNow.AddSeconds(-95));

        // Act
        vm.StartNewGame(9, 9, 10);

        // Assert
        Assert.Equal(TimeSpan.Zero, vm.ElapsedTime);
        Assert.Equal("00:00", vm.ElapsedTimeDisplay);

        bool isTimerRunning = GetPrivateFieldValue<bool>(vm, "_isTimerRunning");
        Assert.False(isTimerRunning);
    }

    /// <summary>
    /// - (EN) Verifies that the elapsed time display formats the stored elapsed time correctly.
    /// - (VI) Kiểm tra chuỗi hiển thị thời gian định dạng đúng từ giá trị elapsed time đã lưu.
    /// </summary>
    [Fact]
    public void ElapsedTimeDisplay_ShouldFormatElapsedTimeCorrectly()
    {
        // Arrange
        var vm = new MainWindowViewModel();

        SetPrivateFieldValue(vm, "_elapsedTime", TimeSpan.FromSeconds(65));

        // Act
        var display = vm.ElapsedTimeDisplay;

        // Assert
        Assert.Equal("01:05", display);
    }

    /// <summary>
    /// - (EN) Verifies that the timer stops when the game ends in a loss.
    /// - (VI) Kiểm tra timer sẽ dừng khi ván chơi kết thúc ở trạng thái thua.
    /// </summary>
    [Fact]
    public void RevealCellCommand_ShouldStopTimer_WhenPlayerLoses()
    {
        // Arrange
        var vm = new MainWindowViewModel();

        ConfigureDeterministicBoard(vm, 2, 2, (0, 0));

        var safeCell = GetCell(vm, 0, 1);
        var mineCell = GetCell(vm, 0, 0);

        // First reveal starts timer
        vm.RevealCellCommand.Execute(safeCell);

        bool timerRunningAfterFirstReveal = GetPrivateFieldValue<bool>(vm, "_isTimerRunning");
        Assert.True(timerRunningAfterFirstReveal);

        // Act - reveal mine to lose
        vm.RevealCellCommand.Execute(mineCell);

        // Assert
        bool timerRunningAfterLoss = GetPrivateFieldValue<bool>(vm, "_isTimerRunning");
        Assert.False(timerRunningAfterLoss);
    }

    #endregion

    #region BestTime

    /// <summary>
    /// - (EN) Verifies that best time display shows fallback text when no record exists.
    /// - (VI) Kiểm tra chuỗi best time hiển thị giá trị mặc định khi chưa có kỷ lục.
    /// </summary>
    [Fact]
    public void BestTimeDisplay_ShouldReturnFallback_WhenNoBestTimeExists()
    {
        // Arrange
        var vm = new MainWindowViewModel();

        // Assert
        Assert.Null(vm.BestTime);
        Assert.Equal("--:--", vm.BestTimeDisplay);
    }

    /// <summary>
    /// - (EN) Verifies that winning a game stores the first best time for the selected difficulty.
    /// - (VI) Kiểm tra khi thắng game thì thời gian thắng đầu tiên sẽ được lưu làm best time cho độ khó đang chọn.
    /// </summary>
    [Fact]
    public void WinningGame_ShouldStoreBestTime_ForSelectedDifficulty()
    {
        // Arrange
        var vm = new MainWindowViewModel();
        vm.SelectedDifficulty = DifficultyLevel.Beginner;

        ConfigureDeterministicBoard(vm, 2, 2, (0, 0));
        SetPrivateFieldValue(vm, "_elapsedTime", TimeSpan.FromSeconds(42));

        vm.RevealCellCommand.Execute(GetCell(vm, 0, 1));
        vm.RevealCellCommand.Execute(GetCell(vm, 1, 0));

        // Act
        vm.RevealCellCommand.Execute(GetCell(vm, 1, 1));

        // Assert
        Assert.NotNull(vm.BestTime);
        Assert.Equal("00:42", vm.BestTimeDisplay);
    }

    /// <summary>
    /// - (EN) Verifies that a better completion time replaces the previous best time.
    /// - (VI) Kiểm tra một thời gian hoàn thành tốt hơn sẽ thay thế best time trước đó.
    /// </summary>
    [Fact]
    public void WinningGame_ShouldReplaceBestTime_WhenNewTimeIsBetter()
    {
        // Arrange
        var vm = new MainWindowViewModel();
        vm.SelectedDifficulty = DifficultyLevel.Beginner;

        SetPrivateFieldValue(vm, "_bestTimes", new Dictionary<DifficultyLevel, TimeSpan>
        {
            [DifficultyLevel.Beginner] = TimeSpan.FromSeconds(60)
        });

        ConfigureDeterministicBoard(vm, 2, 2, (0, 0));
        SetPrivateFieldValue(vm, "_elapsedTime", TimeSpan.FromSeconds(45));

        vm.RevealCellCommand.Execute(GetCell(vm, 0, 1));
        vm.RevealCellCommand.Execute(GetCell(vm, 1, 0));

        // Act
        vm.RevealCellCommand.Execute(GetCell(vm, 1, 1));

        // Assert
        Assert.NotNull(vm.BestTime);
        Assert.Equal("00:45", vm.BestTimeDisplay);
    }

    /// <summary>
    /// - (EN) Verifies that a worse completion time does not overwrite the stored best time.
    /// - (VI) Kiểm tra một thời gian hoàn thành kém hơn sẽ không ghi đè best time đã lưu.
    /// </summary>
    [Fact]
    public void WinningGame_ShouldNotReplaceBestTime_WhenNewTimeIsWorse()
    {
        // Arrange
        var vm = new MainWindowViewModel();
        vm.SelectedDifficulty = DifficultyLevel.Beginner;

        SetPrivateFieldValue(vm, "_bestTimes", new Dictionary<DifficultyLevel, TimeSpan>
        {
            [DifficultyLevel.Beginner] = TimeSpan.FromSeconds(40)
        });

        ConfigureDeterministicBoard(vm, 2, 2, (0, 0));
        SetPrivateFieldValue(vm, "_elapsedTime", TimeSpan.FromSeconds(55));

        vm.RevealCellCommand.Execute(GetCell(vm, 0, 1));
        vm.RevealCellCommand.Execute(GetCell(vm, 1, 0));

        // Act
        vm.RevealCellCommand.Execute(GetCell(vm, 1, 1));

        // Assert
        Assert.Equal(TimeSpan.FromSeconds(40), vm.BestTime);
        Assert.Equal("00:40", vm.BestTimeDisplay);
    }

    /// <summary>
    /// - (EN) Verifies that best time display changes when the selected difficulty changes.
    /// - (VI) Kiểm tra best time hiển thị sẽ thay đổi theo độ khó được chọn.
    /// </summary>
    [Fact]
    public void SelectedDifficulty_ShouldRefreshBestTimeDisplay()
    {
        // Arrange
        var vm = new MainWindowViewModel();

        SetPrivateFieldValue(vm, "_bestTimes", new Dictionary<DifficultyLevel, TimeSpan>
        {
            [DifficultyLevel.Beginner] = TimeSpan.FromSeconds(30),
            [DifficultyLevel.Intermediate] = TimeSpan.FromSeconds(90)
        });

        // Act
        vm.SelectedDifficulty = DifficultyLevel.Beginner;
        var beginnerDisplay = vm.BestTimeDisplay;

        vm.SelectedDifficulty = DifficultyLevel.Intermediate;
        var intermediateDisplay = vm.BestTimeDisplay;

        // Assert
        Assert.Equal("00:30", beginnerDisplay);
        Assert.Equal("01:30", intermediateDisplay);
    }

    #endregion

    #region GameEndedEvent
    /// <summary>
    /// - (EN) Verifies that the view model raises GameEnded when the player loses
    /// after a safe first reveal.
    /// - (VI) Kiểm tra ViewModel sẽ phát GameEnded khi người chơi thua
    /// sau một lần mở ô đầu tiên an toàn.
    /// </summary>
    [Fact]
    public void RevealCellCommand_ShouldRaiseGameEndedEvent_WhenPlayerLoses()
    {
        // Arrange
        var vm = new MainWindowViewModel();
        GameState? finalState = null;

        vm.GameEnded += (_, e) => finalState = e.State;

        ConfigureDeterministicBoard(vm, 2, 2, (0, 0));

        var safeCell = GetCell(vm, 0, 1);
        var mineCell = GetCell(vm, 0, 0);

        // Act - first reveal is safe and should not end the game yet
        vm.RevealCellCommand.Execute(safeCell);

        // Sanity check
        Assert.False(vm.IsGameFinished);
        Assert.Equal(GameState.InProgress, GetInternalGame(vm).State);

        // Reveal the mine on the next move
        vm.RevealCellCommand.Execute(mineCell);

        // Assert
        Assert.Equal(GameState.Lost, finalState);
    }

    /// <summary>
    /// - (EN) Verifies that the view model raises GameEnded when the player wins.
    /// - (VI) Kiểm tra ViewModel sẽ phát GameEnded khi người chơi thắng.
    /// </summary>
    [Fact]
    public void RevealCellCommand_ShouldRaiseGameEndedEvent_WhenPlayerWins()
    {
        // Arrange
        var vm = new MainWindowViewModel();
        GameState? finalState = null;

        vm.GameEnded += (_, e) => finalState = e.State;

        ConfigureDeterministicBoard(vm, 2, 2, (0, 0));

        vm.RevealCellCommand.Execute(GetCell(vm, 0, 1));
        vm.RevealCellCommand.Execute(GetCell(vm, 1, 0));
        vm.RevealCellCommand.Execute(GetCell(vm, 1, 1));

        // Assert
        Assert.Equal(GameState.Won, finalState);
    }
    #endregion

    #region Private Helpers
    /// <summary>
    /// - (EN) Gets the internal game service instance from the view model for deterministic test setup.
    /// - (VI) Lấy instance game service bên trong view model để phục vụ việc setup test theo dữ liệu cố định.
    /// </summary>
    /// <param name="vm">- (EN) Target view model / (VI) ViewModel cần lấy game service</param>
    /// <returns>- (EN) Internal MineSweeperGame instance / (VI) Instance MineSweeperGame bên trong ViewModel</returns>
    private static MineSweeperGame GetInternalGame(MainWindowViewModel vm)
    {
        var field = typeof(MainWindowViewModel).GetField("_game", BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.NotNull(field);

        var game = field!.GetValue(vm) as MineSweeperGame;

        Assert.NotNull(game);

        return game!;
    }

    /// <summary>
    /// - (EN) Gets the value of a private field from the target view model.
    /// - (VI) Lấy giá trị của một private field từ view model mục tiêu.
    /// </summary>
    /// <typeparam name="T">- (EN) Field type / (VI) Kiểu dữ liệu của field</typeparam>
    /// <param name="vm">- (EN) Target view model / (VI) ViewModel cần đọc field</param>
    /// <param name="fieldName">- (EN) Private field name / (VI) Tên private field</param>
    /// <returns>- (EN) Field value / (VI) Giá trị field</returns>
    private static T GetPrivateFieldValue<T>(MainWindowViewModel vm, string fieldName)
    {
        var field = typeof(MainWindowViewModel).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.NotNull(field);

        return (T)field!.GetValue(vm)!;
    }

    /// <summary>
    /// - (EN) Sets the value of a private field on the target view model.
    /// - (VI) Gán giá trị cho một private field của view model mục tiêu.
    /// </summary>
    /// <param name="vm">- (EN) Target view model / (VI) ViewModel cần gán field</param>
    /// <param name="fieldName">- (EN) Private field name / (VI) Tên private field</param>
    /// <param name="value">- (EN) Value to assign / (VI) Giá trị cần gán</param>
    private static void SetPrivateFieldValue(MainWindowViewModel vm, string fieldName, object? value)
    {
        var field = typeof(MainWindowViewModel).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.NotNull(field);

        field!.SetValue(vm, value);
    }

    /// <summary>
    /// - (EN) Finds a cell view model by row and column.
    /// - (VI) Tìm một CellViewModel theo hàng và cột.
    /// </summary>
    /// <param name="vm">- (EN) Target view model / (VI) ViewModel cần tìm cell</param>
    /// <param name="row">- (EN) Row index / (VI) Chỉ số hàng</param>
    /// <param name="column">- (EN) Column index / (VI) Chỉ số cột</param>
    /// <returns>- (EN) Matching cell view model / (VI) CellViewModel tương ứng</returns>
    private static CellViewModel GetCell(MainWindowViewModel vm, int row, int column)
    {
        return vm.Cells.Single(c => c.Row == row && c.Column == column);
    }

    /// <summary>
    /// - (EN) Rebuilds the board with deterministic mine positions for ViewModel tests.
    /// - (VI) Cấu hình lại board với vị trí mìn cố định để test ViewModel một cách ổn định.
    /// </summary>
    /// <param name="vm">- (EN) Target view model / (VI) ViewModel cần cấu hình board</param>
    /// <param name="rows">- (EN) Board rows / (VI) Số hàng của board</param>
    /// <param name="columns">- (EN) Board columns / (VI) Số cột của board</param>
    /// <param name="minePositions">
    /// - (EN) Explicit mine coordinates / (VI) Danh sách tọa độ mìn được chỉ định rõ
    /// </param>
    private static void ConfigureDeterministicBoard(
        MainWindowViewModel vm,
        int rows,
        int columns,
        params (int Row, int Column)[] minePositions)
    {
        vm.StartNewGame(rows, columns, minePositions.Length);

        var game = GetInternalGame(vm);
        var board = game.Board!;

        foreach (var cell in board.Cells)
        {
            cell.IsMine = false;
            cell.IsRevealed = false;
            cell.IsFlagged = false;
            cell.AdjacentMines = 0;
        }

        foreach (var mine in minePositions)
        {
            board.Cells[mine.Row, mine.Column].IsMine = true;
        }

        game.CalculateAdjacentMines(board);

        // Important for first-click-safe mode:
        // disable deferred mine placement because the board is already configured manually.
        // Quan trọng với chế độ first-click-safe:
        // tắt cơ chế đặt mìn trì hoãn vì board đã được cấu hình thủ công.
        var firstRevealPendingField = typeof(MineSweeperGame)
            .GetField("_isFirstRevealPending", BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.NotNull(firstRevealPendingField);
        firstRevealPendingField!.SetValue(game, false);

        foreach (var cellVm in vm.Cells)
        {
            cellVm.Refresh();
        }
    }
    #endregion
}