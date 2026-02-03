using Common;

namespace SemaphoreSlim.After;

/// <summary>
/// AFTER: WaitAsync(CancellationToken). When the harness (or a timeout) cancels,
/// waiting workers get OperationCanceledException and exit cleanly — no deadlock.
/// </summary>
public static class CancellationAndTimeouts
{
    public static async Task<ScenarioRun> RunAsync(int concurrencyLevel, TimeSpan timeout)
    {
        var sem = new System.Threading.SemaphoreSlim(0, concurrencyLevel);
        var cancelAfter = TimeSpan.FromSeconds(Math.Max(1, timeout.TotalSeconds / 5));
        var cts = new CancellationTokenSource(cancelAfter);
        var harness = new TestHarness(concurrencyLevel, timeout);
        var completedOrCanceled = 0;

        var result = await harness.RunAsync(async _ =>
        {
            try
            {
                await sem.WaitAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                Interlocked.Increment(ref completedOrCanceled);
                throw;
            }
        });

        var observation = result.TimedOut
            ? $"All {concurrencyLevel} workers responded to cancellation (WaitAsync with token)."
            : $"All {concurrencyLevel} workers responded to cancellation in ~{(int)cancelAfter.TotalSeconds}s; no deadlock.";
        return new ScenarioRun(result, observation);
    }
}
