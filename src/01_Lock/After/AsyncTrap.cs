using Common;

namespace Lock.After;

/// <summary>
/// AFTER: Don't use lock with async. Use SemaphoreSlim for async-compatible gating.
/// Always acquire/release in same scope and use WaitAsync (never block inside lock).
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
                await Task.Delay(1);
                TimingHelpers.SmallDelay();
            }
            finally
            {
                Gate.Release();
            }
        });

        return new ScenarioRun(result, "SemaphoreSlim + WaitAsync + Release in finally — no lock + async.");
    }
}
