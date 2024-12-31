// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.Types;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class ServiceCollectionExtensions {
    public static IServiceCollection AddOneTimeDataSeeder<TDataSeeder>(this IServiceCollection services)
        where TDataSeeder : class, IDataSeederService, IHostedService
        => services.AddHostedService<TDataSeeder>();

    public static IServiceCollection AddOneTimeDataSeeder<TDataSeeder>(this IServiceCollection services, Func<IServiceProvider, TDataSeeder> implementationFactory)
        where TDataSeeder : class, IDataSeederService, IHostedService
        => services.AddHostedService(implementationFactory);

    public static IServiceCollection AddOneTimeDataSeeder<TDataSeeder>(this IServiceCollection services, Action<TDataSeeder> configure)
        where TDataSeeder : class, IDataSeederService, IHostedService {
        
        services.TryAddEnumerable(
            ServiceDescriptor.Singleton<IHostedService, TDataSeeder>(provider
                => {
                    var seeder = ActivatorUtilities.CreateInstance<TDataSeeder>(provider);
                    configure(seeder);
                    return seeder;
                }
            )
        );
        return services;
    }

    public static IServiceCollection AddOneTimeDataSeeder(this IServiceCollection services, Action<OneTimeDataSeederService> configure)
        => AddOneTimeDataSeeder<OneTimeDataSeederService>(services, configure);
}
