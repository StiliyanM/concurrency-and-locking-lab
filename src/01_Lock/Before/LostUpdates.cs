using Common;

namespace Lock.Before;

/// <summary>
/// BEFORE: Non-thread-safe counter. Read-modify-write without lock causes lost updates.
/// </summary>
public static class LostUpdates
{
    public static async Task<ScenarioRun> RunAsync(int concurrencyLevel, TimeSpan timeout)
    {
        var counter = 0;
        var harness = new TestHarness(concurrencyLevel, timeout);

        var result = await harness.RunAsync(_ =>
        {
            // Race: read-modify-write is not atomic
            var temp = counter;
            TimingHelpers.SmallDelay();
            counter = temp + 1;
        });

        return new ScenarioRun(result, $"Counter: {counter} (expected {concurrencyLevel})");
    }
}
