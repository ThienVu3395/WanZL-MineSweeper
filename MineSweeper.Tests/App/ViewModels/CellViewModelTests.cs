using MineSweeper.App.ViewModels;
using MineSweeper.Core.Models;

namespace MineSweeper.Tests.App.ViewModels;

/// <summary>
/// Unit tests for CellViewModel.
/// Ensures correct mapping from domain model and proper UI behavior.
/// </summary>
public class CellViewModelTests
{
    [Fact]
    public void Constructor_ShouldExposeCoordinatesCorrectly()
    {
        // Arrange
        var cell = new Cell(2, 3);

        // Act
        var vm = new CellViewModel(cell);

        // Assert
        Assert.Equal(2, vm.Row);
        Assert.Equal(3, vm.Column);
    }

    [Fact]
    public void DisplayText_ShouldBeEmpty_WhenCellIsHidden()
    {
        var cell = new Cell(0, 0)
        {
            IsRevealed = false,
            IsFlagged = false,
            IsMine = false,
            AdjacentMines = 3
        };

        var vm = new CellViewModel(cell);

        Assert.Equal(string.Empty, vm.DisplayText);
    }

    [Fact]
    public void DisplayText_ShouldShowFlag_WhenCellIsFlagged()
    {
        var cell = new Cell(0, 0)
        {
            IsRevealed = false,
            IsFlagged = true
        };

        var vm = new CellViewModel(cell);

        Assert.Equal("🚩", vm.DisplayText);
    }

    [Fact]
    public void DisplayText_ShouldShowMine_WhenRevealedAndIsMine()
    {
        var cell = new Cell(0, 0)
        {
            IsRevealed = true,
            IsMine = true
        };

        var vm = new CellViewModel(cell);

        Assert.Equal("💣", vm.DisplayText);
    }

    [Fact]
    public void DisplayText_ShouldShowNumber_WhenRevealedAndHasAdjacentMines()
    {
        var cell = new Cell(0, 0)
        {
            IsRevealed = true,
            IsMine = false,
            AdjacentMines = 4
        };

        var vm = new CellViewModel(cell);

        Assert.Equal("4", vm.DisplayText);
    }

    [Fact]
    public void DisplayText_ShouldBeEmpty_WhenRevealedAndAdjacentMinesIsZero()
    {
        var cell = new Cell(0, 0)
        {
            IsRevealed = true,
            IsMine = false,
            AdjacentMines = 0
        };

        var vm = new CellViewModel(cell);

        Assert.Equal(string.Empty, vm.DisplayText);
    }

    [Fact]
    public void Refresh_ShouldRaisePropertyChanged_ForAllRelevantProperties()
    {
        // Arrange
        var cell = new Cell(0, 0);
        var vm = new CellViewModel(cell);

        var changedProperties = new List<string>();

        vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName != null)
                changedProperties.Add(e.PropertyName);
        };

        // Act
        vm.Refresh();

        // Assert
        Assert.Contains(nameof(vm.IsRevealed), changedProperties);
        Assert.Contains(nameof(vm.IsHidden), changedProperties);
        Assert.Contains(nameof(vm.IsFlagged), changedProperties);
        Assert.Contains(nameof(vm.IsMine), changedProperties);
        Assert.Contains(nameof(vm.AdjacentMines), changedProperties);
        Assert.Contains(nameof(vm.DisplayText), changedProperties);
    }

    [Fact]
    public void Refresh_ShouldReflectUpdatedModelState()
    {
        // Arrange
        var cell = new Cell(0, 0)
        {
            IsRevealed = false,
            IsFlagged = false,
            AdjacentMines = 0
        };

        var vm = new CellViewModel(cell);

        // Update model
        cell.IsFlagged = true;
        cell.AdjacentMines = 2;

        // Act
        vm.Refresh();

        // Assert
        Assert.True(vm.IsFlagged);
        Assert.Equal(2, vm.AdjacentMines);
        Assert.Equal("🚩", vm.DisplayText);
    }
}