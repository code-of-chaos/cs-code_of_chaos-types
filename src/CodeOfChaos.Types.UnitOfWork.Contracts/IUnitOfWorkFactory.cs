// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace CodeOfChaos.Types.UnitOfWork;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IUnitOfWorkFactory<TDbContext> where TDbContext : DbContext {
    IUnitOfWork<TDbContext> Create();
    
    IUnitOfWork<TDbContext> CreateWithTransaction();
    ValueTask<IUnitOfWork<TDbContext>> CreateWithTransactionAsync(CancellationToken ct = default);
    
    bool TryCreateWithTransaction([NotNullWhen(true)] out IUnitOfWork<TDbContext>? unitOfWork);
    ValueTask<IUnitOfWork<TDbContext>?> TryCreateWithTransactionAsync(CancellationToken ct = default);
}
