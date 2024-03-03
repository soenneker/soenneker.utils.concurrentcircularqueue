using Soenneker.Utils.ConcurrentCircularQueue.Abstract;
using System.Collections.Concurrent;
using System.Threading;
using System;
using System.Linq;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace Soenneker.Utils.ConcurrentCircularQueue;

/// <inheritdoc cref="IConcurrentCircularQueue{T}"/>
public class ConcurrentCircularQueue<T> : IConcurrentCircularQueue<T>
{
    private readonly ConcurrentQueue<T> _queue = new();
    private readonly int _maxSize;
    private int _count;
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
    }

    public async ValueTask<bool> Contains(T item)
    {
        if (!_locking)
            return _queue.Contains(item);

        using (await _asyncLock!.LockAsync().ConfigureAwait(false))
        {
            return _queue.Contains(item);
        }
    }

    public async ValueTask Enqueue(T item)
    {
        if (!_locking)
        {
            EnqueueInternal(item);
            return;
        }

        using (await _asyncLock!.LockAsync().ConfigureAwait(false))
        {
            EnqueueInternal(item);
        }
    }

    private void EnqueueInternal(T item)
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

    public async ValueTask<(bool success, T? result)> TryDequeue()
    {
        T? result;

        if (!_locking)
        {
            bool success = TryDequeueInternal(out result);
            return (success, result);
        }

        using (await _asyncLock!.LockAsync().ConfigureAwait(false))
        {
            bool success = TryDequeueInternal(out result);
            return (success, result);
        }
    }

    private bool TryDequeueInternal(out T result)
    {
        bool dequeueResult = _queue.TryDequeue(out result);

        if (dequeueResult)
        {
            Interlocked.Decrement(ref _count);
        }

        return dequeueResult;
    }

    public async ValueTask<int> Count()
    {
        if (!_locking)
            return _count;

        using (await _asyncLock!.LockAsync().ConfigureAwait(false))
        {
            return _count;
        }
    }
}