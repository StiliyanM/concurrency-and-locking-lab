using Common;

namespace SemaphoreSlim.After;

/// <summary>
/// AFTER: Don't acquire the same semaphore twice. Use a reentrant lock (Monitor) when
/// you need same-thread re-entry, or restructure so inner work doesn't need the semaphore.
/// Here we restructure: only one Wait/Release scope; inner logic doesn't re-enter.
/// </summary>
public static class LockTwiceTrap
{
    public static async Task<ScenarioRun> RunAsync(int concurrencyLevel, TimeSpan timeout)
    {
        var sem = new System.Threading.SemaphoreSlim(1, 1);
        var harness = new TestHarness(concurrencyLevel, timeout);

        var result = await harness.RunAsync(_ =>
        {
            sem.Wait();
            try
            {
                // Do all work inside one critical section; no nested Wait
                TimingHelpers.SmallDelay();
            }
            finally
            {
                sem.Release();
            }
        });

        return new ScenarioRun(result,
            "Single Wait/Release scope; no nested acquire (SemaphoreSlim is not reentrant).");
    }
}
