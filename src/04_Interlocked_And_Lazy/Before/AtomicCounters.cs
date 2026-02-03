using Common;

namespace InterlockedAndLazy.Before;

/// <summary>
/// BEFORE: Non-atomic increment. Read-modify-write without Interlocked causes lost updates.
/// </summary>
public static class AtomicCounters
{
    public static async Task<ScenarioRun> RunAsync(int concurrencyLevel, TimeSpan timeout)
    {
        var counter = 0;
        var harness = new TestHarness(concurrencyLevel, timeout);

        var result = await harness.RunAsync(_ =>
        {
            var temp = counter;
            TimingHelpers.SmallDelay();
            counter = temp + 1;
        });

        return new ScenarioRun(result, $"Counter: {counter} (expected {concurrencyLevel}; lost updates).");
    }
}
