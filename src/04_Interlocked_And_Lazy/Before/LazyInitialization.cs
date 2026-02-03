using Common;

namespace InterlockedAndLazy.Before;

/// <summary>
/// BEFORE: Manual lazy init without proper synchronization. Multiple threads can all run
/// the factory; we want exactly one initialization.
/// </summary>
public static class LazyInitialization
{
    private static ExpensiveResource? _instance;
    private static int _factoryCallCount;

    public static async Task<ScenarioRun> RunAsync(int concurrencyLevel, TimeSpan timeout)
    {
        _instance = null;
        _factoryCallCount = 0;
        var harness = new TestHarness(concurrencyLevel, timeout);

        var result = await harness.RunAsync(_ =>
        {
            if (_instance is null)
            {
                Interlocked.Increment(ref _factoryCallCount);
                _instance = new ExpensiveResource();
            }
            _ = _instance.Value;
        });

        return new ScenarioRun(result,
            $"Factory called {_factoryCallCount} time(s) (expected 1; no synchronization = multiple inits).");
    }

    private sealed class ExpensiveResource
    {
        public int Value { get; } = 42;
    }
}
