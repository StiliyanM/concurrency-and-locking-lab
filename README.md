# Concurrency & Locking Lab

A hands-on, example-driven codebase that teaches how to choose and use concurrency primitives correctly in .NET, with emphasis on:

- Correctness under contention
- Avoiding deadlocks and thread starvation
- Understanding async pitfalls
- Practical patterns used in high-throughput services

This is not a "theory repo" and not a "cookbook". It's a set of small, realistic scenarios that demonstrate what breaks, why it breaks, and what the right tool looks like.

## Project Structure

```
/src
  /LabRunner                // Console app entry point
  /Common                   // Timing helpers, logging, test harness utilities
  /01_Lock                  // lock / Monitor scenarios
  /02_SemaphoreSlim         // SemaphoreSlim scenarios
  /03_ConcurrentDictionary   // ConcurrentDictionary + Immutable Collections scenarios
  /04_Interlocked_And_Lazy   // Interlocked & Lazy scenarios
```

## Running the Lab

1. Build the solution:
   ```bash
   dotnet build
   ```

2. Run the lab runner:
   ```bash
   dotnet run --project src/LabRunner/LabRunner.csproj
   ```

3. Follow the interactive menu to select modules and scenarios.

## Modules

### Module 01 - lock / Monitor
- Lost Updates
- Lock Ordering Deadlock
- Reentrancy
- Async Trap

### Module 02 - SemaphoreSlim
- Global Throttle
- Per-Key Single-Flight
- Lock Twice Trap
- Cancellation and Timeouts

### Module 03 - ConcurrentDictionary + Immutable Collections
- Thread-Safe Cache Lookup
- GetOrAdd Factory Duplication
- AddOrUpdate for Counters
- Immutable Collections - Snapshot Isolation
- Immutable Collections - Builder Pattern

### Module 04 - Interlocked & Lazy
- Atomic Counters
- CAS-Based Max Update
- Lazy Initialization
- Single-Flight Async Initialization

## Design Principles

1. **Show failures that are reproducible** - Using barriers, artificial delays, timeouts, and logging
2. **Teach decision-making, not syntax** - Every lab includes "Use this when...", "Avoid when...", "Common trap: ..."
3. **Keep examples realistic** - Scenarios like per-key cache loading, rate-limited calls, event processing

## Requirements

- .NET 10 SDK
