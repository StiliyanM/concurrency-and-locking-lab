using System.Collections.Concurrent;
using Common;

namespace ConcurrentDictionary.Before;

/// <summary>
/// BEFORE: GetOrAdd(key, k => ExpensiveFactory(k)). The factory can be invoked multiple times
/// for the same key (race: two threads miss, both call factory); only one value "wins" but
/// work was duplicated.
/// </summary>
public static class GetOrAddFactoryDuplication
{
    public static async Task<ScenarioRun> RunAsync(int concurrencyLevel, TimeSpan timeout)
    {
        var cache = new ConcurrentDictionary<int, int>();
        var factoryCallCount = 0;
        var harness = new TestHarness(concurrencyLevel, timeout);

        var result = await harness.RunAsync(threadId =>
        {
            var key = threadId % 3; // same keys for many threads
            var value = cache.GetOrAdd(key, _ =>
            {
                Interlocked.Increment(ref factoryCallCount);
                TimingHelpers.SmallDelay();
                return key * 100;
            });
        });

        return new ScenarioRun(result,
            $"Factory called {factoryCallCount} times for 3 keys (duplication possible).");
    }
}
