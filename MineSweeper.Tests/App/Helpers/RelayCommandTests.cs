using MineSweeper.App.Helpers;

namespace MineSweeper.Tests.App.Helpers;

public class RelayCommandTests
{
    [Fact]
    public void Execute_ShouldInvokeAction()
    {
        // Arrange
        bool executed = false;
        var command = new RelayCommand(_ => executed = true);

        // Act
        command.Execute(null);

        // Assert
        Assert.True(executed);
    }

    [Fact]
    public void CanExecute_ShouldReturnTrue_WhenNoPredicate()
    {
        var command = new RelayCommand(_ => { });

        Assert.True(command.CanExecute(null));
    }

    [Fact]
    public void CanExecute_ShouldRespectPredicate()
    {
        var command = new RelayCommand(_ => { }, _ => false);

        Assert.False(command.CanExecute(null));
    }

    [Fact]
    public void CanExecute_ShouldReturnTrue_WhenPredicateTrue()
    {
        var command = new RelayCommand(_ => { }, _ => true);

        Assert.True(command.CanExecute(null));
    }

    [Fact]
    public void RaiseCanExecuteChanged_ShouldTriggerEvent()
    {
        var command = new RelayCommand(_ => { });

        bool eventRaised = false;

        command.CanExecuteChanged += (_, _) => eventRaised = true;

        command.RaiseCanExecuteChanged();

        Assert.True(eventRaised);
    }
}