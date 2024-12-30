// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.CodeOfChaos.Types.DataSeeder;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SeederGroupTests {
    [Test]
    public async Task Constructor_ShouldInitializeEmptySeederGroup() {
        // Arrange
        IServiceProvider mockServiceProvider = new Mock<IServiceProvider>().Object;

        // Act
        var seederGroup = new SeederGroup(mockServiceProvider);

        // Assert
        await Assert.That(seederGroup)
            .IsEmpty()
            .And.HasCount().EqualToZero();
    }

    [Test]
    public async Task Constructor_ShouldInitializeSeederGroupFromArray() {
        // Arrange
        ISeeder mockSeeder1 = new Mock<ISeeder>().Object;
        ISeeder mockSeeder2 = new Mock<ISeeder>().Object;
        ISeeder[] seeders = [mockSeeder1, mockSeeder2];
        IServiceProvider mockServiceProvider = new Mock<IServiceProvider>().Object;

        // Act
        var seederGroup = new SeederGroup(seeders, mockServiceProvider);

        // Assert
        await Assert.That(seederGroup)
            .IsNotEmpty()
            .And.HasCount().EqualTo(2)
            .And.Contains(mockSeeder1)
            .And.Contains(mockSeeder2);
    }

    [Test]
    public async Task AddSeeder_ShouldAddSeederInstance() {
        // Arrange
        ISeeder mockSeeder = new Mock<ISeeder>().Object;
        IServiceProvider mockServiceProvider = new Mock<IServiceProvider>().Object;
        var seederGroup = new SeederGroup(mockServiceProvider);

        // Act
        seederGroup = seederGroup.AddSeeder(mockSeeder);

        // Assert
        await Assert.That(seederGroup)
            .IsNotEmpty()
            .And.HasCount().EqualTo(1)
            .And.Contains(mockSeeder);
    }

    [Test]
    public async Task AddSeeder_ShouldThrowException_WhenSeederAlreadyExists() {
        // Arrange
        ISeeder mockSeeder = new Mock<ISeeder>().Object;
        IServiceProvider mockServiceProvider = new Mock<IServiceProvider>().Object;
        SeederGroup seederGroup = new SeederGroup(mockServiceProvider).AddSeeder(mockSeeder);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => seederGroup.AddSeeder(mockSeeder));
        await Assert.That(seederGroup)
            .HasCount().EqualTo(1);
    }

    [Test]
    public async Task AddSeeder_WithType_ShouldAddActivatorCreatedSeeder() {
        // Arrange
        ServiceProvider mockServiceProvider = new ServiceCollection()
            .AddSingleton<MockSeeder>()
            .AddLogging()
            .BuildServiceProvider();
        var seederGroup = new SeederGroup(mockServiceProvider);

        // Act
        seederGroup = seederGroup.AddSeeder<MockSeeder>();

        // Assert
        await Assert.That(seederGroup)
            .IsNotEmpty()
            .And.HasCount().EqualTo(1);
        await Assert.That(seederGroup.First())
            .IsTypeOf<MockSeeder>();
    }

    [Test]
    public async Task AddSeeder_WithFactory_ShouldAddFactoryCreatedSeeder() {
        // Arrange
        ISeeder mockSeeder = new Mock<ISeeder>().Object;
        IServiceProvider mockServiceProvider = new Mock<IServiceProvider>().Object;
        var seederGroup = new SeederGroup(mockServiceProvider);

        // Act
        seederGroup = seederGroup.AddSeeder(_ => mockSeeder);

        // Assert
        await Assert.That(seederGroup)
            .IsNotEmpty()
            .And.HasCount().EqualTo(1)
            .And.Contains(mockSeeder);
    }

    [Test]
    public async Task AddSeeder_WithFactory_ShouldAddFactoryCreatedSeeder_WithAction() {
        // Arrange
        ISeeder mockSeeder = new Mock<ISeeder>().Object;
        IServiceProvider mockServiceProvider = new Mock<IServiceProvider>().Object;
        var seederGroup = new SeederGroup(mockServiceProvider);

        // Act
        seederGroup = seederGroup.AddSeeder(() => mockSeeder);

        // Assert
        await Assert.That(seederGroup)
            .IsNotEmpty()
            .And.HasCount().EqualTo(1)
            .And.Contains(mockSeeder);
    }

    [Test]
    public async Task Enumerator_ShouldIterateOverSeeders() {
        // Arrange
        ISeeder mockSeeder1 = new Mock<ISeeder>().Object;
        ISeeder mockSeeder2 = new Mock<ISeeder>().Object;
        IServiceProvider mockServiceProvider = new Mock<IServiceProvider>().Object;
        SeederGroup seederGroup = new SeederGroup(mockServiceProvider)
            .AddSeeder(mockSeeder1)
            .AddSeeder(mockSeeder2);

        // Act
        List<ISeeder> seeders = seederGroup.ToList();

        // Assert
        await Assert.That(seeders)
            .IsNotEmpty()
            .And.HasCount().EqualTo(2)
            .And.Contains(mockSeeder1)
            .And.Contains(mockSeeder2);
        await Assert.That(seeders.ElementAtOrDefault(0)).IsEqualTo(mockSeeder1);
        await Assert.That(seeders.ElementAtOrDefault(1)).IsEqualTo(mockSeeder2);
    }

    // Mock Seeder for testing
    public class MockSeeder : ISeeder {
        public Task StartAsync(ILogger logger, CancellationToken ct = default) => Task.CompletedTask;
        public Task<bool> ShouldSeedAsync(CancellationToken ct = default) => Task.FromResult(true);
        public Task SeedAsync(CancellationToken ct = default) => Task.CompletedTask;
    }
}
