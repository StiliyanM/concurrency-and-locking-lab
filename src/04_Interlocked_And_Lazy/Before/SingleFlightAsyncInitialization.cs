using Common;

namespace InterlockedAndLazy.Before;

/// <summary>
/// BEFORE: Multiple callers all await their own async init. No single-flight; N callers = N executions.
/// </summary>
public static class SingleFlightAsyncInitialization
{
    private static int _initCallCount;
    private static Task<int>? _initTask;

    public static async Task<ScenarioRun> RunAsync(int concurrencyLevel, TimeSpan timeout)
    {
        _initCallCount = 0;
        _initTask = null;
        var harness = new TestHarness(concurrencyLevel, timeout);

        var result = await harness.RunAsync(async _ =>
        {
            if (_initTask is null)
            {
                Interlocked.Increment(ref _initCallCount);
                _initTask = DoInitAsync();
            }
            await _initTask;
        });

        return new ScenarioRun(result,
            $"Init called {_initCallCount} time(s) (no single-flight; race on _initTask).");
    }

    private static async Task<int> DoInitAsync()
    {
        await Task.Yield();
        return 42;
    }
}
