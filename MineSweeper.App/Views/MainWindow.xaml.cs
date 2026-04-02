using System.Windows;
using System.Windows.Input;
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
            DataContext = new MainWindowViewModel();
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
                vm.ToggleFlagCommand.Execute(cellVm);
            }
        }
    }
}