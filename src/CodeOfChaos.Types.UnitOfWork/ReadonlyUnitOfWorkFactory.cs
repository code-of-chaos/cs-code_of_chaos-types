// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CodeOfChaos.Types.UnitOfWork;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ReadonlyUnitOfWorkFactory<TDbContext>(
    IDbContextFactory<TDbContext> dbContextFactory,
    IServiceProvider provider
) : IReadonlyUnitOfWorkFactory where TDbContext : DbContext, IReadonlyCapableDbContext {

    public IReadonlyUnitOfWork Create() {
        AsyncServiceScope scope = provider.CreateAsyncScope();
        return new ReadonlyUnitOfWork<TDbContext>(dbContextFactory, scope);
    }
}
