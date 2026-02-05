# Module 01 — lock / Monitor

## What we cover

- Mutual exclusion for synchronous code
- Reentrancy behavior
- Deadlock patterns (lock ordering)
- Why you must not await inside lock
- Why `.Result` / `.Wait()` can deadlock

## Scenarios

| # | Scenario | Before | After |
|---|----------|--------|-------|
| 1 | Lost Updates | Non-thread-safe counter (race on read-modify-write) | `lock` around critical section |
| 2 | Lock Ordering Deadlock | Two locks acquired in different orders by different flows | Consistent ordering (always A then B) |
| 3 | Reentrancy | Non-reentrant gate (e.g. SemaphoreSlim): same logical flow waits twice → deadlock | Monitor is reentrant; same thread can re-enter |
| 4 | Async Trap | `lock` + blocking wait (e.g. `Task.Wait()`) on work that needs same lock → deadlock | SemaphoreSlim + `WaitAsync` + Release in `finally` |

## Rule of thumb

**Use `lock` for short, synchronous critical sections.**  
**If you need to wait asynchronously, don’t use `lock`.**

**Lock vs Interlocked:** Both Lost Updates and Atomic Counters fix a non-atomic increment. Use `lock` when the critical section does more than a single primitive op (e.g. read-modify-write across multiple variables). Use `Interlocked` when it's a single atomic op (increment, add, exchange) — no lock, better scalability.

## Common traps

- Re-entering the same lock from the same thread is allowed (Monitor is reentrant); other primitives (e.g. SemaphoreSlim) are not.
- Holding a lock and then calling code that blocks waiting on the same lock (e.g. `Task.Wait()` on a task that needs that lock) → deadlock.
- Acquiring two locks in different orders in different code paths → risk of circular wait and deadlock.

## Use this when…

- You need mutual exclusion in fully synchronous code.
- The critical section is short and does no I/O or async work.

## Avoid when…

- You need to `await` inside the critical section.
- You are coordinating across async boundaries; use `SemaphoreSlim` or other async-safe mechanisms instead.
