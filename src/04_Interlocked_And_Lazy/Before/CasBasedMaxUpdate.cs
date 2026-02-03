using Common;

namespace InterlockedAndLazy.Before;

/// <summary>
/// BEFORE: Read max, compare, write. Non-atomic; another thread can update between read and write,
/// so we overwrite with a smaller value and lose the true max.
/// </summary>
public static class CasBasedMaxUpdate
{
    public static async Task<ScenarioRun> RunAsync(int concurrencyLevel, TimeSpan timeout)
    {
        var max = 0;
        var harness = new TestHarness(concurrencyLevel, timeout);

        var result = await harness.RunAsync(threadId =>
        {
            var value = threadId * 10;
            if (value > max)
            {
                TimingHelpers.SmallDelay(); // race window
                max = value;
            }
        });

        return new ScenarioRun(result,
            $"Max: {max} (expected {(concurrencyLevel - 1) * 10}; non-atomic read-modify-write can lose updates).");
    }
}
