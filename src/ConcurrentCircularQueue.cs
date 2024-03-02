using Soenneker.Utils.ConcurrentCircularQueue.Abstract;
using System.Collections.Concurrent;
using System.Threading;
using System;
using System.Linq;

namespace Soenneker.Utils.ConcurrentCircularQueue;

/// <inheritdoc cref="IConcurrentCircularQueue{T}"/>
public class ConcurrentCircularQueue<T> : IConcurrentCircularQueue<T>
{
    private readonly ConcurrentQueue<T> _queue = new();
    private readonly int _maxSize;
    private int _count;

    public ConcurrentCircularQueue(int maxSize)
    {
        if (maxSize <= 0)
            throw new ArgumentException("MaxSize must be greater than zero.");

        _maxSize = maxSize;
    }

    public bool Contains(T item)
    {
        return _queue.Contains(item);
    }

    public void Enqueue(T item)
    {
        _queue.Enqueue(item);
        int countAfterIncrement = Interlocked.Increment(ref _count);

        // Ensure the queue size does not exceed maxSize
        if (countAfterIncrement <= _maxSize) 
            return;

        if (_queue.TryDequeue(out T _))
        {
            Interlocked.Decrement(ref _count);
        }
    }

    public bool TryDequeue(out T result)
    {
        bool dequeueResult = _queue.TryDequeue(out result);

        if (dequeueResult)
        {
            Interlocked.Decrement(ref _count);
        }

        return dequeueResult;
    }

    public int Count => _count;
}