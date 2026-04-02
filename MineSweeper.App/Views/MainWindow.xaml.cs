using System.Windows;
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
    }
}