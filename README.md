[![](https://img.shields.io/nuget/v/soenneker.utils.concurrentcircularqueue.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.utils.concurrentcircularqueue/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.utils.concurrentcircularqueue/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.utils.concurrentcircularqueue/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.utils.concurrentcircularqueue.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.utils.concurrentcircularqueue/)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Utils.ConcurrentCircularQueue
### A thread-safe collection type for a fixed length of elements, overwriting the oldest element

## Installation

```
dotnet add package Soenneker.Utils.ConcurrentCircularQueue
```

## Usage

### Creating an Instance
Instantiate a `ConcurrentCircularQueue<T>` object by specifying the maximum size of the queue.
```csharp
var myQueue = new ConcurrentCircularQueue<int>(3); // Creates a queue with a maximum size of 3.
```

### Enqueueing Items
Add an item to the queue. If the queue has reached its maximum size, the oldest item will be removed.
```csharp
myQueue.Enqueue(1);
myQueue.Enqueue(2);
myQueue.Enqueue(3);
myQueue.Enqueue(4);

// The queue now contains 2, 3, and 4.
```

### Dequeueing Items
Remove and return the oldest item from the queue.
```csharp
bool success = myQueue.TryDequeue(out var item);
```

### Checking If an Item Exists
Determine if a specific item is in the queue.
```csharp
bool exists = myQueue.Contains(item);
```

### Count
Retrieve the current number of items in the queue.
```csharp
int currentCount = myQueue.Count;
```