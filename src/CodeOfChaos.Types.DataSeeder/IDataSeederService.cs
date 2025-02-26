// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace CodeOfChaos.Types;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IDataSeederService : IHostedService {
    /// <summary>
    ///     Adds a seeder of type TSeeder to the data seeder service.
    ///     TSeeder must implement the ISeeder interface.
    /// </summary>
    /// <typeparam name="TSeeder">The type of the seeder to be added.</typeparam>
    /// <returns>An instance of IDataSeederService to allow method chaining.</returns>
    IDataSeederService AddSeeder<TSeeder>() where TSeeder : ISeeder;

    /// <summary>
    ///     Adds a group of seeders to the seeder service using the specified <paramref name="group" /> configuration callback.
    /// </summary>
    /// <param name="group">
    ///     An action that allows configuration of the <see cref="SeederGroup" /> by adding individual seeders.
    /// </param>
    /// <returns>
    ///     The updated instance of <see cref="IDataSeederService" /> to allow method chaining.
    /// </returns>
    IDataSeederService AddSeederGroup(Action<SeederGroup> group);

    /// <summary>
    ///     Adds a group of seeders to the current seeder service.
    /// </summary>
    /// <param name="group">
    ///     An instance of <see cref="SeederGroup" /> containing the seeders to be added.
    /// </param>
    /// <returns>
    ///     The current instance of <see cref="IDataSeederService" /> with the added seeder group.
    /// </returns>
    IDataSeederService AddSeederGroup(SeederGroup group);

    /// <summary>
    ///     Adds remaining seeder types from the provided assembly to the data seeder service.
    ///     Ensures that only non-abstract, non-generic, and non-interface types implementing the <see cref="ISeeder" />
    ///     interface are added.
    /// </summary>
    /// <param name="assembly">
    ///     The assembly from which seeder types will be collected.
    /// </param>
    /// <exception cref="AggregateException">
    ///     Thrown when one or more seeder types fail to be instantiated.
    /// </exception>
    void AddRemainderSeeders(Assembly assembly);

    /// <summary>
    ///     Adds all unprocessed data seeder types from the provided assembly as a single group to be executed together.
    /// </summary>
    /// <param name="assembly">
    ///     The assembly from which to collect remaining data seeder types.
    /// </param>
    /// <exception cref="AggregateException">
    ///     Thrown if there are multiple errors while adding seeders to the group.
    /// </exception>
    void AddRemainderSeedersAsOneGroup(Assembly assembly);
}
