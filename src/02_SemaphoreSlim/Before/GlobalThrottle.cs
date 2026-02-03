using Common;

namespace SemaphoreSlim.Before;

/// <summary>
/// BEFORE: No throttling. All workers run at once; under load this can overload a shared resource.
/// </summary>
public static class GlobalThrottle
{
    public static async Task<ScenarioRun> RunAsync(int concurrencyLevel, TimeSpan timeout)
    {
        var maxConcurrent = 0;
        var current = 0;
        var harness = new TestHarness(concurrencyLevel, timeout);

        var result = await harness.RunAsync(_ =>
        {
            var n = Interlocked.Increment(ref current);
            var prev = maxConcurrent;
            while (n > prev && Interlocked.CompareExchange(ref maxConcurrent, n, prev) != prev)
                prev = maxConcurrent;
            TimingHelpers.SmallDelay();
            Interlocked.Decrement(ref current);
        });

        return new ScenarioRun(result,
            $"Max concurrent: {maxConcurrent} (no throttle; all {concurrencyLevel} could run at once).");
    }
}
