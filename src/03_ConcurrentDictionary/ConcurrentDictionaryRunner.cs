using Common;
using Before = ConcurrentDictionary.Before;
using After = ConcurrentDictionary.After;

namespace ConcurrentDictionary;

public static class ConcurrentDictionaryRunner
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);

    public static async Task RunScenarioAsync(string choice, int concurrencyLevel, TimeSpan? timeout = null)
    {
        var t = timeout ?? DefaultTimeout;

        (string? name, Func<Task<ScenarioRun>>? before, Func<Task<ScenarioRun>>? after) scenario = choice switch
        {
            "1" => ("Thread-Safe Cache Lookup",
                () => Before.ThreadSafeCacheLookup.RunAsync(concurrencyLevel, t),
                () => After.ThreadSafeCacheLookup.RunAsync(concurrencyLevel, t)),
            "2" => ("GetOrAdd Factory Duplication",
                () => Before.GetOrAddFactoryDuplication.RunAsync(concurrencyLevel, t),
                () => After.GetOrAddFactoryDuplication.RunAsync(concurrencyLevel, t)),
            "3" => ("AddOrUpdate for Counters",
                () => Before.AddOrUpdateForCounters.RunAsync(concurrencyLevel, t),
                () => After.AddOrUpdateForCounters.RunAsync(concurrencyLevel, t)),
            "4" => ("Immutable Collections - Snapshot Isolation",
                () => Before.SnapshotIsolation.RunAsync(concurrencyLevel, t),
                () => After.SnapshotIsolation.RunAsync(concurrencyLevel, t)),
            "5" => ("Immutable Collections - Builder Pattern",
                () => Before.BuilderPattern.RunAsync(concurrencyLevel, t),
                () => After.BuilderPattern.RunAsync(concurrencyLevel, t)),
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
