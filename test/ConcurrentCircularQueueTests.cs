using Xunit;
using FluentAssertions;
using System;

namespace Soenneker.Utils.ConcurrentCircularQueue.Tests;

public class ConcurrentCircularQueueTests
{
    [Fact]
    public void Enqueue_AddsItemToQueue()
    {
        // Arrange
        var queue = new ConcurrentCircularQueue<int>(3);

        // Act
        queue.Enqueue(1);

        // Assert
        queue.Count.Should().Be(1);
    }

    [Fact]
    public void Enqueue_WithMaxSize_RemovesOldestItem()
    {
        // Arrange
        var queue = new ConcurrentCircularQueue<int>(2);
        queue.Enqueue(1);
        queue.Enqueue(2);

        // Act
        queue.Enqueue(3);

        // Assert
        queue.Count.Should().Be(2);
        queue.Contains(1).Should().BeFalse(); // Oldest item should be removed
    }

    [Fact]
    public void TryDequeue_RemovesItemFromQueue()
    {
        // Arrange
        var queue = new ConcurrentCircularQueue<int>(3);
        queue.Enqueue(1);

        // Act
        int result;
        bool dequeueResult = queue.TryDequeue(out result);

        // Assert
        dequeueResult.Should().BeTrue();
        result.Should().Be(1);
        queue.Count.Should().Be(0);
    }

    [Fact]
    public void TryDequeue_ReturnsFalse_WhenQueueIsEmpty()
    {
        // Arrange
        var queue = new ConcurrentCircularQueue<int>(3);

        // Act
        int result;
        bool dequeueResult = queue.TryDequeue(out result);

        // Assert
        dequeueResult.Should().BeFalse();
        result.Should().Be(default(int)); // Default value when dequeue fails
        queue.Count.Should().Be(0);
    }

    [Fact]
    public void Enqueue_WithZeroMaxSize_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new ConcurrentCircularQueue<int>(0));
    }

    [Fact]
    public void Contains_ReturnsTrue_WhenItemExists()
    {
        // Arrange
        var queue = new ConcurrentCircularQueue<int>(3);
        queue.Enqueue(1);

        // Act & Assert
        queue.Contains(1).Should().BeTrue();
    }

    [Fact]
    public void Contains_ReturnsFalse_WhenItemDoesNotExist()
    {
        // Arrange
        var queue = new ConcurrentCircularQueue<int>(3);
        queue.Enqueue(1);

        // Act & Assert
        queue.Contains(2).Should().BeFalse();
    }
}