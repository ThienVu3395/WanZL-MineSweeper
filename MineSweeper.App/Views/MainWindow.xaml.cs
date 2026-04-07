using MineSweeper.App.Helpers;
using MineSweeper.App.ViewModels;
using MineSweeper.Core.Models;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MineSweeper.App.Views
{
    /// <summary>
    /// - (EN) Interaction logic for MainWindow.xaml.
    /// Acts as the View layer in MVVM, responsible for handling UI events
    /// that cannot be directly bound to commands (e.g., right-click, double-click),
    /// and forwarding them to the ViewModel.
    /// - (VI) Logic tương tác cho MainWindow.xaml.
    /// Đóng vai trò là View trong mô hình MVVM, chịu trách nhiệm xử lý các sự kiện UI
    /// không thể bind trực tiếp bằng command (ví dụ: right-click, double-click),
    /// và chuyển tiếp chúng về ViewModel.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// - (EN) Initializes a new instance of the <see cref="MainWindow"/> class.
        /// Sets up the ViewModel and subscribes to its events.
        /// - (VI) Khởi tạo một instance mới của <see cref="MainWindow"/>.
        /// Thiết lập ViewModel và đăng ký các event cần thiết.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            var vm = new MainWindowViewModel();
            DataContext = vm;

            vm.PropertyChanged += Vm_PropertyChanged;
            vm.GameEnded += OnGameEnded;
            vm.NewBestTimeAchieved += OnNewBestTimeAchieved;
        }

        /// <summary>
        /// - (EN) Handles right-click on a cell and forwards the flag toggle action to the ViewModel.
        /// This is implemented in code-behind because right-click binding is not straightforward in XAML.
        /// - (VI) Xử lý thao tác click chuột phải trên một ô và chuyển tiếp hành động toggle flag về ViewModel.
        /// Được xử lý trong code-behind vì việc bind right-click trong XAML không đơn giản như left-click.
        /// </summary>
        /// <param name="sender">- (EN) UI element that triggered the event / (VI) Phần tử UI phát sinh sự kiện</param>
        /// <param name="e">- (EN) Mouse event arguments / (VI) Tham số sự kiện chuột</param>
        private void Cell_RightClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is not MainWindowViewModel vm)
                return;

            if (sender is FrameworkElement element && element.DataContext is CellViewModel cellVm)
            {
                if (vm.ToggleFlagCommand.CanExecute(cellVm))
                {
                    vm.ToggleFlagCommand.Execute(cellVm);
                }
            }

            e.Handled = true;
        }

        /// <summary>
        /// - (EN) Handles double-click on a revealed cell and forwards the chord action to the ViewModel.
        /// - (VI) Xử lý thao tác double-click trên ô đã mở và chuyển lệnh chord về ViewModel.
        /// </summary>
        /// <param name="sender">- (EN) UI element that triggered the event / (VI) Phần tử UI phát sinh sự kiện</param>
        /// <param name="e">- (EN) Mouse event arguments / (VI) Tham số sự kiện chuột</param>
        private void Cell_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is not MainWindowViewModel vm)
                return;

            if (sender is FrameworkElement element && element.DataContext is CellViewModel cellVm)
            {
                if (vm.ChordCellCommand.CanExecute(cellVm))
                {
                    vm.ChordCellCommand.Execute(cellVm);
                }
            }

            e.Handled = true;
        }

        /// <summary>
        /// - (EN) Handles PropertyChanged events from the ViewModel.
        /// Triggers UI behaviors such as showing toast notifications when Message changes.
        /// - (VI) Xử lý sự kiện PropertyChanged từ ViewModel.
        /// Kích hoạt các hành vi UI như hiển thị toast khi Message thay đổi.
        /// </summary>
        /// <param name="sender">- (EN) Event sender / (VI) Đối tượng phát sự kiện</param>
        /// <param name="e">- (EN) Property changed event arguments / (VI) Tham số sự kiện thay đổi thuộc tính</param>
        private void Vm_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainWindowViewModel.Message))
            {
                ShowToast();
            }
        }

        /// <summary>
        /// - (EN) Handles the game-ended event raised by the view model and shows the custom endgame dialog.
        /// - (VI) Xử lý sự kiện kết thúc game được phát ra từ view model và hiển thị hộp thoại kết thúc tùy biến.
        /// </summary>
        /// <param name="sender">
        /// - (EN) Event sender.
        /// - (VI) Đối tượng phát sự kiện.
        /// </param>
        /// <param name="e">
        /// - (EN) Event arguments containing the final game state.
        /// - (VI) Dữ liệu sự kiện chứa trạng thái cuối cùng của game.
        /// </param>
        private void OnGameEnded(object? sender, GameEndedEventArgs e)
        {
            ShowEndGameDialog(e.State);
        }

        /// <summary>
        /// - (EN) Handles the new-best-time notification raised by the view model
        /// and posts a toast-style message to the player.
        /// - (VI) Xử lý thông báo best time mới được phát ra từ view model
        /// và gửi thông báo dạng toast cho người chơi.
        /// </summary>
        /// <param name="sender">
        /// - (EN) Event sender.
        /// - (VI) Đối tượng phát sự kiện.
        /// </param>
        /// <param name="e">
        /// - (EN) Event data containing the new best-time information.
        /// - (VI) Dữ liệu sự kiện chứa thông tin best time mới.
        /// </param>
        private void OnNewBestTimeAchieved(object? sender, NewBestTimeEventArgs e)
        {
            if (DataContext is not MainWindowViewModel vm)
                return;

            string recordTypeText = e.IsFirstRecord
                ? "🎉 First record!"
                : "🔥 New best time!";

            vm.ShowTemporaryMessage(
                $"🏆 {recordTypeText}: {TimeFormatHelper.Format(e.BestTime)} ({e.Difficulty.ToString().ToUpper()})");
        }

        /// <summary>
        /// - (EN) Displays the custom endgame dialog with content based on the final game state.
        /// - (VI) Hiển thị hộp thoại kết thúc game tùy biến với nội dung dựa trên trạng thái cuối cùng của game.
        /// </summary>
        /// <param name="state">
        /// - (EN) The final game state.
        /// - (VI) Trạng thái cuối cùng của game.
        /// </param>
        private void ShowEndGameDialog(GameState state)
        {
            if (state == GameState.Won)
            {
                EndGameDialogTitle.Text = "🎉 You won!";
                EndGameDialogMessage.Text = "Great job! You cleared all safe cells on the board.";
                EndGameDialogTitle.Foreground = new SolidColorBrush(Color.FromRgb(34, 139, 34));
            }
            else
            {
                EndGameDialogTitle.Text = "💥 Game over";
                EndGameDialogMessage.Text = "You revealed a mine. Try again and beat your best time!";
                EndGameDialogTitle.Foreground = new SolidColorBrush(Color.FromRgb(178, 34, 34));
            }

            EndGameDialogOverlay.Visibility = Visibility.Visible;
            PlayAgainButton.Focus();

            var fadeIn = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(180)
            };

            EndGameDialogOverlay.BeginAnimation(OpacityProperty, fadeIn);
        }

        /// <summary>
        /// - (EN) Hides the custom endgame dialog.
        /// - (VI) Ẩn hộp thoại kết thúc game tùy biến.
        /// </summary>
        private void HideEndGameDialog()
        {
            EndGameDialogOverlay.Visibility = Visibility.Collapsed;
            EndGameDialogOverlay.Opacity = 0;
        }

        /// <summary>
        /// - (EN) Starts a new game from the custom endgame dialog.
        /// - (VI) Bắt đầu một ván mới từ hộp thoại kết thúc game tùy biến.
        /// </summary>
        /// <param name="sender">
        /// - (EN) Event sender.
        /// - (VI) Đối tượng phát sự kiện.
        /// </param>
        /// <param name="e">
        /// - (EN) Routed event data.
        /// - (VI) Dữ liệu sự kiện routed.
        /// </param>
        private void PlayAgainButton_Click(object sender, RoutedEventArgs e)
        {
            HideEndGameDialog();

            if (DataContext is MainWindowViewModel viewModel &&
                viewModel.NewGameCommand.CanExecute(null))
            {
                viewModel.NewGameCommand.Execute(null);
            }
        }

        /// <summary>
        /// - (EN) Closes the custom endgame dialog without starting a new game.
        /// - (VI) Đóng hộp thoại kết thúc game tùy biến mà không bắt đầu ván mới.
        /// </summary>
        /// <param name="sender">
        /// - (EN) Event sender.
        /// - (VI) Đối tượng phát sự kiện.
        /// </param>
        /// <param name="e">
        /// - (EN) Routed event data.
        /// - (VI) Dữ liệu sự kiện routed.
        /// </param>
        private void CloseDialogButton_Click(object sender, RoutedEventArgs e)
        {
            HideEndGameDialog();
        }

        /// <summary>
        /// - (EN) Displays a toast notification using fade-in and fade-out animations.
        /// Automatically hides the toast if there is no message.
        /// - (VI) Hiển thị toast notification với animation fade-in và fade-out.
        /// Tự động ẩn toast nếu không có nội dung.
        /// </summary>
        private void ShowToast()
        {
            if (DataContext is MainWindowViewModel vm && string.IsNullOrWhiteSpace(vm.Message))
            {
                ToastMessage.BeginAnimation(OpacityProperty, null);
                ToastMessage.Opacity = 0;
                return;
            }

            ToastMessage.BeginAnimation(OpacityProperty, null);
            ToastMessage.Opacity = 0;

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
                if (DataContext is MainWindowViewModel currentVm)
                {
                    currentVm.ClearTemporaryMessage();
                }
            };

            ToastMessage.BeginAnimation(OpacityProperty, animation);
        }
    }
}