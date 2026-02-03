using System.Collections.Concurrent;
using Common;

namespace ConcurrentDictionary.After;

/// <summary>
/// AFTER: AddOrUpdate(key, addValue, (k, old) => old + 1) for atomic increment.
/// One atomic operation; no lost updates.
/// </summary>
public static class AddOrUpdateForCounters
{
    public static async Task<ScenarioRun> RunAsync(int concurrencyLevel, TimeSpan timeout)
    {
        var counters = new ConcurrentDictionary<int, int>();
        var harness = new TestHarness(concurrencyLevel, timeout);

        var result = await harness.RunAsync(threadId =>
        {
            var key = threadId % 4;
            counters.AddOrUpdate(key, 1, (_, old) => old + 1);
        });

        var total = counters.Values.Sum();
        return new ScenarioRun(result,
            $"Sum of counters: {total} (expected {concurrencyLevel}); AddOrUpdate is atomic.");
    }
}
