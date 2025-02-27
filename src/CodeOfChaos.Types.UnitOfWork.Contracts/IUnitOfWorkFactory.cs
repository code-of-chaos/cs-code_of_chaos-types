// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace CodeOfChaos.Types.UnitOfWork;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IUnitOfWorkFactory {
    IUnitOfWork Create();
    ValueTask<IUnitOfWork> CreateWithTransactionAsync(CancellationToken ct = default);
    ValueTask<IUnitOfWork?> TryCreateWithTransactionAsync(CancellationToken ct = default);
}
