using System.Collections.Concurrent;
using Common;

namespace ConcurrentDictionary.After;

/// <summary>
/// AFTER: GetOrAdd(key, k => new Lazy&lt;T&gt;(() => ExpensiveFactory(k))).Value.
/// The Lazy ensures the factory runs at most once per key; duplicate GetOrAdd calls
/// get the same Lazy and only the first .Value triggers the factory.
/// </summary>
public static class GetOrAddFactoryDuplication
{
    public static async Task<ScenarioRun> RunAsync(int concurrencyLevel, TimeSpan timeout)
    {
        var cache = new ConcurrentDictionary<int, Lazy<int>>();
        var factoryCallCount = 0;
        var harness = new TestHarness(concurrencyLevel, timeout);

        var result = await harness.RunAsync(threadId =>
        {
            var key = threadId % 3;
            var value = cache.GetOrAdd(key, k => new Lazy<int>(() =>
            {
                Interlocked.Increment(ref factoryCallCount);
                TimingHelpers.SmallDelay();
                return k * 100;
            })).Value;
        });

        return new ScenarioRun(result,
            $"Factory called {factoryCallCount} time(s) for 3 keys (Lazy = single execution per key).");
    }
}
