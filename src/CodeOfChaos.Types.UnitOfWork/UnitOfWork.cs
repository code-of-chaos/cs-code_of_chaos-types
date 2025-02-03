// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace CodeOfChaos.Types.UnitOfWork;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class UnitOfWork<TDbContext>(IDbContextFactory<TDbContext> dbContextFactory, IServiceScope serviceScope) : IUnitOfWork where TDbContext : DbContext{
    private readonly AsyncLazy<TDbContext> _db = new(async ct => await dbContextFactory.CreateDbContextAsync(ct));
    private IDbContextTransaction? _transaction;
    private ConcurrentDictionary<Type, IRepository> AttachedRepositories { get; } = [];
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public virtual async ValueTask SaveChangesAsync(CancellationToken ct = default) {
        DbContext dbContext = await _db.GetValueAsync(ct);
        await dbContext.SaveChangesAsync(ct);
    }

    public virtual async ValueTask<bool> TryCommitTransactionAsync(CancellationToken ct = default) {
        if (_transaction == null) return false;

        await _transaction.CommitAsync(ct);
        _transaction.Dispose();
        _transaction = null;

        return true;
    }

    public virtual async ValueTask<bool> TryCreateTransactionAsync(CancellationToken ct = default) {
        if (_transaction != null) return false;
        
        TDbContext dbContext = await _db.GetValueAsync(ct);
        if (dbContext.Database.CurrentTransaction != null) {
            // Something went wrong during saving before and the transaction wasn't set by the unit of work
            _transaction = dbContext.Database.CurrentTransaction;
            return true;
        }
        
        _transaction = await dbContext.Database.BeginTransactionAsync(ct);
        
        return true;
    }

    public virtual async ValueTask<bool> TryRollbackTransactionAsync(CancellationToken ct = default) {
        if (_transaction == null) return false;

        await _transaction.RollbackAsync(ct);
        await _transaction.DisposeAsync();
        _transaction = null;

        return true;
    }

    public virtual async ValueTask<bool> TryRollbackToSavepointAsync(Guid id, CancellationToken ct = default) {
        if (_transaction == null) return false;
        if (!_transaction.SupportsSavepoints) return false;

        await _transaction.RollbackToSavepointAsync(id.ToString("N"), ct);
        
        return true;
    }

    public virtual async ValueTask<bool> TryCreateSavepointAsync(Guid id, CancellationToken ct = default) {
        if (_transaction == null) return false;
        if (!_transaction.SupportsSavepoints) return false;

        await _transaction.CreateSavepointAsync(id.ToString("N"), ct);
        
        return true;
    }

    public virtual async ValueTask<T> GetDbContextAsync<T>(CancellationToken ct = default) where T : DbContext {
        if (typeof(T) != typeof(TDbContext)) throw new NotSupportedException($"DbContext type '{typeof(T)}' is not supported by this UnitOfWork.");

        TDbContext dbContext = await _db.GetValueAsync(ct);
        return dbContext as T ?? throw new InvalidCastException($"Cannot cast DbContext of type '{dbContext.GetType()}' to '{typeof(T)}'");
    }

    public virtual TRepo GetRepository<TRepo>() where TRepo : class, IRepository {
        if (AttachedRepositories.TryGetValue(typeof(TRepo), out IRepository? cachedRepo) && cachedRepo is TRepo castedCachedRepo) return castedCachedRepo;
        
        // Cache miss so we create a new instance
        var repo = serviceScope.ServiceProvider.GetRequiredService<TRepo>();
        
        repo.Attach(this);
        AttachedRepositories.AddOrUpdate(typeof(TRepo), repo); 
        return repo;
    }

    public virtual async ValueTask DisposeAsync() {
        if (_transaction != null) await TryRollbackTransactionAsync();

        if (!AttachedRepositories.IsEmpty) {
            // First detach all references to this unit of work
            foreach ((_, IRepository repo) in AttachedRepositories) {
                repo.Detach(this);
            }
            
            // Then clear our own reference to them
            AttachedRepositories.Clear();
        }
        
        serviceScope.Dispose();
        
        await _db.DisposeAsync();
            
        GC.SuppressFinalize(this);
    }
}
