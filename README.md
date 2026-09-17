# Concurrency & Locking Lab

A hands-on, example-driven codebase that teaches how to choose and use concurrency primitives correctly in .NET, with emphasis on:

- Correctness under contention
- Avoiding deadlocks and thread starvation
- Understanding async pitfalls
- Practical patterns used in high-throughput services

This is a set of small, realistic scenarios that demonstrate what breaks, why it breaks, and what the right tool looks like.

## When to use what

| Need | Use |
|------|-----|
| **Mutual exclusion** (sync only, no await) | `lock` (Monitor) |
| **Mutual exclusion** (async, await inside) | `SemaphoreSlim` + `WaitAsync` |
| **Throttle** (limit concurrent work) | `SemaphoreSlim` |
| **Single-flight** (one execution, many awaiters) | `Lazy<T>` or `Lazy<Task<T>>` |
| **Thread-safe dictionary** (many reads) | `ConcurrentDictionary` |
| **Per-key single-flight** | `ConcurrentDictionary` + `Lazy<Task<T>>` |
| **Atomic counter** (increment, add) | `Interlocked.Increment` / `Add` |
| **Atomic update** (e.g. max) | `Interlocked.CompareExchange` loop |
| **Stable snapshot** (iterate without blocking writers) | `ImmutableArray` / `ImmutableList` |
| **Lazy init** (exactly once) | `Lazy<T>` |

**Avoid:** `lock` + `await` or `Task.Wait()` on work that needs the same lock → deadlock.

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

## Production problems behind the modules

I built these examples around concurrency problems I’ve worked on. They’re small exercises you can run and break yourself.

- **01: lock / Monitor.** The lost-update and lock-ordering examples connect to an in-memory nomenclature cache at EGT.
- **02: SemaphoreSlim.** At PayRetailers, I added per-card concurrency control through MediatR behaviours so 2 status updates for the same card couldn’t race.
- **03: ConcurrentDictionary and immutable collections.** The cache and duplicate-factory examples relate to my Segment API fix: coalescing requests per segment cut external calls from about 30 to 10 and made the measured flow roughly 3.8x faster. In production, I used a per-segment SemaphoreSlim and a double-checked cache.
- **04: Interlocked and Lazy.** I moved cache warm-up out of a service constructor and into a hosted service, then swapped immutable snapshots with Interlocked.Exchange so reads needed no locks. The refactor removed a 5–6 second first-request delay and cut startup from about 30 seconds to under 1.
