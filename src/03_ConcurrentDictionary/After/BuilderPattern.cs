using System.Collections.Immutable;
using Common;

namespace ConcurrentDictionary.After;

/// <summary>
/// AFTER: ImmutableArray.CreateBuilder(), add all items, ToImmutable(). One allocation
/// for the final array; O(n) time. The builder is mutable and not thread-safe.
/// </summary>
public static class BuilderPattern
{
    public static async Task<ScenarioRun> RunAsync(int concurrencyLevel, TimeSpan timeout)
    {
        var harness = new TestHarness(concurrencyLevel, timeout);
        var builtSize = 0;

        var result = await harness.RunAsync(threadId =>
        {
            var builder = ImmutableArray.CreateBuilder<int>();
            for (var i = 0; i < 200; i++)
                builder.Add(threadId * 1000 + i);
            var arr = builder.ToImmutable(); // single allocation
            Interlocked.Exchange(ref builtSize, arr.Length);
        });

        return new ScenarioRun(result,
            $"Built arrays of size {builtSize}; Builder → ToImmutable() = O(n), single allocation.");
    }
}
