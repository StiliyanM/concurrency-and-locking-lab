using Common;

namespace InterlockedAndLazy.After;

/// <summary>
/// AFTER: Lazy&lt;Task&lt;T&gt;&gt;. Single-flight async init; first caller runs the task,
/// others await the same Task.
/// </summary>
public static class SingleFlightAsyncInitialization
{
    private static int _initCallCount;

    public static async Task<ScenarioRun> RunAsync(int concurrencyLevel, TimeSpan timeout)
    {
        _initCallCount = 0;
        var lazyTask = new Lazy<Task<int>>(() =>
        {
            Interlocked.Increment(ref _initCallCount);
            return DoInitAsync();
        });
        var harness = new TestHarness(concurrencyLevel, timeout);

        var result = await harness.RunAsync(async _ =>
        {
            await lazyTask.Value;
        });

        return new ScenarioRun(result,
            $"Init called {_initCallCount} time(s) (Lazy&lt;Task&lt;T&gt;&gt; = single-flight async).");
    }

    private static async Task<int> DoInitAsync()
    {
        await Task.Yield();
        return 42;
    }
}
