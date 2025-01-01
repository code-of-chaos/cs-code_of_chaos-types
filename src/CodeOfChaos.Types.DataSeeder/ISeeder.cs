// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace CodeOfChaos.Types;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Defines a contract for a seeding mechanism that determines whether a seed process should run
///     and executes the seeding logic if required.
/// </summary>
public interface ISeeder {
    bool ShouldSeed { get; }
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Initiates the seeding process for the implementing class.
    /// </summary>
    /// <param name="serviceProvider">
    /// An instance of <see cref="IServiceProvider" /> used to resolve dependencies during the seeding process.
    /// </param>
    /// <param name="ct">
    /// A <see cref="CancellationToken" /> to observe while waiting for the task to complete. Defaults to
    /// <see cref="CancellationToken.None" />.
    /// </param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous operation.</returns>
    /// <remarks>
    /// This method orchestrates the seeding process, ensuring any required dependencies are resolved via the provided
    /// <c>serviceProvider</c>. If the implementation detects that seeding is unnecessary,
    /// it will gracefully conclude the operation. If any steps in the seeding process require cancellation,
    /// they will respect the provided <c>CancellationToken</c>.
    /// </remarks>
    Task StartAsync(IServiceProvider serviceProvider, CancellationToken ct = default);

    /// Determines whether the seeding process should proceed or be skipped.
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <return>
    ///     A task representing the asynchronous operation. The task result contains a boolean indicating
    ///     whether the seeding process should proceed (true) or be skipped (false).
    /// </return>
    Task<bool> ShouldSeedAsync(CancellationToken ct = default);

    /// <summary>
    ///     Executes the seed operation for a data seeder. This method is intended to be overridden
    ///     by derived classes to implement data seeding logic.
    /// </summary>
    /// <param name="ct">The <see cref="CancellationToken" /> that can be used to signal a request to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SeedAsync(CancellationToken ct = default);
}
