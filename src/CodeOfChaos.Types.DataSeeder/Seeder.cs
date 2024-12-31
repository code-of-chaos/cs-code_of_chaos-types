// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.Logging;

namespace CodeOfChaos.Types;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
/// Represents an abstract implementation of the <see cref="ISeeder"/> interface, providing
/// a base class for seeding operations with pre-seeding validation logic.
/// </summary>
public abstract class Seeder : ISeeder {
    /// <inheritdoc />
    public async Task StartAsync(ILogger logger, CancellationToken ct = default) {
        if (!await ShouldSeedAsync(ct)) {
            logger.LogInformation("Skipping seeding");
            return;
        }

        ct.ThrowIfCancellationRequested();
        await SeedAsync(ct);
    }

    /// <inheritdoc />
    public virtual Task<bool> ShouldSeedAsync(CancellationToken ct = default) => Task.FromResult(true);
    
    /// <inheritdoc />
    public abstract Task SeedAsync(CancellationToken ct = default);
}
