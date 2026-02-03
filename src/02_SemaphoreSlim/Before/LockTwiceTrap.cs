using Common;

namespace SemaphoreSlim.Before;

/// <summary>
/// BEFORE: SemaphoreSlim is not reentrant. Same thread acquiring twice without release deadlocks.
/// We run a single worker that acquires, then tries to acquire again (e.g. via nested call).
/// </summary>
public static class LockTwiceTrap
{
    public static async Task<ScenarioRun> RunAsync(int concurrencyLevel, TimeSpan timeout)
    {
        var sem = new System.Threading.SemaphoreSlim(1, 1);
        var harness = new TestHarness(1, timeout); // One worker that will deadlock

        var result = await harness.RunAsync(_ =>
        {
            sem.Wait(); // First acquire
            try
            {
                sem.Wait(); // Second acquire on same thread → deadlock (non-reentrant)
            }
            finally
            {
                sem.Release();
            }
            sem.Release();
        });

        var observation = result.TimedOut
            ? "Deadlock: same thread tried to acquire SemaphoreSlim twice (non-reentrant)."
            : "Completed (unexpected).";
        return new ScenarioRun(result, observation);
    }
}
