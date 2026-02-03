using System.Collections.Immutable;
using Common;

namespace ConcurrentDictionary.Before;

/// <summary>
/// BEFORE: Building ImmutableArray by repeated Add. Each Add allocates a new array;
/// for n items this is O(n²) time and n allocations. Inefficient.
/// </summary>
public static class BuilderPattern
{
    public static async Task<ScenarioRun> RunAsync(int concurrencyLevel, TimeSpan timeout)
    {
        var harness = new TestHarness(concurrencyLevel, timeout);
        var builtSize = 0;
        var allocationCount = 0;

        var result = await harness.RunAsync(threadId =>
        {
            var arr = ImmutableArray<int>.Empty;
            for (var i = 0; i < 200; i++)
            {
                arr = arr.Add(threadId * 1000 + i); // each Add = new allocation
                Interlocked.Increment(ref allocationCount);
            }
            Interlocked.Exchange(ref builtSize, arr.Length);
        });

        return new ScenarioRun(result,
            $"Built arrays of size {builtSize}; ~{allocationCount / concurrencyLevel} allocations per build (repeated Add = O(n²)).");
    }
}
