using Common;
using Before = InterlockedAndLazy.Before;
using After = InterlockedAndLazy.After;

namespace InterlockedAndLazy;

public static class InterlockedAndLazyRunner
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);

    public static async Task RunScenarioAsync(string choice, int concurrencyLevel, TimeSpan? timeout = null)
    {
        var t = timeout ?? DefaultTimeout;

        (string? name, Func<Task<ScenarioRun>>? before, Func<Task<ScenarioRun>>? after) scenario = choice switch
        {
            "1" => ("Atomic Counters",
                () => Before.AtomicCounters.RunAsync(concurrencyLevel, t),
                () => After.AtomicCounters.RunAsync(concurrencyLevel, t)),
            "2" => ("CAS-Based Max Update",
                () => Before.CasBasedMaxUpdate.RunAsync(concurrencyLevel, t),
                () => After.CasBasedMaxUpdate.RunAsync(concurrencyLevel, t)),
            "3" => ("Lazy Initialization",
                () => Before.LazyInitialization.RunAsync(concurrencyLevel, t),
                () => After.LazyInitialization.RunAsync(concurrencyLevel, t)),
            "4" => ("Single-Flight Async Initialization",
                () => Before.SingleFlightAsyncInitialization.RunAsync(concurrencyLevel, t),
                () => After.SingleFlightAsyncInitialization.RunAsync(concurrencyLevel, t)),
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
