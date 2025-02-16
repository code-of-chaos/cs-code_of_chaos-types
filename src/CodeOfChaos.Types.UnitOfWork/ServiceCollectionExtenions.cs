// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CodeOfChaos.Types.UnitOfWork;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class ServiceCollectionExtenions {
    public static IServiceCollection AddUnitOfWork<TDbContext>(this IServiceCollection services) where TDbContext : DbContext {
        services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory<TDbContext>>();
        services.AddScoped<IUnitOfWork>(static sp => sp.GetRequiredService<IUnitOfWorkFactory>().Create());

        return services;
    }

    public static IServiceCollection AddUnitOfWork<TDbContext>(this IServiceCollection services, string key) where TDbContext : DbContext {
        services.AddKeyedScoped<IUnitOfWorkFactory, UnitOfWorkFactory<TDbContext>>(key);
        services.AddKeyedScoped<IUnitOfWork>(key, implementationFactory: static (sp, k) => sp.GetRequiredKeyedService<IUnitOfWorkFactory>(k).Create());

        return services;
    }
}
