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
    /// <summary>
    ///     Adds a one-time data seeder of type <typeparamref name="TDataSeeder" /> to the application's service collection.
    /// </summary>
    /// <typeparam name="TDataSeeder">
    ///     The type of the data seeder to be added. Must implement both <see cref="IDataSeederService" /> and
    ///     <see cref="IHostedService" />.
    /// </typeparam>
    /// <param name="services">
    ///     The <see cref="IServiceCollection" /> instance to which the data seeder will be added.
    /// </param>
    /// <returns>
    ///     The updated <see cref="IServiceCollection" /> with the registered one-time data seeder service.
    /// </returns>
    public static IServiceCollection AddOneTimeDataSeeder<TDataSeeder>(this IServiceCollection services)
        where TDataSeeder : class, IDataSeederService, IHostedService
        => services.AddHostedService<TDataSeeder>();

    /// <summary>
    ///     Registers a one-time data seeder of type <typeparamref name="TDataSeeder" /> in the service collection using a
    ///     factory function.
    /// </summary>
    /// <typeparam name="TDataSeeder">
    ///     The type of the data seeder to register. Must implement both <see cref="IDataSeederService" /> and
    ///     <see cref="IHostedService" />.
    /// </typeparam>
    /// <param name="services">
    ///     The <see cref="IServiceCollection" /> instance to which the data seeder will be added.
    /// </param>
    /// <param name="implementationFactory">
    ///     A factory function to create an instance of the data seeder.
    /// </param>
    /// <returns>
    ///     The updated <see cref="IServiceCollection" /> with the registered data seeder service.
    /// </returns>
    public static IServiceCollection AddOneTimeDataSeeder<TDataSeeder>(this IServiceCollection services, Func<IServiceProvider, TDataSeeder> implementationFactory)
        where TDataSeeder : class, IDataSeederService, IHostedService
        => services.AddHostedService(implementationFactory);


    /// <summary>
    ///     Adds a one-time data seeder service with a custom configuration to the application's service collection.
    /// </summary>
    /// <param name="services">
    ///     The <see cref="IServiceCollection" /> instance to which the data seeder will be added.
    /// </param>
    /// <param name="configureSeeder">
    ///     An <see cref="Action{T}" /> delegate to configure the <see cref="OneTimeDataSeederService" /> or a custom seeder
    ///     service.
    /// </param>
    /// <returns>
    ///     The updated <see cref="IServiceCollection" /> with the registered one-time data seeder service.
    /// </returns>
    public static IServiceCollection AddOneTimeDataSeeder(this IServiceCollection services, Action<OneTimeDataSeederService> configureSeeder)
        => AddOneTimeDataSeeder<OneTimeDataSeederService>(services, configureSeeder);

    /// <summary>
    ///     Adds a one-time data seeder to the service collection with the ability to configure it via a provided configuration
    ///     action.
    /// </summary>
    /// <typeparam name="TDataSeeder">
    ///     The type of the data seeder to be added, which must implement
    ///     <see cref="IDataSeederService" /> and <see cref="IHostedService" />.
    /// </typeparam>
    /// <param name="services">The service collection to which the data seeder is added.</param>
    /// <param name="configureSeeder">An action to configure the data seeder when it is created.</param>
    /// <returns>The updated service collection with the data seeder registered.</returns>
    public static IServiceCollection AddOneTimeDataSeeder<TDataSeeder>(this IServiceCollection services, Action<TDataSeeder> configureSeeder)
        where TDataSeeder : class, IDataSeederService, IHostedService {

        services.TryAddEnumerable(
            ServiceDescriptor.Singleton<IHostedService, TDataSeeder>(provider
                    => {
                    var seeder = ActivatorUtilities.CreateInstance<TDataSeeder>(provider);
                    configureSeeder(seeder);
                    return seeder;
                }
            )
        );
        return services;
    }
}
