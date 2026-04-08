using MineSweeper.App.Models;
using MineSweeper.App.ViewModels;
using MineSweeper.Core.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MineSweeper.App.Services;

/// <summary>
/// - (EN) Encapsulates toast and dialog behavior used by the main application window.
/// - (VI) Đóng gói hành vi toast và dialog được sử dụng bởi cửa sổ chính của ứng dụng.
/// </summary>
public class MainWindowDialogService
{
    private readonly Border _toastMessage;
    private readonly Border _endGameDialogOverlay;
    private readonly TextBlock _endGameDialogTitle;
    private readonly TextBlock _endGameDialogMessage;
    private readonly Border _restartConfirmationOverlay;
    private readonly TextBlock _restartConfirmationTitle;
    private readonly TextBlock _restartConfirmationMessage;
    private readonly Button _confirmRestartButton;
    private readonly Button _playAgainButton;

    private Action? _pendingRestartAction;

    /// <summary>
    /// - (EN) Initializes a new instance of the <see cref="MainWindowDialogService"/> class.
    /// - (VI) Khởi tạo một instance mới của <see cref="MainWindowDialogService"/>.
    /// </summary>
    public MainWindowDialogService(
        Border toastMessage,
        Border endGameDialogOverlay,
        TextBlock endGameDialogTitle,
        TextBlock endGameDialogMessage,
        Border restartConfirmationOverlay,
        TextBlock restartConfirmationTitle,
        TextBlock restartConfirmationMessage,
        Button confirmRestartButton,
        Button playAgainButton)
    {
        _toastMessage = toastMessage;
        _endGameDialogOverlay = endGameDialogOverlay;
        _endGameDialogTitle = endGameDialogTitle;
        _endGameDialogMessage = endGameDialogMessage;
        _restartConfirmationOverlay = restartConfirmationOverlay;
        _restartConfirmationTitle = restartConfirmationTitle;
        _restartConfirmationMessage = restartConfirmationMessage;
        _confirmRestartButton = confirmRestartButton;
        _playAgainButton = playAgainButton;
    }

    /// <summary>
    /// - (EN) Gets whether the restart confirmation overlay is currently visible.
    /// - (VI) Lấy giá trị cho biết overlay xác nhận restart hiện có đang hiển thị hay không.
    /// </summary>
    public bool IsRestartConfirmationVisible => _restartConfirmationOverlay.Visibility == Visibility.Visible;

    /// <summary>
    /// - (EN) Displays a toast message using fade-in and fade-out animations.
    /// Clears the message from the view model after the animation completes.
    /// - (VI) Hiển thị toast bằng animation fade-in và fade-out.
    /// Xóa message khỏi view model sau khi animation hoàn tất.
    /// </summary>
    /// <param name="viewModel">
    /// - (EN) The active view model containing the current message.
    /// - (VI) ViewModel hiện tại chứa message đang hiển thị.
    /// </param>
    public void ShowToast(MainWindowViewModel viewModel)
    {
        if (string.IsNullOrWhiteSpace(viewModel.Message))
        {
            _toastMessage.BeginAnimation(UIElement.OpacityProperty, null);
            _toastMessage.Opacity = 0;
            return;
        }

        _toastMessage.BeginAnimation(UIElement.OpacityProperty, null);
        _toastMessage.Opacity = 0;

        var animation = new DoubleAnimationUsingKeyFrames();

        animation.KeyFrames.Add(
            new DiscreteDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.Zero)));

        animation.KeyFrames.Add(
            new EasingDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(200)))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            });

        animation.KeyFrames.Add(
            new DiscreteDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(3))));

        animation.KeyFrames.Add(
            new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(3.9)))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            });

        animation.Completed += (_, _) =>
        {
            viewModel.ClearTemporaryMessage();
        };

        _toastMessage.BeginAnimation(UIElement.OpacityProperty, animation);
    }

    /// <summary>
    /// - (EN) Displays the end-game dialog using the supplied game state.
    /// - (VI) Hiển thị hộp thoại kết thúc game dựa trên trạng thái game được cung cấp.
    /// </summary>
    /// <param name="state">
    /// - (EN) Final game state.
    /// - (VI) Trạng thái cuối cùng của game.
    /// </param>
    public void ShowEndGameDialog(GameState state)
    {
        if (state == GameState.Won)
        {
            _endGameDialogTitle.Text = "🎉 You won!";
            _endGameDialogMessage.Text = "Great job! You cleared all safe cells on the board.";
            _endGameDialogTitle.Foreground = new SolidColorBrush(Color.FromRgb(34, 139, 34));
        }
        else
        {
            _endGameDialogTitle.Text = "💥 Game over";
            _endGameDialogMessage.Text = "You revealed a mine. Try again and beat your best time!";
            _endGameDialogTitle.Foreground = new SolidColorBrush(Color.FromRgb(178, 34, 34));
        }

        _endGameDialogOverlay.Visibility = Visibility.Visible;
        _playAgainButton.Focus();

        var fadeIn = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = TimeSpan.FromMilliseconds(180)
        };

        _endGameDialogOverlay.BeginAnimation(UIElement.OpacityProperty, fadeIn);
    }

    /// <summary>
    /// - (EN) Hides the end-game dialog overlay.
    /// - (VI) Ẩn overlay hộp thoại kết thúc game.
    /// </summary>
    public void HideEndGameDialog()
    {
        _endGameDialogOverlay.Visibility = Visibility.Collapsed;
        _endGameDialogOverlay.Opacity = 0;
    }

    /// <summary>
    /// - (EN) Requests execution of a restart-related action.
    /// If the current game has active progress, a confirmation dialog is shown instead of executing immediately.
    /// - (VI) Yêu cầu thực thi một hành động liên quan đến restart.
    /// Nếu ván chơi hiện tại có tiến trình, một hộp thoại xác nhận sẽ được hiển thị thay vì thực thi ngay.
    /// </summary>
    /// <param name="viewModel">
    /// - (EN) Active window view model.
    /// - (VI) ViewModel hiện tại của cửa sổ.
    /// </param>
    /// <param name="executeAction">
    /// - (EN) Action to execute after confirmation.
    /// - (VI) Hành động sẽ được thực thi sau khi xác nhận.
    /// </param>
    /// <param name="request">
    /// - (EN) Confirmation dialog content.
    /// - (VI) Nội dung hộp thoại xác nhận.
    /// </param>
    public void RequestRestartAction(
        MainWindowViewModel viewModel,
        Action executeAction,
        DialogRequest request)
    {
        if (!viewModel.HasActiveGameProgress)
        {
            executeAction();
            return;
        }

        _pendingRestartAction = executeAction;
        ShowRestartConfirmationDialog(request);
    }

    /// <summary>
    /// - (EN) Displays the restart confirmation dialog with the provided request content.
    /// - (VI) Hiển thị hộp thoại xác nhận restart với nội dung được cung cấp.
    /// </summary>
    /// <param name="request">
    /// - (EN) Dialog content to display.
    /// - (VI) Nội dung dialog cần hiển thị.
    /// </param>
    public void ShowRestartConfirmationDialog(DialogRequest request)
    {
        _restartConfirmationTitle.Text = request.Title;
        _restartConfirmationMessage.Text = request.Message;

        _restartConfirmationOverlay.Visibility = Visibility.Visible;
        _confirmRestartButton.Focus();

        var fadeIn = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = TimeSpan.FromMilliseconds(160)
        };

        _restartConfirmationOverlay.BeginAnimation(UIElement.OpacityProperty, fadeIn);
    }

    /// <summary>
    /// - (EN) Hides the restart confirmation dialog and clears the pending action.
    /// - (VI) Ẩn hộp thoại xác nhận restart và xóa hành động đang chờ.
    /// </summary>
    public void CancelRestartConfirmation()
    {
        _pendingRestartAction = null;
        _restartConfirmationOverlay.Visibility = Visibility.Collapsed;
        _restartConfirmationOverlay.Opacity = 0;
    }

    /// <summary>
    /// - (EN) Confirms the pending restart action if one exists.
    /// - (VI) Xác nhận hành động restart đang chờ nếu có.
    /// </summary>
    public void ConfirmRestartConfirmation()
    {
        var action = _pendingRestartAction;

        _pendingRestartAction = null;
        _restartConfirmationOverlay.Visibility = Visibility.Collapsed;
        _restartConfirmationOverlay.Opacity = 0;

        action?.Invoke();
    }
}