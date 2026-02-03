using Common;

namespace InterlockedAndLazy.After;

/// <summary>
/// AFTER: Interlocked.Increment for atomic read-modify-write. No lost updates.
/// </summary>
public static class AtomicCounters
{
    public static async Task<ScenarioRun> RunAsync(int concurrencyLevel, TimeSpan timeout)
    {
        var counter = 0;
        var harness = new TestHarness(concurrencyLevel, timeout);

        var result = await harness.RunAsync(_ =>
        {
            Interlocked.Increment(ref counter);
        });

        return new ScenarioRun(result, $"Counter: {counter} (expected {concurrencyLevel}; Interlocked.Increment).");
    }
}
