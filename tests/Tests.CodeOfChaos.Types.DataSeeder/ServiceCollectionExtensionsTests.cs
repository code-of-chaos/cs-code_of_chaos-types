// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
    
    // Test Seeder Service Implementation for Testing DI
    public class TestSeederService(IServiceProvider sp) : OneTimeDataSeederService(sp, sp.GetRequiredService<ILogger<TestSeederService>>());
}
