using Common;

namespace InterlockedAndLazy.After;

/// <summary>
/// AFTER: Interlocked.CompareExchange in a loop. Atomically update max only if current is smaller.
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
            var current = max;
            while (value > current)
            {
                var replaced = Interlocked.CompareExchange(ref max, value, current);
                if (replaced == current)
                    break;
                current = replaced;
            }
        });

        return new ScenarioRun(result,
            $"Max: {max} (expected {(concurrencyLevel - 1) * 10}; CAS loop = atomic).");
    }
}
