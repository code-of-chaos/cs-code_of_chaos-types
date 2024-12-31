// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Concurrent;

namespace CodeOfChaos.Types;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents a collection of seeders utilized to group related seeders together
///     for execution and organization purposes.
/// </summary>
/// <remarks>
///     The SeederGroup enables operations to manage and ensure that seeders are added uniquely.
///     It checks for duplications before adding new seeder types.
/// </remarks>
public readonly struct SeederGroup() {
    /// <summary>
    ///     Represents the collection of seeder types to be utilized within a SeederGroup.
    ///     This variable is intended to hold a concurrent queue of seeder types
    ///     that can be added and managed dynamically for data seeding purposes.
    /// </summary>
    public readonly ConcurrentQueue<Type> SeederTypes = [];
    /// <summary>
    ///     Gets a value indicating whether the <see cref="SeederGroup" /> contains no seeder types.
    /// </summary>
    /// <remarks>
    ///     This property checks if the underlying queue of seeder types is empty and returns true if no seeder types are
    ///     present; otherwise, false.
    /// </remarks>
    /// <value>
    ///     A boolean value: true if no seeder types are registered in the group; otherwise, false.
    /// </value>
    public bool IsEmpty => SeederTypes.IsEmpty;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Adds a seeder type to the seeder group if it has not already been added.
    ///     This allows for organizing and executing the data seeding process by
    ///     maintaining a collection of seeder types.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the seeder to add. The type must implement the <see cref="ISeeder" /> interface.
    /// </typeparam>
    /// <returns>
    ///     A new instance of <see cref="SeederGroup" /> that includes the added seeder type.
    /// </returns>
    public SeederGroup AddSeeder<T>() where T : ISeeder => AddSeeder(typeof(T));

    /// <summary>
    ///     Adds a new seeder type to the group.
    /// </summary>
    /// <param name="seeder">The type of the seeder to be added. Must implement the <see cref="ISeeder" /> interface.</param>
    /// <returns>A new instance of <see cref="SeederGroup" /> containing the added seeder type.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the specified seeder type has already been added to the group.
    /// </exception>
    public SeederGroup AddSeeder(Type seeder) {
        if (SeederTypes.Any(existing => existing == seeder)) {
            throw new InvalidOperationException($"Seeder instance '{seeder}' has already been added.");
        }

        SeederTypes.Enqueue(seeder);
        return this;
    }
}
