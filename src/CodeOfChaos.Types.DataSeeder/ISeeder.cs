// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.Logging;

namespace CodeOfChaos.Types;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface ISeeder {
    Task StartAsync(ILogger logger, CancellationToken ct = default);
    Task<bool> ShouldSeedAsync(CancellationToken ct = default);
    Task SeedAsync(CancellationToken ct = default);
}
