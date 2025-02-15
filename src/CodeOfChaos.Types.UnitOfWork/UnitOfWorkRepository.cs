// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.EntityFrameworkCore;

namespace CodeOfChaos.Types.UnitOfWork;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public abstract class UnitOfWorkRepository<TDbContext> : IUnitOfWorkRepository 
    where TDbContext : DbContext
{
    private TDbContext? DbContext { get; set; }
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    internal async ValueTask AttachAsync(IUnitOfWork unitOfWork, CancellationToken ct = default) => DbContext = await unitOfWork.GetDbContextAsync<TDbContext>(ct);
    internal void Detach() => DbContext = null;// Remove the reference to the DbContext

    protected TDbContext GetDbContext() => DbContext ?? throw new InvalidOperationException("Repository is not attached to a UnitOfWork.");
    protected DbSet<TModel> GetDbSet<TModel>() where TModel : class => DbContext?.Set<TModel>() ?? throw new InvalidOperationException("Repository is not attached to a UnitOfWork.");
}
