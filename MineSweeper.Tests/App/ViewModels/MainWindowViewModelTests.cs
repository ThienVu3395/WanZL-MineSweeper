using MineSweeper.App.ViewModels;
using MineSweeper.Core.Models;

namespace MineSweeper.Tests.App.ViewModels;

/// <summary>
/// Contains unit tests for <see cref="MainWindowViewModel"/>.
/// These tests focus on MVVM behavior, command execution, state transitions,
/// counter updates, and property change notifications.
/// </summary>
public class MainWindowViewModelTests
{
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

    [Fact]
    public void SelectedDifficulty_ShouldRaisePropertyChanged_ForSelectedDifficulty()
    {
        // Arrange
        var vm = new MainWindowViewModel();
        string? changedProperty = null;

        vm.PropertyChanged += (_, e) => changedProperty = e.PropertyName;

        // Act
        vm.SelectedDifficulty = DifficultyLevel.Intermediate;

        // Assert
        Assert.Equal(nameof(MainWindowViewModel.SelectedDifficulty), changedProperty);
    }

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

    [Fact]
    public void Commands_ShouldBeDisabled_WhenGameIsFinished()
    {
        // Arrange
        var vm = new MainWindowViewModel();

        // 1x1 with 1 mine guarantees immediate loss on reveal.
        vm.StartNewGame(1, 1, 1);
        var onlyCell = vm.Cells.Single();

        // Act
        vm.RevealCellCommand.Execute(onlyCell);

        // Assert
        Assert.True(vm.IsGameFinished);
        Assert.False(vm.RevealCellCommand.CanExecute(onlyCell));
        Assert.False(vm.ToggleFlagCommand.CanExecute(onlyCell));
    }

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
}