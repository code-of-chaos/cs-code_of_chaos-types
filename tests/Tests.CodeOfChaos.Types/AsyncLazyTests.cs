// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using JetBrains.Annotations;
using Moq;
using System.Collections.Concurrent;
using System.Reflection;

namespace Tests.CodeOfChaos.Types;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[TestSubject(typeof(AsyncLazy<>))]
public class AsyncLazyTests {
    [Test]
    public async Task GetValueAsync_ShouldReturnValue_FromTestoryMethod() {
        // Arrange
        int expectedValue = 42;
        var lazy = new AsyncLazy<int>(_ => Task.FromResult(expectedValue));

        // Act
        int result = await lazy.GetValueAsync();

        // Assert
        await Assert.That(result).IsEqualTo(expectedValue);
    }

    [Test]
    public async Task GetValueAsync_ShouldInitializeValueOnlyOnce() {
        // Arrange
        int callCount = 0;
        var lazy = new AsyncLazy<int>(_ => {
            callCount++;
            return Task.FromResult(42);
        });

        // Act
        await lazy.GetValueAsync();
        await lazy.GetValueAsync();// Call again to ensure the factory is only called once

        // Assert
        await Assert.That(callCount).IsEqualTo(1);
    }

    [Test]
    public async Task GetValueAsync_ShouldHandleCancellationToken() {
        // Arrange
        var cts = new CancellationTokenSource();
        var lazy = new AsyncLazy<int>(static ct => {
            ct.ThrowIfCancellationRequested();
            return Task.FromResult(42);
        });

        // Act & Assert
        await cts.CancelAsync();
        await Assert.ThrowsAsync<OperationCanceledException>(() => lazy.GetValueAsync(cts.Token));
    }

    [Test]
    public async Task DisposeAsync_ShouldDisposeIDisposableResource() {
        // Arrange
        var mockDisposable = new Mock<IDisposable>();
        var lazy = new AsyncLazy<IDisposable>(_ => Task.FromResult(mockDisposable.Object));

        // Act
        await lazy.GetValueAsync();// We need to set the value before disposing, else we just skip disposing most of the time
        await lazy.DisposeAsync();

        // Assert
        mockDisposable.Verify(expression: m => m.Dispose(), Times.Once);
    }

    [Test]
    public async Task DisposeAsync_ShouldDisposeIAsyncDisposableResource() {
        // Arrange
        var mockAsyncDisposable = new Mock<IAsyncDisposable>();
        mockAsyncDisposable
            .Setup(d => d.DisposeAsync())
            .Returns(ValueTask.CompletedTask);// Changed this line

        var lazy = new AsyncLazy<IAsyncDisposable>(_ => Task.FromResult(mockAsyncDisposable.Object));

        // Act
        await lazy.GetValueAsync();// We need to set the value before disposing, else we just skip disposing most of the time
        await lazy.DisposeAsync();

        // Assert
        mockAsyncDisposable.Verify(expression: d => d.DisposeAsync(), Times.Once);
    }

    [Test]
    public async Task DisposeAsync_ShouldResetValueToNull() {
        // Arrange
        var lazy = new AsyncLazy<int>(_ => Task.FromResult(42));

        // Act
        await lazy.GetValueAsync();// Load the value
        await lazy.DisposeAsync();// Dispose to reset it

        // Use reflection to access the private `_value` field
        FieldInfo? privateField = typeof(AsyncLazy<int>).GetField("_value", BindingFlags.NonPublic | BindingFlags.Instance);
        object? value = privateField?.GetValue(lazy);

        // Assert
        await Assert.That(value).IsNull();
    }

    [Test]
    public async Task GetValueAsync_ShouldEnsureAtomicity() {
        // Arrange
        int callCount = 0;
        var lazy = new AsyncLazy<int>(_ => {
            Interlocked.Increment(ref callCount);
            return Task.FromResult(42);
        });
        const int count = 100;
        var tasks = new List<Task<int>>();

        // Act
        for (int i = 0; i < count; i++) {
            tasks.Add(lazy.GetValueAsync());// Initiate concurrent access to the value
        }

        int[] results = await Task.WhenAll(tasks);

        // Assert
        await Assert.That(callCount).IsEqualTo(1).Because("Factory method must be called only once");
        foreach (int result in results) {
            await Assert.That(result).IsEqualTo(42).Because("All results must match the expected value");
        }
    }

    [Test]
    public async Task GetValueAsync_ShouldEnsureAtomicity_UsingParallel() {
        // Arrange
        int callCount = 0;
        var lazy = new AsyncLazy<int>(_ => {
            Interlocked.Increment(ref callCount);
            return Task.FromResult(42);
        });
        const int count = 100;
        int[] results = new int[count];// To store the results of parallel invocations
        var parallelTasks = new List<Task>();

        // Act
        Parallel.For(0, count, body: i => {
            parallelTasks.Add(Task.Run(async () => results[i] = await lazy.GetValueAsync()));
        });

        await Task.WhenAll(parallelTasks);

        // Assert
        await Assert.That(callCount).IsEqualTo(1).Because("Factory method must be called only once");
        foreach (int result in results) {
            await Assert.That(result).IsEqualTo(42).Because("All results must match the expected value");
        }
    }

    [Test]
    public async Task GetValueAsync_ShouldEnsureAtomicAccessToIncrementedValue_UsingTasks() {
        // Arrange
        int sharedValue = 0;
        var lazy = new AsyncLazy<int>(_ => {
            Interlocked.Exchange(ref sharedValue, 0);// Initialize the shared value atomically
            return Task.FromResult(sharedValue);
        });
        const int count = 100;

        // Increment the value safely within `AsyncLazy`
        async Task<int> IncrementValueAsync() {
            int _ = await lazy.GetValueAsync();
            return Interlocked.Increment(ref sharedValue);
        }

        var tasks = new List<Task<int>>();

        // Act
        for (int i = 0; i < count; i++) {
            tasks.Add(IncrementValueAsync());
        }

        int[] results = await Task.WhenAll(tasks);

        // Assert: Ensure all increments are atomic and unique
        await Assert.That(results).HasDistinctItems().Because("All results must be unique due to atomic increments");
        await Assert.That(results.Min()).IsEqualTo(1).Because("The first value must start with 1");
        await Assert.That(results.Max()).IsEqualTo(count).Because("The last value must increment to 10");
        await Assert.That(sharedValue).IsEqualTo(count).Because("The sharedValue must be correctly incremented to 10");
    }

    [Test]
    public async Task GetValueAsync_ShouldEnsureAtomicAccessToIncrementedValue_UsingParallel() {
        // Arrange
        int sharedValue = 0;
        var lazy = new AsyncLazy<int>(_ => {
            Interlocked.Exchange(ref sharedValue, 0);// Initialize the shared value atomically
            return Task.FromResult(sharedValue);
        });
        const int count = 100;
        var results = new ConcurrentBag<int>();

        // Increment the value safely within `AsyncLazy`
        async Task<int> IncrementValueAsync() {
            int _ = await lazy.GetValueAsync();
            return Interlocked.Increment(ref sharedValue);
        }

        // Act
        Parallel.For(0, count, body: _ => {
            int result = Task.Run(IncrementValueAsync).Result;// Run the async method inside parallel execution
            results.Add(result);
        });

        // Assert: Ensure all increments are atomic and unique
        await Assert.That(results).HasDistinctItems().Because("All results must be unique due to atomic increments");
        await Assert.That(results.Min()).IsEqualTo(1).Because("The first value must start with 1");
        await Assert.That(results.Max()).IsEqualTo(count).Because("The last value must increment to 10");
        await Assert.That(sharedValue).IsEqualTo(count).Because("The sharedValue must be correctly incremented to 10");
    }
}
