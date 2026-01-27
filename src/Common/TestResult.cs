namespace Common;

/// <summary>
/// Results from running a concurrency test scenario.
/// </summary>
public class TestResult
{
    public int CompletedCount { get; set; }
    public int ExpectedCount { get; set; }
    public List<Exception> Exceptions { get; set; } = [];
    public long ElapsedMilliseconds { get; set; }
    public bool TimedOut { get; set; }

    public bool IsSuccess => CompletedCount == ExpectedCount && Exceptions.Count == 0 && !TimedOut;

    public override string ToString()
    {
        var result = $"Completed: {CompletedCount}/{ExpectedCount}";

        result += TimedOut ? " [TIMEOUT - Possible deadlock!]" : "";

        if (Exceptions.Count > 0)
        {
            result += $"\nExceptions: {Exceptions.Count}";
            foreach (var ex in Exceptions.Take(5))
            {
                result += $"\n  - {ex.GetType().Name}: {ex.Message}";
            }
            if (Exceptions.Count > 5)
                result += $"\n  ... and {Exceptions.Count - 5} more";
        }

        result += $"\nElapsed: {ElapsedMilliseconds}ms";
        return result;
    }
}

/// <summary>
/// Result of running a scenario (before or after), with optional observation (e.g. final counter value).
/// </summary>
public record ScenarioRun(TestResult Result, string? Observation = null);
