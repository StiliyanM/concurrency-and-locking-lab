using Common;

namespace Lock.After;

/// <summary>
/// AFTER: Don't use lock with async. Use SemaphoreSlim for async-compatible gating.
/// Always acquire/release in same scope and use WaitAsync (never block inside lock).
/// This \"after\" path is intentionally very fast so it clearly contrasts with the deadlocking \"before\".
/// </summary>
public static class AsyncTrap
{
    private static readonly SemaphoreSlim Gate = new(1, 1);

    public static async Task<ScenarioRun> RunAsync(int concurrencyLevel, TimeSpan timeout)
    {
        var harness = new TestHarness(concurrencyLevel, timeout);

        var result = await harness.RunAsync(async _ =>
        {
            await Gate.WaitAsync();
            try
            {
                // Intentionally minimal work: just simulate a tiny async critical section.
                // The key teaching point is that we can \"wait\" asynchronously without blocking a lock.
                await Task.Yield();
            }
            finally
            {
                Gate.Release();
            }
        });

        return new ScenarioRun(result, "SemaphoreSlim + WaitAsync + Release in finally — no lock + async.");
    }
}
