// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CodeOfChaos.Types.UnitOfWork;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class ServiceCollectionExtensions {
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

    public static IServiceCollection AddReadonlyUnitOfWork<TDbContext>(this IServiceCollection services) where TDbContext : DbContext, IReadonlyCapableDbContext {
        services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory<TDbContext>>();
        services.AddScoped<IUnitOfWork>(static sp => sp.GetRequiredService<IUnitOfWorkFactory>().Create());
        
        services.AddScoped<IReadonlyUnitOfWorkFactory, ReadonlyUnitOfWorkFactory<TDbContext>>();
        services.AddScoped<IReadonlyUnitOfWork>(static sp => sp.GetRequiredService<IReadonlyUnitOfWorkFactory>().Create());
        
        return services;
    }

    public static IServiceCollection AddReadonlyUnitOfWork<TDbContext>(this IServiceCollection services, string key) where TDbContext : DbContext, IReadonlyCapableDbContext {
        services.AddKeyedScoped<IUnitOfWorkFactory, UnitOfWorkFactory<TDbContext>>(key);
        services.AddKeyedScoped<IUnitOfWork>(key, implementationFactory: static (sp, k) => sp.GetRequiredKeyedService<IUnitOfWorkFactory>(k).Create());
        
        services.AddKeyedScoped<IReadonlyUnitOfWorkFactory, ReadonlyUnitOfWorkFactory<TDbContext>>(key);
        services.AddKeyedScoped<IReadonlyUnitOfWork>(key, implementationFactory: static (sp, k) => sp.GetRequiredKeyedService<IReadonlyUnitOfWorkFactory>(k).Create());
        
        return services;
    }
}
