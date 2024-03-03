using Xunit;
using FluentAssertions;
using System;
using System.Threading.Tasks;

namespace Soenneker.Utils.ConcurrentCircularQueue.Tests;

public class ConcurrentCircularQueueTests
{
    [Fact]
    public async Task Enqueue_AddsItemToQueue()
    {
        // Arrange
        var queue = new ConcurrentCircularQueue<int>(3);

        // Act
        await queue.Enqueue(1);

        // Assert
        (await queue.Count()).Should().Be(1);
    }

    [Fact]
    public async Task Enqueue_WithMaxSize_RemovesOldestItem()
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

    [Fact]
    public async Task TryDequeue_RemovesItemFromQueue()
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

    [Fact]
    public async Task TryDequeue_ReturnsFalse_WhenQueueIsEmpty()
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

    [Fact]
    public void Enqueue_WithZeroMaxSize_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new ConcurrentCircularQueue<int>(0));
    }

    [Fact]
    public async Task Contains_ReturnsTrue_WhenItemExists()
    {
        // Arrange
        var queue = new ConcurrentCircularQueue<int>(3);
        await queue.Enqueue(1);

        // Act & Assert
        (await queue.Contains(1)).Should().BeTrue();
    }

    [Fact]
    public async Task Contains_ReturnsFalse_WhenItemDoesNotExist()
    {
        // Arrange
        var queue = new ConcurrentCircularQueue<int>(3);
        await queue.Enqueue(1);

        // Act & Assert
        (await queue.Contains(2)).Should().BeFalse();
    }
}