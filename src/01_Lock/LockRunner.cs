using Common;
using Before = Lock.Before;
using After = Lock.After;

namespace Lock;

public static class LockRunner
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);

    public static async Task RunScenarioAsync(string choice, int concurrencyLevel, TimeSpan? timeout = null)
    {
        var t = timeout ?? DefaultTimeout;

        (string? name, Func<Task<ScenarioRun>>? before, Func<Task<ScenarioRun>>? after) scenario = choice switch
        {
            "1" => ("Lost Updates",
                () => Before.LostUpdates.RunAsync(concurrencyLevel, t),
                () => After.LostUpdates.RunAsync(concurrencyLevel, t)),
            "2" => ("Lock Ordering Deadlock",
                () => Before.LockOrderingDeadlock.RunAsync(concurrencyLevel, t),
                () => After.LockOrderingDeadlock.RunAsync(concurrencyLevel, t)),
            "3" => ("Reentrancy",
                () => Before.Reentrancy.RunAsync(concurrencyLevel, t),
                () => After.Reentrancy.RunAsync(concurrencyLevel, t)),
            "4" => ("Async Trap",
                () => Before.AsyncTrap.RunAsync(concurrencyLevel, t),
                () => After.AsyncTrap.RunAsync(concurrencyLevel, t)),
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
