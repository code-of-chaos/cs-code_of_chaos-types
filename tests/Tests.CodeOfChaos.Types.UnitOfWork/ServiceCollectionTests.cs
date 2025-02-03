// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.Types.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.CodeOfChaos.Types.UnitOfWork;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ServiceCollectionTests {
    private IServiceCollection _services = default!;

    [Before(Test)]
    public void Setup() {
        _services = new ServiceCollection();
        _services.AddLogging();
        _services.AddDbContextFactory<DefaultDbContext>();
        _services.AddDbContextFactory<OtherDbContext>();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Tests
    // -----------------------------------------------------------------------------------------------------------------
    
    [Test]
    public async Task AddUnitOfWork_ShouldRegister_UnitOfWorkServices() {
        // Arrange
        _services.AddUnitOfWork<DefaultDbContext>();
        
        ServiceProvider provider = _services.BuildServiceProvider();
        
        // Act
        var unitOfWorkFactory = provider.GetService<IUnitOfWorkFactory>();
        var unitOfWork = provider.GetService<IUnitOfWork>();

        // Assert
        await Assert.That(unitOfWorkFactory).IsNotNull()
            .And.IsTypeOf<UnitOfWorkFactory<DefaultDbContext>>();
        
        await Assert.That(unitOfWork).IsNotNull()
            .And.IsTypeOf<UnitOfWork<DefaultDbContext>>();
    }

    [Test]
    public async Task AddUnitOfWork_ShouldRegister_UnitOfWorkServices_WithKeyedService() {
        // Arrange
        _services.AddUnitOfWork<DefaultDbContext>("default");
        _services.AddUnitOfWork<OtherDbContext>("other");
        
        ServiceProvider provider = _services.BuildServiceProvider();
        
        // Act
        var defaultUnitOfWorkFactory = provider.GetKeyedService<IUnitOfWorkFactory>("default");
        var defaultUnitOfWork = provider.GetKeyedService<IUnitOfWork>("default");
        var otherUnitOfWorkFactory = provider.GetKeyedService<IUnitOfWorkFactory>("other");
        var otherUnitOfWork = provider.GetKeyedService<IUnitOfWork>("other");
        
        // Assert
        await Assert.That(defaultUnitOfWorkFactory).IsNotNull()
            .And.IsTypeOf<UnitOfWorkFactory<DefaultDbContext>>();
        
        await Assert.That(defaultUnitOfWork).IsNotNull()
            .And.IsTypeOf<UnitOfWork<DefaultDbContext>>();
        
        await Assert.That(otherUnitOfWorkFactory).IsNotNull()
            .And.IsTypeOf<UnitOfWorkFactory<OtherDbContext>>();
        
        await Assert.That(otherUnitOfWork).IsNotNull()
            .And.IsTypeOf<UnitOfWork<OtherDbContext>>();
        
    }

    public class DefaultDbContext : DbContext {
        public DefaultDbContext() : base() {}
        public DefaultDbContext(DbContextOptions<DefaultDbContext> options) : base(options) {}
    }

    public class OtherDbContext : DbContext {
        public OtherDbContext() : base() {}
        public OtherDbContext(DbContextOptions<OtherDbContext> options) : base(options) {}
    }
}
