# Module 04 — Interlocked & Lazy

## What we cover

- Atomic counters with Interlocked.Increment
- CAS (Compare-And-Swap) for non-trivial updates (e.g. max)
- Lazy&lt;T&gt; for thread-safe lazy initialization
- Lazy&lt;Task&lt;T&gt;&gt; for single-flight async initialization

## Scenarios

| # | Scenario | Before | After |
|---|----------|--------|-------|
| 1 | Atomic Counters | Read-modify-write (lost updates) | Interlocked.Increment |
| 2 | CAS-Based Max Update | Read max, compare, write (race, lose max) | Interlocked.CompareExchange loop |
| 3 | Lazy Initialization | Manual if-null init (multiple threads run factory) | Lazy&lt;T&gt; = single execution |
| 4 | Single-Flight Async Init | Multiple callers each start async init | Lazy&lt;Task&lt;T&gt;&gt; = one task, all await |

## Rule of thumb

**Use Interlocked for simple atomic operations (increment, add, exchange).**  
**Use Lazy&lt;T&gt; when you need exactly-once initialization.**  
**Use Lazy&lt;Task&lt;T&gt;&gt; for single-flight async init.**

## Common traps

- Read-modify-write (read, compute, write) is not atomic; use Interlocked or lock.
- Manual double-checked locking is easy to get wrong; prefer Lazy&lt;T&gt;.
- For async init, Lazy&lt;Task&lt;T&gt;&gt; ensures one task runs; others await the same instance.

## Use this when…

- You need a counter or simple atomic update.
- You need lazy init with thread-safe, exactly-once execution.
- You need async lazy init (e.g. load config from network once).

## Avoid when…

- The operation is complex (multiple steps); use lock instead.
- You need per-key lazy init; use ConcurrentDictionary + Lazy per key.
