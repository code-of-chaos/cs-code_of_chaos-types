// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;

namespace CodeOfChaos.Types.UnitOfWork;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IUnitOfWorkFactory {
    IUnitOfWork Create();
    
    IUnitOfWork CreateWithTransaction();
    ValueTask<IUnitOfWork> CreateWithTransactionAsync(CancellationToken ct = default);
    
    bool TryCreateWithTransaction([NotNullWhen(true)] out IUnitOfWork? unitOfWork);
    ValueTask<IUnitOfWork?> TryCreateWithTransactionAsync(CancellationToken ct = default);
}
