// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CodeOfChaos.Types.UnitOfWork;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ReadonlyUnitOfWork<TDbContext>(
    IDbContextFactory<TDbContext> dbContextFactory,
    IServiceScope serviceScope
) : UnitOfWork<TDbContext>(dbContextFactory, serviceScope), IReadonlyUnitOfWork
    where TDbContext : DbContext, IReadonlyCapableDbContext {
    
    protected async override ValueTask<TDbContext> GetDbContextAsync(CancellationToken ct) {
        TDbContext dbContext = await base.GetDbContextAsync(ct);
        dbContext.SetAsReadonly();
        return dbContext;
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public override ValueTask SaveChangesAsync(CancellationToken ct = default) 
        => throw new NotSupportedException("ReadonlyUnitOfWork cannot save changes.");

    public override ValueTask<bool> TryCommitTransactionAsync(CancellationToken ct = default) 
        => throw new NotSupportedException("ReadonlyUnitOfWork cannot commit transactions.");

    public override ValueTask<bool> TryCreateTransactionAsync(CancellationToken ct = default) 
        => throw new NotSupportedException("ReadonlyUnitOfWork cannot create transactions.");

    public override ValueTask<bool> TryRollbackTransactionAsync(CancellationToken ct = default) 
        => throw new NotSupportedException("ReadonlyUnitOfWork cannot rollback transactions.");

    public override ValueTask<bool> TryRollbackToSavepointAsync(Guid id, CancellationToken ct = default) 
        => throw new NotSupportedException("ReadonlyUnitOfWork cannot rollback to savepoints.");

    public override ValueTask<bool> TryCreateSavepointAsync(Guid id, CancellationToken ct = default) 
        => throw new NotSupportedException("ReadonlyUnitOfWork cannot create savepoints.");
}
