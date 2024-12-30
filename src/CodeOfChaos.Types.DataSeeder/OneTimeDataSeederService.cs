// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Reflection;

namespace CodeOfChaos.Types;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public abstract class OneTimeDataSeederService(IServiceProvider serviceProvider, ILogger<OneTimeDataSeederService> logger) : IDataSeederService {
    private readonly ConcurrentQueue<SeederGroup> _seeders = [];
    private readonly ConcurrentBag<Type> _seederTypes = [];
    private bool _collectedRemainders;// If set to true, the remainder seeders have been collected and thus should throw an exception if any are added

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public async Task StartAsync(CancellationToken ct = default) {
        logger.LogInformation("DataSeederService starting...");

        // User has to collect the seeders
        await CollectAsync(ct);
        ct.ThrowIfCancellationRequested();// Don't throw during collection, but throw afterward

        // Validation has to succeed before we continue
        //      Technically this library doesn't need to validate much,
        //      but to make sure we've enabled overloading of the method
        if (!ValidateSeeders()) return;

        int i = 0;
        foreach (SeederGroup seederGroup in _seeders) {
            if (ct.IsCancellationRequested) {
                logger.LogWarning("Seeding process canceled during execution.");
                break;
            }

            if (seederGroup.IsEmpty) {
                logger.LogDebug("ExecutionStep {step} : Skipping empty seeder array", i++);
                continue;
            }

            logger.LogDebug("ExecutionStep {step} : {count} Seeder(s) found, executing...", i++, seederGroup.Count);
            await Task.WhenAll(seederGroup.Select(seeder => seeder.StartAsync(logger, ct)));
        }

        logger.LogInformation("All seeders completed in {i} steps", i);
        // Cleanup
        _seeders.Clear();
        _seederTypes.Clear();
        _collectedRemainders = false;
    }

    public Task StopAsync(CancellationToken ct = default) {
        logger.LogInformation("Stopping DataSeederService...");
        return Task.CompletedTask;
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Seeder manipulation Methods
    // -----------------------------------------------------------------------------------------------------------------
    public IDataSeederService AddSeeder<TSeeder>() where TSeeder : ISeeder
        => AddSeederGroup(group => group.AddSeeder<TSeeder>());

    public IDataSeederService AddSeederGroup(params ISeeder[] seeders)
        => AddSeederGroup(new SeederGroup(seeders, serviceProvider));

    public IDataSeederService AddSeederGroup(Action<SeederGroup> group) {
        var seeders = new SeederGroup(serviceProvider);
        group(seeders);
        return AddSeederGroup(seeders);
    }

    public IDataSeederService AddSeederGroup(SeederGroup group) {
        ThrowIfRemainderSeeders();

        _seeders.Enqueue(group);
        foreach (ISeeder seeder in group) {
            _seederTypes.Add(seeder.GetType());
        }

        return this;
    }

    public void AddRemainderSeeders(Assembly assembly) {
        Type[] types = CollectTypes(assembly);
        var errors = new List<Exception>();

        foreach (Type type in types) {
            if (_seederTypes.Contains(type)) {
                logger.LogDebug("Skipping {t} as it was already assigned", type);
                continue;
            }

            try {
                AddSeederGroup(group => group.AddSeeder(type));
            }
            catch (Exception ex) {
                logger.LogError(ex, "Failed to instantiate {t}. Skipping...", type);
                errors.Add(ex);
            }
        }

        if (errors.Count != 0) throw new AggregateException(errors);

        _collectedRemainders = true;
    }

    public void AddRemainderSeedersAsOneGroup(Assembly assembly) {
        Type[] types = CollectTypes(assembly);
        var group = new SeederGroup(serviceProvider);
        var errors = new List<Exception>();

        foreach (Type type in types) {
            if (_seederTypes.Contains(type)) {
                logger.LogDebug("Skipping {t} as it was already assigned", type);
                continue;
            }

            try {
                group.AddSeeder(type);
                _seederTypes.Add(type);
            }
            catch (Exception ex) {
                logger.LogError(ex, "Failed to instantiate {t}. Skipping...", type);
                errors.Add(ex);
            }
        }

        if (errors.Count != 0) throw new AggregateException(errors);

        // Collect as one Concurrent step
        AddSeederGroup(group);
        _collectedRemainders = true;
    }

    protected virtual Task CollectAsync(CancellationToken ct = default) => Task.CompletedTask;

    private static Type[] CollectTypes(Assembly assembly)
        => assembly.GetTypes()
            // order is deterministic
            .OrderBy(t => t.FullName)
            .Where(type => type.IsAssignableTo(typeof(ISeeder))
                && type is { IsAbstract: false, IsInterface: false, IsGenericTypeDefinition: false })
            .ToArray();

    private void ThrowIfRemainderSeeders() {
        if (!_collectedRemainders) return;

        logger.LogError("Remainder seeders have already been collected");
        throw new InvalidOperationException("Remainder seeders have already been collected");
    }

    protected virtual bool ValidateSeeders() {
        if (!_seeders.IsEmpty) return true;

        logger.LogWarning("No seeders were added prior to execution.");
        return false;

    }
}
