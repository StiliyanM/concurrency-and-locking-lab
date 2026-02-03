using System.Collections.Immutable;
using Common;

namespace ConcurrentDictionary.After;

/// <summary>
/// AFTER: ImmutableArray. Capture the current reference = stable snapshot. Readers iterate
/// without blocking; writers create new instances. No lock needed for readers.
/// </summary>
public static class SnapshotIsolation
{
    private static ImmutableArray<int> _arr = ImmutableArray<int>.Empty;
    private static readonly object PublishLock = new();

    public static async Task<ScenarioRun> RunAsync(int concurrencyLevel, TimeSpan timeout)
    {
        var builder = ImmutableArray.CreateBuilder<int>();
        for (var i = 0; i < 10; i++)
            builder.Add(i);
        _arr = builder.ToImmutable();

        var harness = new TestHarness(concurrencyLevel, timeout);
        var snapshotCount = 0;

        var result = await harness.RunAsync(threadId =>
        {
            if (threadId == 0)
            {
                var snapshot = _arr; // stable snapshot; never mutated
                var count = 0;
                foreach (var _ in snapshot)
                {
                    count++;
                    TimingHelpers.SmallDelay();
                }
                snapshotCount = count;
            }
            else
            {
                lock (PublishLock)
                {
                    _arr = _arr.Add(threadId % 10);
                }
            }
        });

        return new ScenarioRun(result,
            $"Snapshot had {snapshotCount} entries; ImmutableArray = stable snapshot, readers never block writers.");
    }
}
