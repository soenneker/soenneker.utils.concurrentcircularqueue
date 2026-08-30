[![](https://img.shields.io/nuget/v/soenneker.utils.concurrentcircularqueue.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.utils.concurrentcircularqueue/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.utils.concurrentcircularqueue/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.utils.concurrentcircularqueue/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.utils.concurrentcircularqueue.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.utils.concurrentcircularqueue/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.utils.concurrentcircularqueue/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.utils.concurrentcircularqueue/actions/workflows/codeql.yml)

# Soenneker.Utils.ConcurrentCircularQueue

A thread-safe, fixed-capacity FIFO queue that discards the oldest item when a new item exceeds its capacity.

## Installation

```bash
dotnet add package Soenneker.Utils.ConcurrentCircularQueue
```

## Usage

```csharp
var recentIds = new ConcurrentCircularQueue<int>(maxSize: 3);

await recentIds.Enqueue(10);
await recentIds.Enqueue(20);
await recentIds.Enqueue(30);
await recentIds.Enqueue(40); // 10 is discarded

(bool success, int? oldest) = await recentIds.TryDequeue(); // 20
int count = await recentIds.Count();                         // 2
bool contains = await recentIds.Contains(40);                // true
```

`TryDequeue()` returns immediately with `success == false` when the queue is empty. This is a collection, not a producer/consumer channel: it has no operation that waits for a future item.

## Concurrency modes

The default mode uses `ConcurrentQueue<T>` and atomic counting. Operations are thread-safe, but `Contains()` and `Count()` are observations made while other callers may be changing the queue.

```csharp
var queue = new ConcurrentCircularQueue<Job>(100, locking: true);
```

With `locking: true`, enqueue, dequeue, count, and contains operations are serialized through an asynchronous lock. This provides a consistent point-in-time observation relative to the other queue operations, at the cost of contention and asynchronous lock overhead.

Neither mode makes a multi-call sequence atomic. For example, another caller can change the queue between `Contains()` and `TryDequeue()`.
