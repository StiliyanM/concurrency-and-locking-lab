using Common;

namespace SemaphoreSlim.After;

/// <summary>
/// AFTER: SemaphoreSlim limits concurrent access. Only MaxConcurrent can be inside at once.
/// </summary>
public static class GlobalThrottle
{
    private const int MaxConcurrent = 3;

    public static async Task<ScenarioRun> RunAsync(int concurrencyLevel, TimeSpan timeout)
    {
        var semaphore = new System.Threading.SemaphoreSlim(MaxConcurrent, MaxConcurrent);
        var maxConcurrent = 0;
        var current = 0;
        var harness = new TestHarness(concurrencyLevel, timeout);

        var result = await harness.RunAsync(_ =>
        {
            semaphore.Wait();
            try
            {
                var n = Interlocked.Increment(ref current);
                var prev = maxConcurrent;
                while (n > prev && Interlocked.CompareExchange(ref maxConcurrent, n, prev) != prev)
                    prev = maxConcurrent;
                TimingHelpers.SmallDelay();
            }
            finally
            {
                Interlocked.Decrement(ref current);
                semaphore.Release();
            }
        });

        return new ScenarioRun(result,
            $"Max concurrent: {maxConcurrent} (throttled to {MaxConcurrent}).");
    }
}
