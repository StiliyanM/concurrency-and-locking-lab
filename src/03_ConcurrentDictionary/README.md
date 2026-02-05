# Module 03 — ConcurrentDictionary + Immutable Collections

## What we cover

- Thread-safe cache lookup (lock-free reads with ConcurrentDictionary)
- GetOrAdd factory running multiple times (Lazy to ensure single execution per key)
- Atomic counters with AddOrUpdate
- ImmutableArray for snapshot isolation (capture reference = stable snapshot)
- Builder pattern for constructing immutable collections (ImmutableArray.CreateBuilder)

## Scenarios

| # | Scenario | Before | After |
|---|----------|--------|-------|
| 1 | Thread-Safe Cache Lookup | Dictionary without lock → races, exceptions, wrong values | ConcurrentDictionary = thread-safe |
| 2 | GetOrAdd Factory Duplication | GetOrAdd(key, factory) — factory can run multiple times for same key | GetOrAdd(key, k => new Lazy\<T\>(() => factory(k))).Value |
| 3 | AddOrUpdate for Counters | TryGetValue + assign (read-modify-write race, lost updates) | AddOrUpdate(key, 1, (_, old) => old + 1) atomic |
| 4 | Snapshot Isolation | Mutable List: iteration throws or blocks writers | ImmutableArray; capture reference = stable snapshot |
| 5 | Builder Pattern | ImmutableArray by repeated Add → O(n²), many allocations | Builder → ToImmutable() = O(n), single allocation |

## Rule of thumb

**Use ConcurrentDictionary for shared key/value state with high read concurrency.**  
**Use GetOrAdd with Lazy\<T\> when the factory is expensive and must run once per key.**  
**Use ImmutableArray/ImmutableList when readers need a stable snapshot without locking.**

## Common traps

- GetOrAdd(key, valueFactory): the factory can be invoked more than once for the same key under contention; wrap in Lazy if you need exactly one execution.
- Read-modify-write on ConcurrentDictionary (TryGetValue then AddOrUpdate) is not atomic; use AddOrUpdate with an update delegate.
- Immutable collections (ImmutableArray, ImmutableList, etc.): updates return a new instance; assign back to the shared reference (with proper synchronization for writers).

## Use this when…

- You need a thread-safe dictionary with many readers and fewer writers.
- You need per-key single-flight (one computation per key, others await).
- You need a point-in-time snapshot for iteration without locking writers.

## Avoid when…

- You need a simple lock for low contention; lock + Dictionary may be enough.
- Every update must be visible to all readers immediately; immutable collections copy-on-write, so use ConcurrentDictionary for that.
