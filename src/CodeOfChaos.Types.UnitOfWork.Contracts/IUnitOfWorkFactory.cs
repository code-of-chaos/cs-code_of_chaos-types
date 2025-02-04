// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.Extensions.DependencyInjection;

namespace CodeOfChaos.Types.UnitOfWork;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IUnitOfWorkFactory : IFactoryService<IUnitOfWork> {
    ValueTask<IUnitOfWork> CreateWithTransactionAsync(CancellationToken ct = default);
    ValueTask<IUnitOfWork?> TryCreateWithTransactionAsync(CancellationToken ct = default);
}
