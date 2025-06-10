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
public class UnitOfWork<TDbContext>(IDbContextFactory<TDbContext> dbContextFactory, IServiceScope serviceScope) : IUnitOfWork where TDbContext : DbContext {
    private TDbContext? _dbContext;
    private IDbContextTransaction? _transaction;
    private readonly ConcurrentDictionary<Type, IUnitOfWorkRepository> AttachedRepositories = [];
    private readonly SemaphoreSlim _initLock = new(1, 1);


    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    protected virtual async ValueTask<TDbContext> GetDbContextAsync(CancellationToken ct) {
        if (_dbContext != null) return _dbContext;

        await _initLock.WaitAsync(ct);
        try {
            // Double-check pattern
            if (_dbContext != null) return _dbContext;

            _dbContext = await dbContextFactory.CreateDbContextAsync(ct);
            return _dbContext;
        }
        finally {
            _initLock.Release();
        }
    }
    
    public virtual async ValueTask<bool> TryCreateTransactionAsync(CancellationToken ct = default) {
        if (_transaction != null) return false;

        TDbContext dbContext = await GetDbContextAsync(ct);
        if (dbContext.Database.CurrentTransaction != null) {
            _transaction = dbContext.Database.CurrentTransaction;
            return true;
        }

        await _initLock.WaitAsync(ct);
        try {
            _transaction = await dbContext.Database.BeginTransactionAsync(ct);
            return true;
        }
        finally {
            _initLock.Release();
        }
    }
    
    public virtual async ValueTask SaveChangesAsync(CancellationToken ct = default) {
        DbContext dbContext = await GetDbContextAsync(ct);
        await dbContext.SaveChangesAsync(ct);
    }

    public virtual async ValueTask<bool> TryCommitTransactionAsync(CancellationToken ct = default) {
        if (_transaction == null) return false;

        await _initLock.WaitAsync(ct);
        try {
            await _transaction.CommitAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
            return true;
        }
        finally {
            _initLock.Release();
        }
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

        TDbContext dbContext = await GetDbContextAsync(ct);
        return dbContext as T ?? throw new InvalidCastException($"Cannot cast DbContext of type '{dbContext.GetType()}' to '{typeof(T)}'");
    }

    public virtual async ValueTask<TRepo> GetRepositoryAsync<TRepo>(CancellationToken ct = default) where TRepo : class, IUnitOfWorkRepository {
        if (AttachedRepositories.TryGetValue(typeof(TRepo), out IUnitOfWorkRepository? cachedRepo) && cachedRepo is TRepo castedCachedRepo) return castedCachedRepo;

        // Cache miss so we create a new instance
        var repo = await CreateAndAttachRepositoryAsync<TRepo>(ct);

        AttachedRepositories.AddOrUpdate(typeof(TRepo), repo);
        return repo;
    }

    private async ValueTask<TRepo> CreateAndAttachRepositoryAsync<TRepo>(CancellationToken ct = default) where TRepo : class, IUnitOfWorkRepository {
        var repo = serviceScope.ServiceProvider.GetRequiredService<TRepo>();
        if (repo is not UnitOfWorkRepository<TDbContext> castedRepo) throw new InvalidCastException($"Cannot cast repository of type '{repo.GetType()}' to '{typeof(TRepo)}'");

        await castedRepo.AttachAsync(this, ct);
        return repo;
    }

    public virtual async ValueTask DisposeAsync() {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (_initLock == null) {
            GC.SuppressFinalize(this);
            return;
        }

        await _initLock.WaitAsync();
        try {
            if (_transaction != null) {
                await TryRollbackTransactionAsync();
            }

            foreach (IUnitOfWorkRepository repository in AttachedRepositories.Values) {
                if (repository is UnitOfWorkRepository<TDbContext> castedRepo) {
                    castedRepo.Detach();
                }
            }

            AttachedRepositories.Clear();

            if (_dbContext != null) {
                await _dbContext.DisposeAsync();
                _dbContext = null;
            }

            serviceScope.Dispose();
        }
        finally {
            _initLock.Release();
            _initLock.Dispose();
            GC.SuppressFinalize(this);
        }
    }

}
