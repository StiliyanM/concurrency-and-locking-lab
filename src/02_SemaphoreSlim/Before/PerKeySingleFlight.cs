using Common;

namespace SemaphoreSlim.Before;

/// <summary>
/// BEFORE: Every caller does the work for the same key. No single-flight; N callers = N executions.
/// </summary>
public static class PerKeySingleFlight
{
    private static readonly object LockObj = new();
    private static int _workCallCount;

    public static async Task<ScenarioRun> RunAsync(int concurrencyLevel, TimeSpan timeout)
    {
        _workCallCount = 0;
        var harness = new TestHarness(concurrencyLevel, timeout);
        const string key = "A";

        var result = await harness.RunAsync(_ =>
        {
            lock (LockObj)
            {
                _workCallCount++;
            }
            // Simulate expensive work per call (no sharing)
            TimingHelpers.SmallDelay();
        });

        return new ScenarioRun(result,
            $"Work executed {_workCallCount} times for key \"{key}\" (no single-flight).");
    }
}
