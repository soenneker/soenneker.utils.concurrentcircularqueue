using Soenneker.Utils.ConcurrentCircularQueue.Abstract;
using System.Collections.Concurrent;
using System;
using System.Linq;
using System.Threading.Tasks;
using Soenneker.Asyncs.Locks;
using Soenneker.Extensions.ValueTask;
using Soenneker.Atomics.ValueInts;

namespace Soenneker.Utils.ConcurrentCircularQueue;

/// <inheritdoc cref="IConcurrentCircularQueue{T}"/>
public class ConcurrentCircularQueue<T> : IConcurrentCircularQueue<T>
{
    private readonly ConcurrentQueue<T> _queue = new();
    private readonly int _maxSize;

    private ValueAtomicInt _count;

    private readonly bool _locking;
    private readonly AsyncLock? _asyncLock;

    public ConcurrentCircularQueue(int maxSize, bool locking = false)
    {
        if (maxSize <= 0)
            throw new ArgumentException("MaxSize must be greater than zero.");

        _maxSize = maxSize;
        _locking = locking;

        if (_locking)
            _asyncLock = new AsyncLock();

        _count = new ValueAtomicInt(0);
    }

    public async ValueTask<bool> Contains(T item)
    {
        if (!_locking)
            return _queue.Contains(item);

        using (await _asyncLock!.Lock()
                                .NoSync())
            return _queue.Contains(item);
    }

    public async ValueTask Enqueue(T item)
    {
        if (!_locking)
        {
            EnqueueInternal(item);
            return;
        }

        using (await _asyncLock!.Lock()
                                .NoSync())
            EnqueueInternal(item);
    }

    private void EnqueueInternal(T item)
    {
        _queue.Enqueue(item);

        int newCount = _count.Increment();

        // Ensure the queue size does not exceed maxSize (handles bursts/contended callers)
        while (newCount > _maxSize && _queue.TryDequeue(out _))
        {
            newCount = _count.Decrement();
        }
    }

    public async ValueTask<(bool success, T? result)> TryDequeue()
    {
        if (!_locking)
        {
            bool success = TryDequeueInternal(out T? result);
            return (success, result);
        }

        using (await _asyncLock!.Lock()
                                .NoSync())
        {
            bool success = TryDequeueInternal(out T? result);
            return (success, result);
        }
    }

    private bool TryDequeueInternal(out T result)
    {
        bool dequeueResult = _queue.TryDequeue(out result);

        if (dequeueResult)
            _count.Decrement();

        return dequeueResult;
    }

    public async ValueTask<int> Count()
    {
        if (!_locking)
            return _count.Read();

        using (await _asyncLock!.Lock()
                                .NoSync())
            return _count.Read();
    }
}