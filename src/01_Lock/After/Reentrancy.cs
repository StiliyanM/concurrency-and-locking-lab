using Common;

namespace Lock.After;

/// <summary>
/// AFTER: Monitor (lock) is reentrant. Same thread can enter the same lock multiple times.
/// </summary>
public static class Reentrancy
{
    private static readonly object LockObj = new();

    public static async Task<ScenarioRun> RunAsync(int concurrencyLevel, TimeSpan timeout)
    {
        var harness = new TestHarness(concurrencyLevel, timeout);

        var result = await harness.RunAsync(_ =>
        {
            lock (LockObj)
            {
                TimingHelpers.SmallDelay();
                Nested();
            }
        });

        return new ScenarioRun(result, "Monitor is reentrant — same thread re-entered the same lock.");
    }

    private static void Nested()
    {
        lock (LockObj)
        {
            TimingHelpers.SmallDelay();
        }
    }
}
