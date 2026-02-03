using Common;
using ConcurrentDictionary;
using InterlockedAndLazy;
using Lock;
using SemaphoreSlim;

namespace LabRunner;

/// <summary>
/// Interactive menu system for selecting and running lab scenarios.
/// </summary>
public class LabMenu
{
    private const int DefaultConcurrencyLevel = 10;
    private const int DefaultTimeoutSeconds = 10;

    public async Task RunAsync()
    {
        while (true)
        {
            Console.WriteLine("\nSelect a module:");
            Console.WriteLine("1. lock / Monitor");
            Console.WriteLine("2. SemaphoreSlim");
            Console.WriteLine("3. ConcurrentDictionary + Immutable Collections");
            Console.WriteLine("4. Interlocked & Lazy");
            Console.WriteLine("0. Exit");
            Console.Write("\nChoice: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await RunLockAsync();
                    break;
                case "2":
                    await RunSemaphoreSlimAsync();
                    break;
                case "3":
                    await RunConcurrentDictionaryAsync();
                    break;
                case "4":
                    await RunInterlockedAndLazyAsync();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    private async Task RunLockAsync()
    {
        Console.WriteLine("\nlock / Monitor");
        Console.WriteLine("Select scenario:");
        Console.WriteLine("1. Lost Updates");
        Console.WriteLine("2. Lock Ordering Deadlock");
        Console.WriteLine("3. Reentrancy");
        Console.WriteLine("4. Async Trap");
        Console.WriteLine("0. Back");
        Console.Write("\nChoice: ");

        var choice = Console.ReadLine()?.Trim();
        if (choice is "0" or null)
            return;

        var concurrencyLevel = GetConcurrencyLevel();
        await LockRunner.RunScenarioAsync(choice, concurrencyLevel);
    }

    private async Task RunSemaphoreSlimAsync()
    {
        Console.WriteLine("\nSemaphoreSlim");
        Console.WriteLine("Select scenario:");
        Console.WriteLine("1. Global Throttle");
        Console.WriteLine("2. Per-Key Single-Flight");
        Console.WriteLine("3. Lock Twice Trap");
        Console.WriteLine("4. Cancellation and Timeouts");
        Console.WriteLine("0. Back");
        Console.Write("\nChoice: ");

        var choice = Console.ReadLine()?.Trim();
        if (choice is "0" or null)
            return;

        var concurrencyLevel = GetConcurrencyLevel();
        await SemaphoreSlimRunner.RunScenarioAsync(choice, concurrencyLevel);
    }

    private async Task RunConcurrentDictionaryAsync()
    {
        Console.WriteLine("\nConcurrentDictionary + Immutable Collections");
        Console.WriteLine("Select scenario:");
        Console.WriteLine("1. Thread-Safe Cache Lookup");
        Console.WriteLine("2. GetOrAdd Factory Duplication");
        Console.WriteLine("3. AddOrUpdate for Counters");
        Console.WriteLine("4. Immutable Collections - Snapshot Isolation");
        Console.WriteLine("5. Immutable Collections - Builder Pattern");
        Console.WriteLine("0. Back");
        Console.Write("\nChoice: ");

        var choice = Console.ReadLine()?.Trim();
        if (choice is "0" or null)
            return;

        var concurrencyLevel = GetConcurrencyLevel();
        await ConcurrentDictionaryRunner.RunScenarioAsync(choice, concurrencyLevel);
    }

    private async Task RunInterlockedAndLazyAsync()
    {
        Console.WriteLine("\nInterlocked & Lazy");
        Console.WriteLine("Select scenario:");
        Console.WriteLine("1. Atomic Counters");
        Console.WriteLine("2. CAS-Based Max Update");
        Console.WriteLine("3. Lazy Initialization");
        Console.WriteLine("4. Single-Flight Async Initialization");
        Console.WriteLine("0. Back");
        Console.Write("\nChoice: ");

        var choice = Console.ReadLine()?.Trim();
        if (choice is "0" or null)
            return;

        var concurrencyLevel = GetConcurrencyLevel();
        await InterlockedAndLazyRunner.RunScenarioAsync(choice, concurrencyLevel);
    }

    private int GetConcurrencyLevel()
    {
        Console.Write($"\nConcurrency level (default: {DefaultConcurrencyLevel}): ");
        var input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
        {
            return DefaultConcurrencyLevel;
        }

        if (int.TryParse(input, out var level) && level > 0)
        {
            return level;
        }

        Console.WriteLine($"Invalid input, using default: {DefaultConcurrencyLevel}");
        return DefaultConcurrencyLevel;
    }
}
