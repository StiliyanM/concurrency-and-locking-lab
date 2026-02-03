using System.Collections.Generic;
using Common;

namespace ConcurrentDictionary.Before;

/// <summary>
/// BEFORE: Mutable List. One thread iterates while others modify. Without synchronization
/// you get InvalidOperationException (collection was modified during enumeration).
/// With a lock, iteration blocks all writers.
/// </summary>
public static class SnapshotIsolation
{
    public static async Task<ScenarioRun> RunAsync(int concurrencyLevel, TimeSpan timeout)
    {
        var list = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        var harness = new TestHarness(concurrencyLevel, timeout);
        var snapshotCount = 0;

        var result = await harness.RunAsync(threadId =>
        {
            if (threadId == 0)
            {
                // Iterate while others modify → InvalidOperationException or wrong count
                var count = 0;
                foreach (var _ in list)
                {
                    count++;
                    TimingHelpers.SmallDelay();
                }
                snapshotCount = count;
            }
            else
            {
                list.Add(threadId);
            }
        });

        var observation = result.Exceptions.Count > 0
            ? $"Exceptions: {result.Exceptions.Count} (mutable List modified during enumeration)."
            : $"Enumerator saw {snapshotCount} entries; mutable collection, no stable snapshot.";
        return new ScenarioRun(result, observation);
    }
}
