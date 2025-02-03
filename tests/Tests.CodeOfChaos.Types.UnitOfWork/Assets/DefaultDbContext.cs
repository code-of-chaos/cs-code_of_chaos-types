// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.EntityFrameworkCore;

namespace Tests.CodeOfChaos.Types.UnitOfWork.Assets;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class DefaultDbContext : DbContext {
    public DefaultDbContext() : base() {}
    public DefaultDbContext(DbContextOptions<DefaultDbContext> options) : base(options) {}
}
