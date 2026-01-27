using Common;

namespace Lock.After;

/// <summary>
/// AFTER: Consistent lock ordering — always take LockA then LockB.
/// </summary>
public static class LockOrderingDeadlock
{
    private static readonly object LockA = new();
    private static readonly object LockB = new();

    public static async Task<ScenarioRun> RunAsync(int concurrencyLevel, TimeSpan timeout)
    {
        var harness = new TestHarness(concurrencyLevel, timeout);

        var result = await harness.RunAsync(_ =>
        {
            lock (LockA)
            {
                TimingHelpers.SmallDelay();
                lock (LockB)
                {
                    TimingHelpers.SmallDelay();
                }
            }
        });

        return new ScenarioRun(result, "Consistent order (A then B) avoids deadlock.");
    }
}
