using Common;

namespace Lock.Before;

/// <summary>
/// BEFORE: Two locks acquired in different order by different flows → deadlock.
/// Half of the threads take lockA then lockB, half take lockB then lockA.
/// </summary>
public static class LockOrderingDeadlock
{
    private static readonly object LockA = new();
    private static readonly object LockB = new();

    public static async Task<ScenarioRun> RunAsync(int concurrencyLevel, TimeSpan timeout)
    {
        var harness = new TestHarness(concurrencyLevel, timeout);

        var result = await harness.RunAsync(threadId =>
        {
            if (threadId % 2 == 0)
            {
                lock (LockA)
                {
                    TimingHelpers.SmallDelay();
                    lock (LockB)
                    {
                        TimingHelpers.SmallDelay();
                    }
                }
            }
            else
            {
                lock (LockB)
                {
                    TimingHelpers.SmallDelay();
                    lock (LockA)
                    {
                        TimingHelpers.SmallDelay();
                    }
                }
            }
        });

        var observation = result.TimedOut
            ? "Deadlock detected (timeout). Different lock order causes circular wait."
            : "Completed (unlikely under load).";
        return new ScenarioRun(result, observation);
    }
}
