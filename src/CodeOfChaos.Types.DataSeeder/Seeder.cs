// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.Logging;

namespace CodeOfChaos.Types;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public abstract class Seeder : ISeeder {
    public async Task StartAsync(ILogger logger, CancellationToken ct = default) {
        if (!await ShouldSeedAsync(ct)) {
            logger.LogInformation("Skipping seeding");
            return;
        }
        ct.ThrowIfCancellationRequested();
        await SeedAsync(ct);
    }

    public virtual Task<bool> ShouldSeedAsync(CancellationToken ct = default) => Task.FromResult(true);
    public abstract Task SeedAsync(CancellationToken ct = default);
}
