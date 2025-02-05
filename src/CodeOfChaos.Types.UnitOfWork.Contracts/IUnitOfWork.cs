// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.EntityFrameworkCore;

namespace CodeOfChaos.Types.UnitOfWork;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IUnitOfWork : IAsyncDisposable {
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // ----------------------------------------------------------------------------------------------------------------
    ValueTask SaveChangesAsync(CancellationToken ct = default);
    ValueTask<bool> TryCommitTransactionAsync(CancellationToken ct = default);
    ValueTask<bool> TryCreateTransactionAsync(CancellationToken ct = default);
    ValueTask<bool> TryRollbackTransactionAsync(CancellationToken ct = default);
    ValueTask<bool> TryRollbackToSavepointAsync(Guid id, CancellationToken ct = default);
    ValueTask<bool> TryCreateSavepointAsync(Guid id, CancellationToken ct = default);
    
    ValueTask<TDbContext> GetDbContextAsync<TDbContext>(CancellationToken ct = default) where TDbContext : DbContext;

    TRepo GetRepository<TRepo>() where TRepo : class, ICanAttachToUnitOfWork;
}
