// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CodeOfChaos.Types;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents an abstract implementation of the <see cref="ISeeder" /> interface, providing
///     a base class for seeding operations with pre-seeding validation logic.
/// </summary>
public abstract class Seeder : ISeeder {
    public bool ShouldSeed { get; private set; } = false;
    
    /// <inheritdoc />
    public async Task StartAsync(IServiceProvider serviceProvider, CancellationToken ct = default) {
        ShouldSeed = await ShouldSeedAsync(ct);
        if (!ShouldSeed) {
            var logger = serviceProvider.GetService<ILogger>();
            logger?.LogInformation("Skipping seeding");
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
