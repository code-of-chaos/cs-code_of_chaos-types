// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using System.Collections.Concurrent;

namespace CodeOfChaos.Types;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public readonly struct SeederGroup(IServiceProvider serviceProvider) : IEnumerable<ISeeder> {
    private readonly ConcurrentQueue<ISeeder> _seeders = [];
    public bool IsEmpty => _seeders.IsEmpty;
    public int Count => _seeders.Count;

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    public SeederGroup(ISeeder[] seeders, IServiceProvider serviceProvider) : this(serviceProvider) {
        foreach (ISeeder seeder in seeders) _seeders.Enqueue(seeder);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public SeederGroup AddSeeder<T>(Func<IServiceProvider, T> seederFactory) where T : ISeeder
        => AddSeeder(seederFactory(serviceProvider));

    public SeederGroup AddSeeder<T>(Func<T> seederFactory) where T : ISeeder
        => AddSeeder(seederFactory());

    public SeederGroup AddSeeder<T>() where T : ISeeder
        => AddSeeder(serviceProvider.GetRequiredService<T>());

    public SeederGroup AddSeeder(Type type)
        => AddSeeder(ActivatorUtilities.CreateInstance<ISeeder>(serviceProvider, type));

    public SeederGroup AddSeeder(ISeeder seeder) {
        if (_seeders.Any(existing => existing.Equals(seeder))) {
            throw new InvalidOperationException($"Seeder instance '{seeder}' has already been added.");
        }

        _seeders.Enqueue(seeder);
        return this;
    }

    public IEnumerator<ISeeder> GetEnumerator() => _seeders.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
