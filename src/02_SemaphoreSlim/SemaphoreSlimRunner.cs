using Common;
using Before = SemaphoreSlim.Before;
using After = SemaphoreSlim.After;

namespace SemaphoreSlim;

public static class SemaphoreSlimRunner
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);

    public static async Task RunScenarioAsync(string choice, int concurrencyLevel, TimeSpan? timeout = null)
    {
        var t = timeout ?? DefaultTimeout;

        (string? name, Func<Task<ScenarioRun>>? before, Func<Task<ScenarioRun>>? after) scenario = choice switch
        {
            "1" => ("Global Throttle",
                () => Before.GlobalThrottle.RunAsync(concurrencyLevel, t),
                () => After.GlobalThrottle.RunAsync(concurrencyLevel, t)),
            "2" => ("Per-Key Single-Flight",
                () => Before.PerKeySingleFlight.RunAsync(concurrencyLevel, t),
                () => After.PerKeySingleFlight.RunAsync(concurrencyLevel, t)),
            "3" => ("Lock Twice Trap",
                () => Before.LockTwiceTrap.RunAsync(concurrencyLevel, t),
                () => After.LockTwiceTrap.RunAsync(concurrencyLevel, t)),
            "4" => ("Cancellation and Timeouts",
                () => Before.CancellationAndTimeouts.RunAsync(concurrencyLevel, t),
                () => After.CancellationAndTimeouts.RunAsync(concurrencyLevel, t)),
            _ => (null, null, null)
        };

        if (scenario.name is null || scenario.before is null || scenario.after is null)
        {
            Console.WriteLine("Unknown scenario.");
            return;
        }

        Console.WriteLine($"\nRunning {scenario.name} (concurrency: {concurrencyLevel}, timeout: {t.TotalSeconds}s)...\n");
        var runBefore = await scenario.before();
        var runAfter = await scenario.after();
        ResultSummary.PrintResults(scenario.name, runBefore, runAfter);
    }
}
