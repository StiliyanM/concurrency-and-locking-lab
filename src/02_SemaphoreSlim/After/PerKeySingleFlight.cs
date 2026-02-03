using System.Collections.Concurrent;
using Common;

namespace SemaphoreSlim.After;

/// <summary>
/// AFTER: Single-flight per key. First caller runs the work; others await the same task.
/// </summary>
public static class PerKeySingleFlight
{
    private static readonly ConcurrentDictionary<string, Lazy<Task>> _flight = new();

    public static async Task<ScenarioRun> RunAsync(int concurrencyLevel, TimeSpan timeout)
    {
        _flight.Clear();
        var workCallCount = 0;
        var harness = new TestHarness(concurrencyLevel, timeout);
        const string key = "A";

        var result = await harness.RunAsync(async _ =>
        {
            var task = _flight.GetOrAdd(key, _ => new Lazy<Task>(() => DoWorkAsync()));
            await task.Value;
        });

        async Task DoWorkAsync()
        {
            Interlocked.Increment(ref workCallCount);
            await Task.Yield();
        }

        return new ScenarioRun(result,
            $"Work executed {workCallCount} time(s) for key \"{key}\" (single-flight).");
    }
}
