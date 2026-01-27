using Common;

namespace Lock.After;

/// <summary>
/// AFTER: lock around the critical section makes read-modify-write atomic.
/// </summary>
public static class LostUpdates
{
    private static readonly object LockObj = new();

    public static async Task<ScenarioRun> RunAsync(int concurrencyLevel, TimeSpan timeout)
    {
        var counter = 0;
        var harness = new TestHarness(concurrencyLevel, timeout);

        var result = await harness.RunAsync(_ =>
        {
            lock (LockObj)
            {
                var temp = counter;
                TimingHelpers.SmallDelay();
                counter = temp + 1;
            }
        });

        return new ScenarioRun(result, $"Counter: {counter} (expected {concurrencyLevel})");
    }
}
