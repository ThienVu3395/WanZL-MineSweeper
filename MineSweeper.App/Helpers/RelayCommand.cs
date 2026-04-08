using System.Windows.Input;

namespace MineSweeper.App.Helpers
{
    /// <summary>
    /// - (EN) Provides a basic ICommand implementation to handle UI actions in MVVM pattern.
    /// - (VI) Cung cấp một triển khai ICommand cơ bản để xử lý các hành động UI theo mô hình MVVM.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        /// <summary>
        /// - (EN) Initializes a new instance of the RelayCommand class with execute and optional canExecute delegates.
        /// - (VI) Khởi tạo một instance mới của RelayCommand với delegate execute và canExecute (tùy chọn).
        /// </summary>
        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// - (EN) Occurs when changes affect whether the command should execute.
        /// - (VI) Xảy ra khi có thay đổi ảnh hưởng đến việc command có thể thực thi hay không.
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// - (EN) Determines whether the command can execute with the given parameter.
        /// - (VI) Xác định command có thể thực thi với tham số đã cho hay không.
        /// </summary>
        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        /// <summary>
        /// - (EN) Executes the command action with the given parameter.
        /// - (VI) Thực thi hành động của command với tham số đã cho.
        /// </summary>
        public void Execute(object? parameter)
        {
            _execute(parameter);
        }

        /// <summary>
        /// - (EN) Raises the CanExecuteChanged event to notify the UI to re-evaluate command availability.
        /// - (VI) Kích hoạt sự kiện CanExecuteChanged để yêu cầu UI đánh giá lại khả năng thực thi của command.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
