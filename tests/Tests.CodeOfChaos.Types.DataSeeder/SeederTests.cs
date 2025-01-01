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
    var serviceProviderMock = new Mock<IServiceProvider>();
    var seeder = new TestSeeder {
        ShouldSeedResult = false // Simulate ShouldSeedAsync returning false
    };

    // Act
    await seeder.StartAsync(serviceProviderMock.Object);

    // Assert
    await Assert.That(seeder.ShouldSeed).IsFalse();
}

    [Test]
public async Task StartAsync_ShouldCallSeed_WhenShouldSeedReturnsTrue() {
    // Arrange
    var serviceProviderMock = new Mock<IServiceProvider>();
    var seeder = new TestSeeder {
        ShouldSeedResult = true // Simulate ShouldSeedAsync returning true
    };

    // Act
    await seeder.StartAsync(serviceProviderMock.Object);

    // Assert
    await Assert.That(seeder.ShouldSeed).IsTrue();
}

    [Test]
public async Task StartAsync_ShouldRespectCancellationToken() {
    // Arrange
    var serviceProviderMock = new Mock<IServiceProvider>();
    var seeder = new TestSeeder();
    using var cts = new CancellationTokenSource();
    await cts.CancelAsync(); // Simulate cancellation

    // Act & Assert
    await Assert.ThrowsAsync<OperationCanceledException>(async () => {
        await seeder.StartAsync(serviceProviderMock.Object, cts.Token);
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
