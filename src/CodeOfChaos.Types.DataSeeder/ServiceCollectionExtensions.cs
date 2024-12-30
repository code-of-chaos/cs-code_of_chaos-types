// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.Types;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class ServiceCollectionExtensions {
    public static IServiceCollection AddOneTimeDataSeeder<TDataSeeder>(this IServiceCollection services)
        where TDataSeeder : class, IDataSeederService 
        => services.AddHostedService<TDataSeeder>();
    
    public static IServiceCollection AddOneTimeDataSeeder<TDataSeeder>(this IServiceCollection services,  Func<IServiceProvider, TDataSeeder> implementationFactory) 
        where TDataSeeder : class, IDataSeederService 
        => services.AddHostedService(implementationFactory);
}
