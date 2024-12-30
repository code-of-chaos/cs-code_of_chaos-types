// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.Types;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.CodeOfChaos.Types.DataSeeder;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SeederTests {
    [Test]
    public async Task StartAsync_ShouldLogAndReturn_WhenShouldSeedReturnsFalse() {
        // Arrange
        var loggerMock = new Mock<ILogger>();
        var seeder = new TestSeeder {
            ShouldSeedResult = false// Simulate ShouldSeedAsync returning false
        };

        // Act
        await seeder.StartAsync(loggerMock.Object);

        // Assert
        loggerMock.Verify(
            expression: logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!.Contains("Skipping seeding")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
        await Assert.That(seeder.SeedWasCalled).IsFalse();
    }

    [Test]
    public async Task StartAsync_ShouldCallSeed_WhenShouldSeedReturnsTrue() {
        // Arrange
        var loggerMock = new Mock<ILogger>();
        var seeder = new TestSeeder {
            ShouldSeedResult = true// Simulate ShouldSeedAsync returning true
        };

        // Act
        await seeder.StartAsync(loggerMock.Object);

        // Assert
        loggerMock.Verify(
            expression: logger => logger.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Never);// No additional logging should occur during SeedAsync
        await Assert.That(seeder.SeedWasCalled).IsTrue();
    }

    [Test]
    public async Task StartAsync_ShouldRespectCancellationToken() {
        // Arrange
        var loggerMock = new Mock<ILogger>();
        var seeder = new TestSeeder();
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();// Simulate cancellation

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(async () => {
            await seeder.StartAsync(loggerMock.Object, cts.Token);
        });
    }

    // Test implementation of Seeder for unit tests
    public class TestSeeder : Seeder {
        public bool ShouldSeedResult { get; set; } = true;
        public bool SeedWasCalled { get; private set; }

        public override Task<bool> ShouldSeedAsync(CancellationToken ct = default) =>
            ShouldSeedResult ? Task.FromResult(true) : Task.FromResult(false);

        public override Task SeedAsync(CancellationToken ct = default) {
            if (ct.IsCancellationRequested) {
                ct.ThrowIfCancellationRequested();
            }

            SeedWasCalled = true;
            return Task.CompletedTask;
        }
    }
}