using MineSweeper.App.Helpers;

namespace MineSweeper.Tests.App.Helpers;

/// <summary>
/// - (EN) Contains unit tests for RelayCommand.
/// - (VI) Chứa các bài kiểm thử đơn vị cho RelayCommand.
/// </summary>
public class RelayCommandTests
{
    /// <summary>
    /// - (EN) Should invoke the execute action when Execute is called.
    /// - (VI) Phải gọi action thực thi khi phương thức Execute được gọi.
    /// </summary>
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

    /// <summary>
    /// - (EN) Should return true when no canExecute predicate is provided.
    /// - (VI) Phải trả về true khi không cung cấp predicate canExecute.
    /// </summary>
    [Fact]
    public void CanExecute_ShouldReturnTrue_WhenNoPredicate()
    {
        var command = new RelayCommand(_ => { });

        Assert.True(command.CanExecute(null));
    }

    /// <summary>
    /// - (EN) Should return false when the canExecute predicate returns false.
    /// - (VI) Phải trả về false khi predicate canExecute trả về false.
    /// </summary>
    [Fact]
    public void CanExecute_ShouldRespectPredicate()
    {
        var command = new RelayCommand(_ => { }, _ => false);

        Assert.False(command.CanExecute(null));
    }

    /// <summary>
    /// - (EN) Should return true when the canExecute predicate returns true.
    /// - (VI) Phải trả về true khi predicate canExecute trả về true.
    /// </summary>
    [Fact]
    public void CanExecute_ShouldReturnTrue_WhenPredicateTrue()
    {
        var command = new RelayCommand(_ => { }, _ => true);

        Assert.True(command.CanExecute(null));
    }

    /// <summary>
    /// - (EN) Should raise CanExecuteChanged event when RaiseCanExecuteChanged is called.
    /// - (VI) Phải kích hoạt sự kiện CanExecuteChanged khi gọi RaiseCanExecuteChanged.
    /// </summary>
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