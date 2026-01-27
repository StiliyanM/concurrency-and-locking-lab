using Common;

namespace Lock.Before;

/// <summary>
/// BEFORE: lock + blocking wait. Thread holds lock, starts a Task that needs the same lock,
/// then blocks on that Task → deadlock.
/// </summary>
public static class AsyncTrap
{
    private static readonly object LockObj = new();

    public static async Task<ScenarioRun> RunAsync(int concurrencyLevel, TimeSpan timeout)
    {
        var harness = new TestHarness(concurrencyLevel, timeout);

        var result = await harness.RunAsync(_ =>
        {
            lock (LockObj)
            {
                var t = Task.Run(() =>
                {
                    lock (LockObj)
                    {
                        TimingHelpers.SmallDelay();
                    }
                });
                t.Wait(); // Deadlock: we hold lock, task needs lock, we block on task
            }
        });

        var observation = result.TimedOut
            ? "Deadlock: lock + blocking wait (Task.Wait) on work that needs same lock."
            : "Completed.";
        return new ScenarioRun(result, observation);
    }
}
