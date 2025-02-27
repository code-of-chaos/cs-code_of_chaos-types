// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.Types.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Tests.CodeOfChaos.Types.UnitOfWork.Assets;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class DefaultDbContext : DbContext, IReadonlyCapableDbContext {
    public bool IsReadonly { get; private set; }
    public void SetAsReadonly() => IsReadonly = true;
    public DefaultDbContext() {}
    public DefaultDbContext(DbContextOptions<DefaultDbContext> options) : base(options) {}
}
