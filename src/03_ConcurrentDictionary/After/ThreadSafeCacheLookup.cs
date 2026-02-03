using System.Collections.Concurrent;
using Common;

namespace ConcurrentDictionary.After;

/// <summary>
/// AFTER: ConcurrentDictionary. GetOrAdd is atomic; no races, no exceptions.
/// Thread-safe by design.
/// </summary>
public static class ThreadSafeCacheLookup
{
    private static readonly ConcurrentDictionary<int, int> Cache = new();

    public static async Task<ScenarioRun> RunAsync(int concurrencyLevel, TimeSpan timeout)
    {
        Cache.Clear();
        var harness = new TestHarness(concurrencyLevel, timeout);
        var lookupCount = 0;

        var result = await harness.RunAsync(threadId =>
        {
            var key = threadId % 5;
            var value = Cache.GetOrAdd(key, k => k * 10); // atomic; no race
            Interlocked.Increment(ref lookupCount);
        });

        return new ScenarioRun(result,
            $"Lookups: {lookupCount}, cache size: {Cache.Count} (ConcurrentDictionary = thread-safe).");
    }
}
