using Common;

namespace InterlockedAndLazy.After;

/// <summary>
/// AFTER: Lazy&lt;T&gt;. Thread-safe by default; factory runs exactly once.
/// </summary>
public static class LazyInitialization
{
    private static int _factoryCallCount;

    public static async Task<ScenarioRun> RunAsync(int concurrencyLevel, TimeSpan timeout)
    {
        _factoryCallCount = 0;
        var lazy = new Lazy<ExpensiveResource>(() =>
        {
            Interlocked.Increment(ref _factoryCallCount);
            return new ExpensiveResource();
        });
        var harness = new TestHarness(concurrencyLevel, timeout);

        var result = await harness.RunAsync(threadId =>
        {
            _ = lazy.Value;
        });

        return new ScenarioRun(result,
            $"Factory called {_factoryCallCount} time(s) (Lazy&lt;T&gt; = single execution).");
    }

    private sealed class ExpensiveResource
    {
        public int Value { get; } = 42;
    }
}
