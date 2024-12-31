// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.Logging;

namespace CodeOfChaos.Types;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Defines a contract for a seeding mechanism that determines whether a seed process should run
///     and executes the seeding logic if required.
/// </summary>
public interface ISeeder {
    /// <summary>
    ///     Initiates the seeding process for the implementing class.
    /// </summary>
    /// <param name="logger">An instance of <see cref="ILogger" /> for logging operations.</param>
    /// <param name="ct">
    ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete. Defaults to
    ///     <see cref="CancellationToken.None" />.
    /// </param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous operation.</returns>
    /// <remarks>
    ///     This method should first checks whether seeding should occur by calling <c>ShouldSeedAsync</c>. If seeding is not
    ///     needed, it logs an informational message and returns.
    ///     If seeding is required, the method executes <c>SeedAsync</c>, respecting the provided cancellation token.
    /// </remarks>
    Task StartAsync(ILogger logger, CancellationToken ct = default);

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
