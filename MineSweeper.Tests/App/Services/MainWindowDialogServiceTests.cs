using MineSweeper.App.Models;
using MineSweeper.App.Services;
using MineSweeper.App.ViewModels;
using MineSweeper.Core.Models;
using System.Windows;
using System.Windows.Controls;

namespace MineSweeper.Tests.App.Services;

/// <summary>
/// - (EN) Contains unit tests for <see cref="MainWindowDialogService"/>.
/// - (VI) Chứa các unit test cho <see cref="MainWindowDialogService"/>.
/// </summary>
public class MainWindowDialogServiceTests
{
    /// <summary>
    /// - (EN) Verifies that a restart-related action executes immediately when the current game has no active progress.
    /// - (VI) Kiểm tra hành động liên quan đến restart sẽ được thực thi ngay khi ván hiện tại chưa có tiến trình đáng kể.
    /// </summary>
    [Fact]
    public void RequestRestartAction_ShouldExecuteImmediately_WhenNoActiveProgress()
    {
        TestStaHelper.Run(() =>
        {
            string tempDirectory = CreateTempDirectory();

            try
            {
                var vm = CreateViewModel(tempDirectory);
                var controls = CreateControls();
                var service = CreateService(controls);

                bool executed = false;

                service.RequestRestartAction(
                    vm,
                    () => executed = true,
                    new DialogRequest
                    {
                        Title = "Restart?",
                        Message = "Confirm restart."
                    });

                Assert.True(executed);
                Assert.Equal(Visibility.Collapsed, controls.RestartConfirmationOverlay.Visibility);
            }
            finally
            {
                DeleteTempDirectory(tempDirectory);
            }
        });
    }

    /// <summary>
    /// - (EN) Verifies that a restart confirmation dialog is shown instead of immediate execution when active game progress exists.
    /// - (VI) Kiểm tra hộp thoại xác nhận restart sẽ được hiển thị thay vì thực thi ngay khi ván hiện tại đã có tiến trình.
    /// </summary>
    [Fact]
    public void RequestRestartAction_ShouldShowConfirmation_WhenActiveProgressExists()
    {
        TestStaHelper.Run(() =>
        {
            string tempDirectory = CreateTempDirectory();

            try
            {
                var vm = CreateViewModel(tempDirectory);
                var controls = CreateControls();
                var service = CreateService(controls);

                vm.ToggleFlagCommand.Execute(vm.Cells.First());

                bool executed = false;

                service.RequestRestartAction(
                    vm,
                    () => executed = true,
                    new DialogRequest
                    {
                        Title = "Restart current game?",
                        Message = "Your current progress will be lost."
                    });

                Assert.False(executed);
                Assert.Equal(Visibility.Visible, controls.RestartConfirmationOverlay.Visibility);
                Assert.Equal("Restart current game?", controls.RestartConfirmationTitle.Text);
                Assert.Equal("Your current progress will be lost.", controls.RestartConfirmationMessage.Text);
            }
            finally
            {
                DeleteTempDirectory(tempDirectory);
            }
        });
    }

    /// <summary>
    /// - (EN) Verifies that confirming the restart confirmation executes the pending action.
    /// - (VI) Kiểm tra việc xác nhận hộp thoại restart sẽ thực thi hành động đang chờ.
    /// </summary>
    [Fact]
    public void ConfirmRestartConfirmation_ShouldExecutePendingAction()
    {
        TestStaHelper.Run(() =>
        {
            string tempDirectory = CreateTempDirectory();

            try
            {
                var vm = CreateViewModel(tempDirectory);
                var controls = CreateControls();
                var service = CreateService(controls);

                vm.ToggleFlagCommand.Execute(vm.Cells.First());

                bool executed = false;

                service.RequestRestartAction(
                    vm,
                    () => executed = true,
                    new DialogRequest
                    {
                        Title = "Restart?",
                        Message = "Confirm restart."
                    });

                service.ConfirmRestartConfirmation();

                Assert.True(executed);
                Assert.Equal(Visibility.Collapsed, controls.RestartConfirmationOverlay.Visibility);
            }
            finally
            {
                DeleteTempDirectory(tempDirectory);
            }
        });
    }

    /// <summary>
    /// - (EN) Verifies that cancelling the restart confirmation does not execute the pending action.
    /// - (VI) Kiểm tra việc hủy hộp thoại restart sẽ không thực thi hành động đang chờ.
    /// </summary>
    [Fact]
    public void CancelRestartConfirmation_ShouldNotExecutePendingAction()
    {
        TestStaHelper.Run(() =>
        {
            string tempDirectory = CreateTempDirectory();

            try
            {
                var vm = CreateViewModel(tempDirectory);
                var controls = CreateControls();
                var service = CreateService(controls);

                vm.ToggleFlagCommand.Execute(vm.Cells.First());

                bool executed = false;

                service.RequestRestartAction(
                    vm,
                    () => executed = true,
                    new DialogRequest
                    {
                        Title = "Restart?",
                        Message = "Confirm restart."
                    });

                service.CancelRestartConfirmation();

                Assert.False(executed);
                Assert.Equal(Visibility.Collapsed, controls.RestartConfirmationOverlay.Visibility);
            }
            finally
            {
                DeleteTempDirectory(tempDirectory);
            }
        });
    }

    /// <summary>
    /// - (EN) Verifies that showing the restart confirmation dialog populates the title and message correctly.
    /// - (VI) Kiểm tra việc hiển thị hộp thoại xác nhận restart sẽ gán đúng tiêu đề và nội dung.
    /// </summary>
    [Fact]
    public void ShowRestartConfirmationDialog_ShouldPopulateDialogContent()
    {
        TestStaHelper.Run(() =>
        {
            var controls = CreateControls();
            var service = CreateService(controls);

            service.ShowRestartConfirmationDialog(new DialogRequest
            {
                Title = "Change difficulty?",
                Message = "Your current progress will be lost."
            });

            Assert.Equal(Visibility.Visible, controls.RestartConfirmationOverlay.Visibility);
            Assert.Equal("Change difficulty?", controls.RestartConfirmationTitle.Text);
            Assert.Equal("Your current progress will be lost.", controls.RestartConfirmationMessage.Text);
        });
    }

    /// <summary>
    /// - (EN) Verifies that the end-game dialog shows win title and message when the game state is won.
    /// - (VI) Kiểm tra hộp thoại kết thúc game sẽ hiển thị tiêu đề và nội dung chiến thắng khi trạng thái game là thắng.
    /// </summary>
    [Fact]
    public void ShowEndGameDialog_ShouldDisplayWinContent_WhenStateIsWon()
    {
        TestStaHelper.Run(() =>
        {
            var controls = CreateControls();
            var service = CreateService(controls);

            service.ShowEndGameDialog(GameState.Won);

            Assert.Equal(Visibility.Visible, controls.EndGameDialogOverlay.Visibility);
            Assert.Equal("🎉 You won!", controls.EndGameDialogTitle.Text);
            Assert.Contains("cleared all safe cells", controls.EndGameDialogMessage.Text, StringComparison.OrdinalIgnoreCase);
        });
    }

    /// <summary>
    /// - (EN) Verifies that the end-game dialog shows loss title and message when the game state is lost.
    /// - (VI) Kiểm tra hộp thoại kết thúc game sẽ hiển thị tiêu đề và nội dung thua cuộc khi trạng thái game là thua.
    /// </summary>
    [Fact]
    public void ShowEndGameDialog_ShouldDisplayLossContent_WhenStateIsLost()
    {
        TestStaHelper.Run(() =>
        {
            var controls = CreateControls();
            var service = CreateService(controls);

            service.ShowEndGameDialog(GameState.Lost);

            Assert.Equal(Visibility.Visible, controls.EndGameDialogOverlay.Visibility);
            Assert.Equal("💥 Game over", controls.EndGameDialogTitle.Text);
            Assert.Contains("revealed a mine", controls.EndGameDialogMessage.Text, StringComparison.OrdinalIgnoreCase);
        });
    }

    /// <summary>
    /// - (EN) Verifies that hiding the end-game dialog collapses the overlay and resets its opacity.
    /// - (VI) Kiểm tra việc ẩn hộp thoại kết thúc game sẽ collapse overlay và đặt lại opacity của nó.
    /// </summary>
    [Fact]
    public void HideEndGameDialog_ShouldCollapseOverlay()
    {
        TestStaHelper.Run(() =>
        {
            var controls = CreateControls();
            var service = CreateService(controls);

            service.ShowEndGameDialog(GameState.Won);
            service.HideEndGameDialog();

            Assert.Equal(Visibility.Collapsed, controls.EndGameDialogOverlay.Visibility);
            Assert.Equal(0, controls.EndGameDialogOverlay.Opacity);
        });
    }

    /// <summary>
    /// - (EN) Verifies that showing a toast with null or empty message hides the toast element.
    /// - (VI) Kiểm tra việc hiển thị toast với message null hoặc rỗng sẽ ẩn phần tử toast.
    /// </summary>
    [Fact]
    public void ShowToast_ShouldHideToast_WhenMessageIsNullOrEmpty()
    {
        TestStaHelper.Run(() =>
        {
            string tempDirectory = CreateTempDirectory();

            try
            {
                var vm = CreateViewModel(tempDirectory);
                var controls = CreateControls();
                var service = CreateService(controls);

                vm.ClearTemporaryMessage();
                service.ShowToast(vm);

                Assert.Equal(0, controls.ToastMessage.Opacity);
            }
            finally
            {
                DeleteTempDirectory(tempDirectory);
            }
        });
    }

    #region Helpers
    private static MainWindowDialogService CreateService(DialogControls controls)
    {
        return new MainWindowDialogService(
            controls.ToastMessage,
            controls.EndGameDialogOverlay,
            controls.EndGameDialogTitle,
            controls.EndGameDialogMessage,
            controls.RestartConfirmationOverlay,
            controls.RestartConfirmationTitle,
            controls.RestartConfirmationMessage,
            controls.ConfirmRestartButton,
            controls.PlayAgainButton);
    }

    private static DialogControls CreateControls()
    {
        return new DialogControls
        {
            ToastMessage = new Border { Visibility = Visibility.Visible, Opacity = 1 },
            EndGameDialogOverlay = new Border { Visibility = Visibility.Collapsed, Opacity = 0 },
            EndGameDialogTitle = new TextBlock(),
            EndGameDialogMessage = new TextBlock(),
            RestartConfirmationOverlay = new Border { Visibility = Visibility.Collapsed, Opacity = 0 },
            RestartConfirmationTitle = new TextBlock(),
            RestartConfirmationMessage = new TextBlock(),
            ConfirmRestartButton = new Button(),
            PlayAgainButton = new Button()
        };
    }

    private static MainWindowViewModel CreateViewModel(string tempDirectory)
    {
        return new MainWindowViewModel(
            new PlayerStatisticsStore(Path.Combine(tempDirectory, "best-times.json")),
            new PlayerPreferencesStore(Path.Combine(tempDirectory, "player-preferences.json")),
            new GameSessionStore(Path.Combine(tempDirectory, "game-session.json")));
    }

    private static string CreateTempDirectory()
    {
        string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDirectory);
        return tempDirectory;
    }

    private static void DeleteTempDirectory(string tempDirectory)
    {
        if (Directory.Exists(tempDirectory))
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    private sealed class DialogControls
    {
        public required Border ToastMessage { get; init; }
        public required Border EndGameDialogOverlay { get; init; }
        public required TextBlock EndGameDialogTitle { get; init; }
        public required TextBlock EndGameDialogMessage { get; init; }
        public required Border RestartConfirmationOverlay { get; init; }
        public required TextBlock RestartConfirmationTitle { get; init; }
        public required TextBlock RestartConfirmationMessage { get; init; }
        public required Button ConfirmRestartButton { get; init; }
        public required Button PlayAgainButton { get; init; }
    }
    #endregion
}