using Soenneker.Utils.ConcurrentCircularQueue.Abstract;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Soenneker.Asyncs.Locks;
using Soenneker.Extensions.ValueTask;
using Soenneker.Atomics.ValueInts;

namespace Soenneker.Utils.ConcurrentCircularQueue;

/// <inheritdoc cref="IConcurrentCircularQueue{T}"/>
public sealed class ConcurrentCircularQueue<T> : IConcurrentCircularQueue<T>
{
    private readonly ConcurrentQueue<T> _queue = new();
    private readonly int _maxSize;

    private ValueAtomicInt _count;

    private readonly bool _locking;
    private readonly AsyncLock? _asyncLock;

    public ConcurrentCircularQueue(int maxSize, bool locking = false)
    {
        if (maxSize <= 0)
            throw new ArgumentException("MaxSize must be greater than zero.", nameof(maxSize));

        _maxSize = maxSize;
        _locking = locking;

        if (locking)
            _asyncLock = new AsyncLock();

        _count = new ValueAtomicInt(0);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<bool> Contains(T item)
    {
        if (!_locking)
            return new ValueTask<bool>(QueueContains(item));

        return ContainsLocked(item);
    }

    private async ValueTask<bool> ContainsLocked(T item)
    {
        using (await _asyncLock!.Lock()
                                .NoSync())
            return QueueContains(item);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool QueueContains(T item)
    {
        var comparer = EqualityComparer<T>.Default;

        foreach (T current in _queue)
        {
            if (comparer.Equals(current, item))
                return true;
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask Enqueue(T item)
    {
        if (!_locking)
        {
            EnqueueInternal(item);
            return ValueTask.CompletedTask;
        }

        return EnqueueLocked(item);
    }

    private async ValueTask EnqueueLocked(T item)
    {
        using (await _asyncLock!.Lock()
                                .NoSync())
            EnqueueInternal(item);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<(bool success, T? result)> TryDequeue()
    {
        if (!_locking)
        {
            bool success = TryDequeueInternal(out T? result);
            return new ValueTask<(bool success, T? result)>((success, result));
        }

        return TryDequeueLocked();
    }

    private async ValueTask<(bool success, T? result)> TryDequeueLocked()
    {
        using (await _asyncLock!.Lock()
                                .NoSync())
        {
            bool success = TryDequeueInternal(out T? result);
            return (success, result);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryDequeueInternal(out T result)
    {
        bool dequeueResult = _queue.TryDequeue(out result);

        if (dequeueResult)
            _count.Decrement();

        return dequeueResult;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<int> Count()
    {
        if (!_locking)
            return new ValueTask<int>(_count.Read());

        return CountLocked();
    }

    private async ValueTask<int> CountLocked()
    {
        using (await _asyncLock!.Lock()
                                .NoSync())
            return _count.Read();
    }
}