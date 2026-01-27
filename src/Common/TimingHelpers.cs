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
    /// Adds a small artificial delay to increase chance of race conditions.
    /// </summary>
    public static void SmallDelay()
    {
        Thread.Sleep(1);
    }

    /// <summary>
    /// Adds a small artificial delay asynchronously.
    /// </summary>
    public static async Task SmallDelayAsync()
    {
        await Task.Delay(1);
    }
}
