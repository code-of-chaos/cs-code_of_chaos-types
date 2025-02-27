// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.Types.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Tests.CodeOfChaos.Types.UnitOfWork.Assets;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class MockDbContext : DbContext, IReadonlyCapableDbContext {
    public bool IsReadonly { get; private set; }
    public void SetAsReadonly() => IsReadonly = true;
    
    public MockDbContext() {}
    public MockDbContext(DbContextOptions<MockDbContext> options) : base(options) {}
}
