# Module 02 — SemaphoreSlim

## What we cover

- Throttling concurrent work (global limit)
- Single-flight per key (one execution, many awaiters)
- Non-reentrancy (lock-twice deadlock)
- Cancellation and timeouts with `WaitAsync(CancellationToken)`

## Scenarios

| # | Scenario | Before | After |
|---|----------|--------|-------|
| 1 | Global Throttle | No limit; all workers run at once | SemaphoreSlim limits max concurrent (e.g. 3) |
| 2 | Per-Key Single-Flight | Every caller does the work (N callers = N executions) | One task per key; others await same task (Lazy\<Task\>) |
| 3 | Lock Twice Trap | Same thread acquires SemaphoreSlim twice → deadlock (non-reentrant) | Single Wait/Release scope; no nested acquire |
| 4 | Cancellation and Timeouts | `Wait()` with no token → blocks forever when harness times out | `WaitAsync(CancellationToken)` → workers exit on cancel |

## Rule of thumb

**Use SemaphoreSlim when you need async-friendly throttling or signaling.**  
**Always use `WaitAsync` (and a token) in async code; release in `finally`.**

## Common traps

- SemaphoreSlim is not reentrant: the same thread cannot acquire it twice without releasing first.
- `Wait()` with no cancellation token will not respond to timeout/cancellation; use `WaitAsync(cts.Token)`.
- For per-key single-flight, use a shared structure (e.g. `ConcurrentDictionary<string, Lazy<Task>>`) so only one execution runs per key.

## Use this when…

- You need to limit concurrent access to a resource (throttle).
- You need to wait asynchronously (use `WaitAsync`).
- You want to coordinate with cancellation/timeout.

## Avoid when…

- You need reentrancy (same thread entering twice); use `lock` (Monitor) for that.
- You only need mutual exclusion with no async; `lock` is simpler.
