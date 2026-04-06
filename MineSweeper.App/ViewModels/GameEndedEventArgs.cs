using MineSweeper.Core.Models;

namespace MineSweeper.App.ViewModels;

/// <summary>
/// - (EN) Provides data for a game-ended notification raised by the view model.
/// - (VI) Cung cấp dữ liệu cho thông báo kết thúc game được phát ra từ view model.
/// </summary>
public class GameEndedEventArgs : EventArgs
{
    /// <summary>
    /// - (EN) Initializes a new instance of the <see cref="GameEndedEventArgs"/> class.
    /// - (VI) Khởi tạo một instance mới của <see cref="GameEndedEventArgs"/>.
    /// </summary>
    /// <param name="state">- (EN) Final game state / (VI) Trạng thái cuối cùng của game</param>
    public GameEndedEventArgs(GameState state)
    {
        State = state;
    }

    /// <summary>
    /// - (EN) Gets the final game state.
    /// - (VI) Lấy trạng thái cuối cùng của game.
    /// </summary>
    public GameState State { get; }
}