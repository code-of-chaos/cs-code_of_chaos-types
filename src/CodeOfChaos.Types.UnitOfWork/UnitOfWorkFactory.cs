// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CodeOfChaos.Types.UnitOfWork;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class UnitOfWorkFactory<TDbContext>(IDbContextFactory<TDbContext> dbContextFactory, IServiceProvider provider, ILogger<UnitOfWorkFactory<TDbContext>> logger) : IUnitOfWorkFactory where TDbContext : DbContext {
    public IUnitOfWork Create() {
        // Each unit of work should have their own scope which they pull their repositories from
        //      This, if the factory is used correctly, should enforce correct usage and limit dbcontext concurrency issues.
        IServiceScope scope = provider.CreateScope();

        // Because our factory doesn't create the actual dbcontext, yet we are safe, and we can just inject it downwards.
        return new UnitOfWork<TDbContext>(dbContextFactory, scope);
    }
    
    public async ValueTask<IUnitOfWork> CreateWithTransactionAsync(CancellationToken ct = default) {
        IUnitOfWork unitOfWork = Create();

        // ReSharper disable once InvertIf
        if (!await unitOfWork.TryCreateTransactionAsync(ct)) {
            logger.LogError("Failed to create transaction for new unit of work.");
            throw new Exception("Failed to create transaction");
        }

        return unitOfWork;
    }

    public async ValueTask<IUnitOfWork?> TryCreateWithTransactionAsync(CancellationToken ct = default) {
        IUnitOfWork unitOfWork = Create();

        // ReSharper disable once InvertIf
        if (!await unitOfWork.TryCreateTransactionAsync(ct)) {
            logger.LogError("Failed to create transaction for new unit of work.");
            return null;
        }

        return unitOfWork;
    }
}
