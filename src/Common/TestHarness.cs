using System.Collections.Concurrent;
using System.Diagnostics;

namespace Common;

/// <summary>
/// Test harness for running concurrency scenarios with synchronization, timeout detection, and result aggregation.
/// </summary>
public class TestHarness(int concurrencyLevel, TimeSpan timeout)
{
    private readonly Action<int>? _workAction;
    private readonly Func<int, Task>? _asyncWorkAction;

    public TestHarness(int concurrencyLevel, TimeSpan timeout, Action<int> workAction)
        : this(concurrencyLevel, timeout)
    {
        _workAction = workAction;
    }

    public TestHarness(int concurrencyLevel, TimeSpan timeout, Func<int, Task> asyncWorkAction)
        : this(concurrencyLevel, timeout)
    {
        _asyncWorkAction = asyncWorkAction;
    }

    /// <summary>
    /// Runs a synchronous scenario with barrier synchronization.
    /// </summary>
    public async Task<TestResult> RunAsync(Action<int> workAction)
    {
        var barrier = new Barrier(concurrencyLevel + 1); // +1 for main thread
        var cts = new CancellationTokenSource(timeout);
        var exceptions = new ConcurrentBag<Exception>();
        var completedCount = 0;
        var sw = Stopwatch.StartNew();

        var tasks = Enumerable.Range(0, concurrencyLevel)
            .Select(threadId => Task.Run(() =>
            {
                try
                {
                    // Wait for all threads to be ready
                    barrier.SignalAndWait(cts.Token);

                    // Execute the work
                    workAction(threadId);

                    Interlocked.Increment(ref completedCount);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }))
            .ToArray();

        try
        {
            // Signal that main thread is ready
            barrier.SignalAndWait(cts.Token);

            // Wait for all tasks to complete or timeout
            await Task.WhenAll(tasks).WaitAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            // Timeout occurred - potential deadlock
        }

        sw.Stop();

        return new TestResult
        {
            CompletedCount = completedCount,
            ExpectedCount = concurrencyLevel,
            Exceptions = [..exceptions],
            ElapsedMilliseconds = sw.ElapsedMilliseconds,
            TimedOut = completedCount < concurrencyLevel
        };
    }

    /// <summary>
    /// Runs an asynchronous scenario with barrier synchronization.
    /// </summary>
    public async Task<TestResult> RunAsync(Func<int, Task> asyncWorkAction)
    {
        var barrier = new Barrier(concurrencyLevel + 1); // +1 for main thread
        var cts = new CancellationTokenSource(timeout);
        var exceptions = new ConcurrentBag<Exception>();
        var completedCount = 0;
        var sw = Stopwatch.StartNew();

        var tasks = Enumerable.Range(0, concurrencyLevel)
            .Select(async threadId =>
            {
                try
                {
                    // Wait for all threads to be ready
                    barrier.SignalAndWait(cts.Token);

                    // Execute the async work
                    await asyncWorkAction(threadId);

                    Interlocked.Increment(ref completedCount);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            })
            .ToArray();

        try
        {
            // Signal that main thread is ready
            barrier.SignalAndWait(cts.Token);

            // Wait for all tasks to complete or timeout
            await Task.WhenAll(tasks).WaitAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            // Timeout occurred - potential deadlock
        }

        sw.Stop();

        return new TestResult
        {
            CompletedCount = completedCount,
            ExpectedCount = concurrencyLevel,
            Exceptions = [..exceptions],
            ElapsedMilliseconds = sw.ElapsedMilliseconds,
            TimedOut = completedCount < concurrencyLevel
        };
    }
}
