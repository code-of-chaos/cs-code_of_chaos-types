// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Tests.CodeOfChaos.Types.DataSeeder;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ServiceCollectionExtensionsTests {
    [Test]
    public async Task AddOneTimeDataSeeder_ShouldRegister_OneTimeDataSeederService() {
        // Arrange
        IServiceCollection services = new ServiceCollection().AddLogging();

        // Act
        services.AddOneTimeDataSeeder<TestSeederService>();
        var hostedService = services.BuildServiceProvider().GetRequiredService<IHostedService>();

        // Assert
        await Assert.That(hostedService)
            .IsNotNull()
            .And.IsTypeOf<TestSeederService>();
    }

    [Test]
    public async Task AddOneTimeDataSeeder_ShouldRegisterUsingFactory() {
        // Arrange
        IServiceCollection services = new ServiceCollection().AddLogging();

        // Act
        services.AddOneTimeDataSeeder(sp => new TestSeederService(sp));
        var hostedService = services.BuildServiceProvider().GetRequiredService<IHostedService>();

        // Assert
        await Assert.That(hostedService)
            .IsNotNull()
            .And.IsTypeOf<TestSeederService>();
    }

    [Test]
    public async Task AddOneTimeDataSeeder_ShouldRegisterUsingAction() {
        // Arrange
        IServiceCollection services = new ServiceCollection()
                .AddLogging()
                .AddSingleton<TestSeeder>()
            ;

        // Act
        services.AddOneTimeDataSeeder<TestSeederService>(seeder => seeder.AddSeeder<TestSeeder>());
        var hostedService = services.BuildServiceProvider().GetRequiredService<IHostedService>();
        var seeder = hostedService as TestSeederService;
        bool? hasTestSeeder = seeder?.OverloadSeeders.Any(s => s.SeederTypes.Any(s2 => s2 == typeof(TestSeeder)));
        bool? hasTestSeederType = seeder?.OverloadSeederTypes.Contains(typeof(TestSeeder));
        bool? collectedRemainders = seeder?.OverloadCollectedRemainders;

        // Assert
        await Assert.That(hostedService)
            .IsNotNull()
            .And.IsTypeOf<TestSeederService>();
        await Assert.That(hasTestSeeder).IsTrue();
        await Assert.That(hasTestSeederType).IsTrue();
        await Assert.That(collectedRemainders).IsFalse();
    }

    // Test Seeder Service Implementation for Testing DI
    public class TestSeederService(IServiceProvider sp) : OneTimeDataSeederService(sp, sp.GetRequiredService<ILogger<TestSeederService>>()) {
        public ConcurrentQueue<SeederGroup> OverloadSeeders => Seeders;
        public ConcurrentBag<Type> OverloadSeederTypes => SeederTypes;
        public bool OverloadCollectedRemainders => CollectedRemainders;
    }

    public class TestSeeder : Seeder {
        public override Task SeedAsync(CancellationToken ct = default) => Task.CompletedTask;
    }
}
