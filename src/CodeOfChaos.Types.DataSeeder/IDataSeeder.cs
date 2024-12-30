// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace CodeOfChaos.Types;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IDataSeederService : IHostedService {
    IDataSeederService AddSeeder<TSeeder>() where TSeeder : ISeeder;
    IDataSeederService AddSeederGroup(params ISeeder[] seeders);
    IDataSeederService AddSeederGroup(Action<SeederGroup> group);
    IDataSeederService AddSeederGroup(SeederGroup group);
    
    void AddRemainderSeeders(Assembly assembly);
    void AddRemainderSeedersAsOneGroup(Assembly assembly);
}
