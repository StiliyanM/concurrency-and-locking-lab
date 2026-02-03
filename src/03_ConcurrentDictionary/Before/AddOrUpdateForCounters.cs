using System.Collections.Concurrent;
using Common;

namespace ConcurrentDictionary.Before;

/// <summary>
/// BEFORE: TryGetValue then AddOrUpdate in a loop (or separate Add/Update) can lose updates
/// if not done atomically. Here we do read-modify-write with GetOrAdd + separate update,
/// which is not atomic for "increment".
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
            counters.TryGetValue(key, out var current);
            TimingHelpers.SmallDelay(); // race window
            counters[key] = current + 1;
        });

        var total = counters.Values.Sum();
        return new ScenarioRun(result,
            $"Sum of counters: {total} (expected {concurrencyLevel}); lost updates possible.");
    }
}
