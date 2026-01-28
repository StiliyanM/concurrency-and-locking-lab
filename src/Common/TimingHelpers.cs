using System.Diagnostics;

namespace Common;

/// <summary>
/// Helpers for timing and latency measurement.
/// </summary>
public static class TimingHelpers
{
    /// <summary>
    /// Measures the execution time of an action.
    /// </summary>
    public static TimeSpan Measure(Action action)
    {
        var sw = Stopwatch.StartNew();
        action();
        sw.Stop();
        return sw.Elapsed;
    }

    /// <summary>
    /// Measures the execution time of an async function.
    /// </summary>
    public static async Task<TimeSpan> MeasureAsync(Func<Task> asyncAction)
    {
        var sw = Stopwatch.StartNew();
        await asyncAction();
        sw.Stop();
        return sw.Elapsed;
    }

    /// <summary>
    /// Yields briefly to increase chance of race conditions (faster than Sleep(1)).
    /// </summary>
    public static void SmallDelay()
    {
        Thread.Sleep(0);
    }

    /// <summary>
    /// Yields asynchronously to increase interleaving (faster than Delay(1)).
    /// </summary>
    public static async Task SmallDelayAsync()
    {
        await Task.Yield();
    }
}
