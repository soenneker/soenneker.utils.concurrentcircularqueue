using System.Diagnostics.Contracts;
using System.Threading.Tasks;

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
    ValueTask<bool> Contains(T item);

    /// <summary>
    /// Enqueues an item into the queue.
    /// </summary>
    /// <param name="item">The item to enqueue.</param>
    ValueTask Enqueue(T item);

    /// <summary>
    /// Tries to dequeue an item from the queue.
    /// </summary>
    ValueTask<(bool success, T? result)> TryDequeue();

    /// <summary>
    /// Gets the number of items in the queue.
    /// </summary>
    [Pure]
    ValueTask<int> Count();
}