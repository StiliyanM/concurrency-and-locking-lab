using System.Collections.Generic;
using Common;

namespace ConcurrentDictionary.Before;

/// <summary>
/// BEFORE: Plain Dictionary with no lock. Multiple threads doing TryGetValue + Add causes
/// races: duplicate key exceptions, corrupted state, or wrong values. Not thread-safe.
/// </summary>
public static class ThreadSafeCacheLookup
{
    private static readonly Dictionary<int, int> Cache = new();

    public static async Task<ScenarioRun> RunAsync(int concurrencyLevel, TimeSpan timeout)
    {
        Cache.Clear();
        var harness = new TestHarness(concurrencyLevel, timeout);
        var lookupCount = 0;

        var result = await harness.RunAsync(threadId =>
        {
            var key = threadId % 5;
            if (!Cache.TryGetValue(key, out var value))
            {
                value = key * 10; // simulate load
                TimingHelpers.SmallDelay(); // widen race window
                Cache.Add(key, value); // Add throws ArgumentException if key exists (race); indexer would corrupt
            }
            Interlocked.Increment(ref lookupCount);
        });

        var observation = result.Exceptions.Count > 0
            ? $"Exceptions: {result.Exceptions.Count} (Dictionary without lock — not thread-safe)."
            : $"Lookups: {lookupCount}, cache size: {Cache.Count} (races possible; may get wrong values).";
        return new ScenarioRun(result, observation);
    }
}
