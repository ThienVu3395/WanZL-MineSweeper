using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using MineSweeper.App.ViewModels;

namespace MineSweeper.App.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Gắn ViewModel cho Window để XAML binding hoạt động
            // Nghĩa là: Toàn bộ binding trong MainWindow.xaml sẽ lấy dữ liệu từ MainWindowViewModel
            var vm = new MainWindowViewModel();
            DataContext = vm;

            vm.PropertyChanged += Vm_PropertyChanged;
        }

        /// <summary>
        /// Handles right-click on a cell and forwards it to the ViewModel.
        /// - Đây là code-behind phải xử lý riêng theo kiểu dùng event → forward về ViewModel
        /// - Vì không bind right-click command dễ như left-click
        /// </summary>
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

        private void Vm_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainWindowViewModel.Message))
            {
                ShowToast();
            }
        }

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
        }
    }
}