using MineSweeper.App.Extensions;
using MineSweeper.App.Helpers;
using MineSweeper.App.Models;
using MineSweeper.App.Services;
using MineSweeper.App.ViewModels;
using MineSweeper.Core.Models;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

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
        private readonly MainWindowDialogService _dialogService;

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

            _dialogService = new MainWindowDialogService(
                ToastMessage,
                EndGameDialogOverlay,
                EndGameDialogTitle,
                EndGameDialogMessage,
                RestartConfirmationOverlay,
                RestartConfirmationTitle,
                RestartConfirmationMessage,
                ConfirmRestartButton,
                PlayAgainButton);

            vm.PropertyChanged += Vm_PropertyChanged;
            vm.GameEnded += OnGameEnded;
            vm.NewBestTimeAchieved += OnNewBestTimeAchieved;
        }

        #region Handlers
        /// <summary>
        /// - (EN) Handles the New Game button click from the header area.
        /// Routes the action through the restart confirmation flow when the current game has active progress.
        /// - (VI) Xử lý thao tác nhấn nút New Game ở phần header.
        /// Chuyển hành động này qua luồng xác nhận restart khi ván hiện tại đang có tiến trình.
        /// </summary>
        /// <param name="sender">
        /// - (EN) Event sender.
        /// - (VI) Đối tượng phát sự kiện.
        /// </param>
        /// <param name="e">
        /// - (EN) Routed event data.
        /// - (VI) Dữ liệu sự kiện routed.
        /// </param>
        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not MainWindowViewModel vm)
                return;

            _dialogService.RequestRestartAction(
                vm,
                executeAction: () =>
                {
                    if (vm.NewGameCommand.CanExecute(null))
                    {
                        vm.NewGameCommand.Execute(null);
                    }
                },
                request: new DialogRequest
                {
                    Title = "Start new game?",
                    Message = "Your current progress will be lost. Start a new game using the currently selected configuration?"
                });
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
        /// - (EN) Starts a quick restart from the custom endgame dialog.
        /// - (VI) Thực hiện restart nhanh từ hộp thoại kết thúc game tùy biến.
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
            _dialogService.HideEndGameDialog();

            if (DataContext is MainWindowViewModel viewModel &&
                viewModel.QuickRestartCommand.CanExecute(null))
            {
                viewModel.QuickRestartCommand.Execute(null);
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
            _dialogService.HideEndGameDialog();
        }

        /// <summary>
        /// - (EN) Confirms the pending restart-related action and executes it.
        /// - (VI) Xác nhận hành động liên quan đến restart đang chờ và thực thi nó.
        /// </summary>
        /// <param name="sender">
        /// - (EN) Event sender.
        /// - (VI) Đối tượng phát sự kiện.
        /// </param>
        /// <param name="e">
        /// - (EN) Routed event data.
        /// - (VI) Dữ liệu sự kiện routed.
        /// </param>
        private void ConfirmRestartButton_Click(object sender, RoutedEventArgs e)
        {
            _dialogService.ConfirmRestartConfirmation();
        }

        /// <summary>
        /// - (EN) Cancels the pending restart-related action.
        /// - (VI) Hủy hành động liên quan đến restart đang chờ.
        /// </summary>
        /// <param name="sender">
        /// - (EN) Event sender.
        /// - (VI) Đối tượng phát sự kiện.
        /// </param>
        /// <param name="e">
        /// - (EN) Routed event data.
        /// - (VI) Dữ liệu sự kiện routed.
        /// </param>
        private void CancelRestartButton_Click(object sender, RoutedEventArgs e)
        {
            _dialogService.CancelRestartConfirmation();
        }

        /// <summary>
        /// - (EN) Handles keyboard shortcuts for restart and difficulty actions.
        /// - (VI) Xử lý các phím tắt bàn phím cho thao tác restart và đổi độ khó.
        /// </summary>
        /// <param name="sender">
        /// - (EN) Event sender.
        /// - (VI) Đối tượng phát sự kiện.
        /// </param>
        /// <param name="e">
        /// - (EN) Key event data.
        /// - (VI) Dữ liệu sự kiện bàn phím.
        /// </param>
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (_dialogService.IsRestartConfirmationVisible)
            {
                if (e.Key == Key.Enter)
                {
                    ConfirmRestartButton_Click(this, new RoutedEventArgs());
                    e.Handled = true;
                }
                else if (e.Key == Key.Escape)
                {
                    CancelRestartButton_Click(this, new RoutedEventArgs());
                    e.Handled = true;
                }

                return;
            }

            if (Keyboard.Modifiers == ModifierKeys.None && e.Key == Key.F2)
            {
                if (DataContext is MainWindowViewModel vm)
                {
                    _dialogService.RequestRestartAction(
                        vm,
                        executeAction: () =>
                        {
                            if (vm.QuickRestartCommand.CanExecute(null))
                            {
                                vm.QuickRestartCommand.Execute(null);
                            }
                        },
                        request: new DialogRequest
                        {
                            Title = "Restart current game?",
                            Message = "Your current progress will be lost. Restart the current game with the same configuration?"
                        });
                }

                e.Handled = true;
                return;
            }

            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (TryHandleDifficultyShortcut(e.Key))
                {
                    e.Handled = true;
                }
            }
        }
        #endregion

        #region Events
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
            if (e.PropertyName == nameof(MainWindowViewModel.Message) && 
                DataContext is MainWindowViewModel vm)
            {
                _dialogService.ShowToast(vm);
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
            _dialogService.ShowEndGameDialog(e.State);
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
        #endregion

        #region Private Helpers
        /// <summary>
        /// - (EN) Tries to handle a difficulty shortcut key and start a new game with the matching difficulty.
        /// - (VI) Thử xử lý một phím tắt đổi độ khó và bắt đầu ván mới với độ khó tương ứng.
        /// </summary>
        /// <param name="key">
        /// - (EN) The pressed key.
        /// - (VI) Phím vừa được nhấn.
        /// </param>
        /// <returns>
        /// - (EN) True if the key was recognized as a supported difficulty shortcut.
        /// - (VI) True nếu phím được nhận diện là một phím tắt độ khó được hỗ trợ.
        /// </returns>
        private bool TryHandleDifficultyShortcut(Key key)
        {
            DifficultyLevel? targetDifficulty = key switch
            {
                Key.D1 or Key.NumPad1 => DifficultyLevel.Beginner,
                Key.D2 or Key.NumPad2 => DifficultyLevel.Intermediate,
                Key.D3 or Key.NumPad3 => DifficultyLevel.Expert,
                Key.D4 or Key.NumPad4 => DifficultyLevel.Custom,
                _ => null
            };

            if (targetDifficulty is null)
            {
                return false;
            }

            if (DataContext is not MainWindowViewModel vm)
                return false;

            _dialogService.RequestRestartAction(
                vm,
                executeAction: () =>
                {
                    vm.StartNewGameForDifficulty(targetDifficulty.Value);
                },
                request: new DialogRequest
                {
                    Title = "Change difficulty?",
                    Message = $"Your current progress will be lost. Switch to {targetDifficulty.Value.ToDisplayString()} and start a new game?"
                });

            return true;
        }
        #endregion
    }
}