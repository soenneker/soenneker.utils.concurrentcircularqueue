using AwesomeAssertions;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace Soenneker.Utils.ConcurrentCircularQueue.Tests;

public class ConcurrentCircularQueueTests
{
    [Test]
    public async ValueTask Enqueue_AddsItemToQueue(CancellationToken cancellationToken)
    {
        // Arrange
        var queue = new ConcurrentCircularQueue<int>(3);

        // Act
        await queue.Enqueue(1);

        // Assert
        (await queue.Count()).Should().Be(1);
    }

    [Test]
    public async ValueTask Enqueue_WithMaxSize_RemovesOldestItem(CancellationToken cancellationToken)
    {
        // Arrange
        var queue = new ConcurrentCircularQueue<int>(2);
        await queue.Enqueue(1);
        await queue.Enqueue(2);

        // Act
        await queue.Enqueue(3);

        // Assert
        (await queue.Count()).Should().Be(2);
        (await queue.Contains(1)).Should().BeFalse(); // Oldest item should be removed
    }

    [Test]
    public async ValueTask TryDequeue_RemovesItemFromQueue(CancellationToken cancellationToken)
    {
        // Arrange
        var queue = new ConcurrentCircularQueue<int>(3);
        await queue.Enqueue(1);

        // Act
        (bool success, int result) = await queue.TryDequeue();

        // Assert
        success.Should().BeTrue();
        result.Should().Be(1);
        (await queue.Count()).Should().Be(0);
    }

    [Test]
    public async ValueTask TryDequeue_ReturnsFalse_WhenQueueIsEmpty(CancellationToken cancellationToken)
    {
        // Arrange
        var queue = new ConcurrentCircularQueue<int>(3);

        // Act
        (bool success, int result) = await queue.TryDequeue();

        // Assert
        success.Should().BeFalse();
        result.Should().Be(default(int)); // Default value when dequeue fails
        (await queue.Count()).Should().Be(0);
    }

    [Test]
    public void Enqueue_WithZeroMaxSize_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new ConcurrentCircularQueue<int>(0));
    }

    [Test]
    public async ValueTask Contains_ReturnsTrue_WhenItemExists(CancellationToken cancellationToken)
    {
        // Arrange
        var queue = new ConcurrentCircularQueue<int>(3);
        await queue.Enqueue(1);

        // Act & Assert
        (await queue.Contains(1)).Should().BeTrue();
    }

    [Test]
    public async ValueTask Contains_ReturnsFalse_WhenItemDoesNotExist(CancellationToken cancellationToken)
    {
        // Arrange
        var queue = new ConcurrentCircularQueue<int>(3);
        await queue.Enqueue(1);

        // Act & Assert
        (await queue.Contains(2)).Should().BeFalse();
    }
}