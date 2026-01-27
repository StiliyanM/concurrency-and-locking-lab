using Common;

namespace Lock.Before;

/// <summary>
/// BEFORE: Using a non-reentrant primitive (SemaphoreSlim). Same thread awaiting
/// twice without release → deadlock. Simulates "lock twice" with a gate that has no re-entry.
/// </summary>
public static class Reentrancy
{
    public static async Task<ScenarioRun> RunAsync(int concurrencyLevel, TimeSpan timeout)
    {
        using var gate = new SemaphoreSlim(1, 1);
        var harness = new TestHarness(concurrencyLevel, timeout);

        var result = await harness.RunAsync(async _ =>
        {
            await gate.WaitAsync();
            try
            {
                TimingHelpers.SmallDelay();
                await Nested(gate);
            }
            finally
            {
                gate.Release();
            }
        });

        var observation = result.TimedOut
            ? "Same-thread re-entry deadlock (non-reentrant gate)."
            : "Completed.";
        return new ScenarioRun(result, observation);
    }

    private static async Task Nested(SemaphoreSlim gate)
    {
        await gate.WaitAsync(); // Same thread waits again → no Release in between → deadlock
        gate.Release();
    }
}
