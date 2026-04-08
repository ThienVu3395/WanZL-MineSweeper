using System.Runtime.ExceptionServices;

namespace MineSweeper.Tests;

/// <summary>
/// - (EN) Provides a helper to execute test code on an STA thread for WPF-related unit tests.
/// - (VI) Cung cấp helper để thực thi mã test trên STA thread cho các unit test liên quan đến WPF.
/// </summary>
public static class TestStaHelper
{
    /// <summary>
    /// - (EN) Runs the specified test action on a dedicated STA thread and rethrows any exception.
    /// - (VI) Chạy action test được chỉ định trên một STA thread riêng và ném lại mọi ngoại lệ nếu có.
    /// </summary>
    /// <param name="testAction">
    /// - (EN) The test logic to execute.
    /// - (VI) Logic test cần thực thi.
    /// </param>
    public static void Run(Action testAction)
    {
        Exception? capturedException = null;

        var thread = new Thread(() =>
        {
            try
            {
                testAction();
            }
            catch (Exception ex)
            {
                capturedException = ex;
            }
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();

        if (capturedException is not null)
        {
            ExceptionDispatchInfo.Capture(capturedException).Throw();
        }
    }
}