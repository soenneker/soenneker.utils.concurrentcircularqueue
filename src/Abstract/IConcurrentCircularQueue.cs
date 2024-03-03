using System.Diagnostics.Contracts;

namespace Soenneker.Utils.ConcurrentCircularQueue.Abstract;

/// <summary>
/// A thread-safe collection type for a fixed length of elements, overwriting the oldest element
/// </summary>
/// <typeparam name="T">The type of elements in the queue.</typeparam>
public interface IConcurrentCircularQueue<T>
{
    /// <summary>
    /// Checks whether the queue contains the specified item.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns><see langword="true"/> if the item is found in the queue; otherwise, <see langword="false"/>.</returns>
    [Pure]
    bool Contains(T item);

    /// <summary>
    /// Enqueues an item into the queue.
    /// </summary>
    /// <param name="item">The item to enqueue.</param>
    void Enqueue(T item);

    /// <summary>
    /// Tries to dequeue an item from the queue.
    /// </summary>
    /// <param name="result">When this method returns, contains the dequeued item, if the operation succeeded; otherwise, the default value.</param>
    /// <returns><see langword="true"/> if an item was dequeued successfully; otherwise, <see langword="false"/>.</returns>
    bool TryDequeue(out T result);

    /// <summary>
    /// Gets the number of items in the queue.
    /// </summary>
    [Pure]
    int Count { get; }
}