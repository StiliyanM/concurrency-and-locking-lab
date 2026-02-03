using Common;

namespace SemaphoreSlim.Before;

/// <summary>
/// BEFORE: Wait() with no cancellation. When semaphore has no capacity, workers block
/// forever and never see the timeout/cancellation from the harness.
/// </summary>
public static class CancellationAndTimeouts
{
    public static async Task<ScenarioRun> RunAsync(int concurrencyLevel, TimeSpan timeout)
    {
        // No capacity: nobody can enter; all will block on Wait()
        var sem = new System.Threading.SemaphoreSlim(0, concurrencyLevel);
        var harness = new TestHarness(concurrencyLevel, timeout);

        var result = await harness.RunAsync(_ =>
        {
            sem.Wait(); // No token → blocks forever; harness timeout won't cancel this
        });

        var observation = result.TimedOut
            ? "Workers blocked on Wait() with no token; harness timed out (no cancellation)."
            : "Completed (unexpected).";
        return new ScenarioRun(result, observation);
    }
}
