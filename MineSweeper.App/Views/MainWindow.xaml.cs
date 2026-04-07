using MineSweeper.App.Helpers;
using MineSweeper.App.ViewModels;
using MineSweeper.Core.Models;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
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

            // Gắn ViewModel cho Window để XAML binding hoạt động
            // Nghĩa là: Toàn bộ binding trong MainWindow.xaml sẽ lấy dữ liệu từ MainWindowViewModel
            var vm = new MainWindowViewModel();
            DataContext = vm;

            vm.PropertyChanged += Vm_PropertyChanged;

            vm.GameEnded += Vm_GameEnded;

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
        /// - (EN) Handles endgame notifications raised by the ViewModel and displays corresponding dialogs.
        /// - (VI) Xử lý thông báo kết thúc game từ ViewModel và hiển thị dialog tương ứng.
        /// </summary>
        /// <param name="sender">- (EN) Event sender / (VI) Đối tượng phát sự kiện</param>
        /// <param name="e">- (EN) Game ended event data / (VI) Dữ liệu sự kiện kết thúc game</param>
        private void Vm_GameEnded(object? sender, GameEndedEventArgs e)
        {
            if (e.State == GameState.Won)
            {
                MessageBox.Show("Congratulations! You cleared all cells!", "Victory 🎉");
            }
            else if (e.State == GameState.Lost)
            {
                MessageBox.Show("Boom! You hit a mine.", "Game Over 💥");
            }
        }

        /// <summary>
        /// - (EN) Handles the new-best-time notification raised by the view model
        /// and displays a toast-style message to the player.
        /// - (VI) Xử lý thông báo best time mới được phát ra từ view model
        /// và hiển thị một thông báo dạng toast cho người chơi.
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

            string recordTypeText = e.IsFirstRecord ? "🎉 First record!" : "🔥 New best time!";

            vm.ShowTemporaryMessage($"🏆 {recordTypeText}: {TimeFormatHelper.Format(e.BestTime)} ({e.Difficulty.ToString().ToUpper()})");

            ShowToast();
        }

        /// <summary>
        /// - (EN) Displays a toast notification using fade-in and fade-out animations.
        /// Automatically hides the toast if there is no message.
        /// - (VI) Hiển thị toast notification với animation fade-in và fade-out.
        /// Tự động ẩn toast nếu không có nội dung.
        /// </summary>
        private void ShowToast()
        {
            // Nếu không có nội dung thì ẩn toast ngay
            if (DataContext is MainWindowViewModel vm && string.IsNullOrWhiteSpace(vm.Message))
            {
                ToastMessage.BeginAnimation(OpacityProperty, null);
                ToastMessage.Opacity = 0;
                return;
            }

            // Hủy animation cũ
            ToastMessage.BeginAnimation(OpacityProperty, null);
            ToastMessage.Opacity = 0;

            var animation = new DoubleAnimationUsingKeyFrames();

            // Bắt đầu từ 0
            animation.KeyFrames.Add(
                new DiscreteDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.Zero)));

            // Fade in nhanh
            animation.KeyFrames.Add(
                new EasingDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(200)))
                {
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                });

            // Giữ nguyên một lúc
            animation.KeyFrames.Add(
                new DiscreteDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(3))));

            // Fade out mượt
            animation.KeyFrames.Add(
                new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(3.9)))
                {
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                });

            ToastMessage.BeginAnimation(OpacityProperty, animation);

            animation.Completed += (_, _) =>
            {
                if (DataContext is MainWindowViewModel vm)
                {
                    vm.ClearTemporaryMessage();
                }
            };
        }
    }
}