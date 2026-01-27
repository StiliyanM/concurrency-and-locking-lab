namespace Common;

/// <summary>
/// Formats and displays test results in a structured way.
/// </summary>
public static class ResultSummary
{
    public static void PrintResults(string scenarioName, ScenarioRun before, ScenarioRun? after = null)
    {
        Console.WriteLine(new string('=', 80));
        Console.WriteLine($"Scenario: {scenarioName}");
        Console.WriteLine(new string('=', 80));

        Console.WriteLine("\n[BEFORE]");
        PrintResult(before.Result, before.Observation);

        if (after is { } a)
        {
            Console.WriteLine("\n[AFTER]");
            PrintResult(a.Result, a.Observation);
        }

        Console.WriteLine(new string('=', 80));
        Console.WriteLine();
    }

    private static void PrintResult(TestResult result, string? observation = null)
    {
        var status = result.IsSuccess ? "✓ SUCCESS" : "✗ FAILED";
        Console.WriteLine($"Status: {status}");
        Console.WriteLine($"Completed: {result.CompletedCount}/{result.ExpectedCount}");

        if (observation is { Length: > 0 })
            Console.WriteLine($"Observation: {observation}");

        if (result is { TimedOut: true })
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("⚠ TIMEOUT DETECTED - Possible deadlock or thread starvation!");
            Console.ResetColor();
        }

        if (result is { Exceptions.Count: > 0 })
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Exceptions: {result.Exceptions.Count}");
            foreach (var ex in result.Exceptions.Take(3))
                Console.WriteLine($"  - {ex.GetType().Name}: {ex.Message}");
            if (result.Exceptions.Count > 3)
                Console.WriteLine($"  ... and {result.Exceptions.Count - 3} more");
            Console.ResetColor();
        }

        Console.WriteLine($"Elapsed: {result.ElapsedMilliseconds}ms");
    }
}
