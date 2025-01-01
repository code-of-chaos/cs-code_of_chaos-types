// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Reflection;

namespace CodeOfChaos.Types;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class OneTimeDataSeederService(IServiceProvider serviceProvider, ILogger<OneTimeDataSeederService> logger) : IDataSeederService {
    /// <summary>
    ///     Represents a thread-safe queue of SeederGroup objects used within the OneTimeDataSeederService
    ///     to maintain and manage seeding operations.
    /// </summary>
    /// <remarks>
    ///     The <c>Seeders</c> field serves as the central storage for SeederGroups, allowing them to be
    ///     enqueued and dequeued in a controlled manner during the seeding process. This ensures
    ///     proper execution order and thread safety for concurrent operations.
    /// </remarks>
    protected readonly ConcurrentQueue<SeederGroup> Seeders = [];

    /// <summary>
    ///     Represents a thread-safe collection of seeder types used by the OneTimeDataSeederService to track
    ///     registered data seeders. This collection ensures that each type of seeder is only added once
    ///     and prevents duplicates during the seeding process. It plays a critical role in managing
    ///     and validating the lifecycle of seeders added to the service.
    /// </summary>
    protected readonly ConcurrentBag<Type> SeederTypes = [];

    /// <summary>
    ///     Indicates whether the remainder seeders have been successfully collected.
    ///     If set to <c>true</c>, it indicates that no additional remainder seeders can
    ///     be added to the service, and attempting to do so will throw an exception.
    /// </summary>
    protected bool CollectedRemainders;// If set to true, the remainder seeders have been collected and thus should throw an exception if any are added

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Starts the data seeding process by executing all configured seeder groups in sequence.
    ///     Validates seeders, collects data, and manages the lifecycle of each group using scoped services.
    /// </summary>
    /// <param name="ct">A CancellationToken used to observe cancellation requests.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task StartAsync(CancellationToken ct = default) {
        logger.LogInformation("DataSeederService starting...");

        await CollectAsync(ct);// If user choose the old format of adding seeders, this will be what ads the seeders to the queue
        ct.ThrowIfCancellationRequested();// Don't throw during collection, but throw afterward

        // Validation has to succeed before we continue
        //      Technically this library doesn't need to validate much,
        //      but to make sure we've enabled overloading of the method
        if (!ValidateSeeders()) return;

        int i = 0;
        while (Seeders.TryDequeue(out SeederGroup seederGroup)) {
            if (ct.IsCancellationRequested) {
                logger.LogWarning("Seeding process canceled during execution.");
                break;
            }

            if (seederGroup.IsEmpty) {
                logger.LogDebug("ExecutionStep {step} : Skipping empty seeder array", i++);
                continue;
            }

            // Each SEEDER should have their own scope
            List<(Task, AsyncServiceScope)> seederTasks = [];

            while (seederGroup.SeederTypes.TryDequeue(out Type? seederType)) {
                AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
                IServiceProvider scopeProvider = scope.ServiceProvider;
                
                // Because of checks by the SeederGroup struct we know that the seeder inherits from ISeeder and thus is not null
                var seeder = (ISeeder) scopeProvider.GetRequiredService(seederType);
                Task seederTask = seeder.StartAsync(scopeProvider, ct); 
                
                // Because our scope has to be gracefully disposed, we add the scope here
                seederTasks.Add((seederTask, scope));
            }

            logger.LogDebug("ExecutionStep {step} : {count} Seeder(s) found, executing...", i++, seederTasks.Count);
            await Task.WhenAll(seederTasks.Select(t => t.Item1));
            
            // Gracefully dispose the scope
            foreach (AsyncServiceScope scope in seederTasks.Select(t => t.Item2)) {
                await scope.DisposeAsync();
            }
        }

        logger.LogInformation("All seeders completed in {i} steps", i);
        // Cleanup
        Seeders.Clear();
        SeederTypes.Clear();
        CollectedRemainders = false;
    }

    /// <summary>
    ///     Asynchronously stops the OneTimeDataSeederService, handling any necessary cleanup or finalization logic.
    /// </summary>
    /// <param name="ct">The cancellation token to observe while awaiting the task to complete.</param>
    /// <returns>A task that completes when the service has been stopped.</returns>
    public Task StopAsync(CancellationToken ct = default) {
        logger.LogInformation("Stopping DataSeederService...");
        return Task.CompletedTask;
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Seeder manipulation Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc />
    public IDataSeederService AddSeeder<TSeeder>() where TSeeder : ISeeder
        => AddSeederGroup(group => group.AddSeeder<TSeeder>());

    /// <inheritdoc />
    public IDataSeederService AddSeederGroup(Action<SeederGroup> group) {
        var seeders = new SeederGroup();
        group(seeders);
        return AddSeederGroup(seeders);
    }

    /// <inheritdoc />
    public IDataSeederService AddSeederGroup(SeederGroup group) {
        ThrowIfRemainderSeeders();

        Seeders.Enqueue(group);
        foreach (Type seeder in group.SeederTypes.ToArray()) {
            SeederTypes.Add(seeder);
        }

        return this;
    }

    /// <inheritdoc />
    public void AddRemainderSeeders(Assembly assembly) {
        Type[] types = CollectTypes(assembly);
        var errors = new List<Exception>();

        foreach (Type type in types) {
            if (SeederTypes.Contains(type)) {
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

        CollectedRemainders = true;
    }

    /// <inheritdoc />
    public void AddRemainderSeedersAsOneGroup(Assembly assembly) {
        Type[] types = CollectTypes(assembly);
        var group = new SeederGroup();
        var errors = new List<Exception>();

        foreach (Type type in types) {
            if (SeederTypes.Contains(type)) {
                logger.LogDebug("Skipping {t} as it was already assigned", type);
                continue;
            }

            try {
                group.AddSeeder(type);
                SeederTypes.Add(type);
            }
            catch (Exception ex) {
                logger.LogError(ex, "Failed to instantiate {t}. Skipping...", type);
                errors.Add(ex);
            }
        }

        if (errors.Count != 0) throw new AggregateException(errors);

        // Collect as one Concurrent step
        AddSeederGroup(group);
        CollectedRemainders = true;
    }

    /// <summary>
    ///     Collects seeders asynchronously to prepare for execution based on the seeding logic.
    ///     This method is intended to be overridden for custom collection logic.
    /// </summary>
    /// <param name="ct">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    protected virtual Task CollectAsync(CancellationToken ct = default) => Task.CompletedTask;

    private static Type[] CollectTypes(Assembly assembly)
        => assembly.GetTypes()
            // order is deterministic
            .OrderBy(t => t.FullName)
            .Where(type => type.IsAssignableTo(typeof(ISeeder))
                && type is { IsAbstract: false, IsInterface: false, IsGenericTypeDefinition: false })
            .ToArray();

    private void ThrowIfRemainderSeeders() {
        if (!CollectedRemainders) return;

        logger.LogError("Remainder seeders have already been collected");
        throw new InvalidOperationException("Remainder seeders have already been collected");
    }

    protected virtual bool ValidateSeeders() {
        if (!Seeders.IsEmpty) return true;

        logger.LogWarning("No seeders were added prior to execution.");
        return false;

    }
}
