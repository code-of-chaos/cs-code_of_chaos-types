// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.Types;
using JetBrains.Annotations;
using System.Diagnostics;

namespace Tests.CodeOfChaos.Types.TypedValueStore;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[TestSubject(typeof(global::CodeOfChaos.Types.TypedValueStore))]
public class TypedValueStoreTest {

    private const int PreFilledStoreAmount = 12;
    private static global::CodeOfChaos.Types.TypedValueStore GetPrefilledStore() {
        var store = new global::CodeOfChaos.Types.TypedValueStore();

        store.TryAdd("alpha", "something");
        store.TryAdd("beta", 123);
        store.TryAdd("gamma", 1.23f);
        store.TryAdd("delta", 1.23d);
        store.TryAdd("epsilon", 123L);
        store.TryAdd("zeta", true);
        store.TryAdd("eta", false);
        store.TryAdd("theta", 'c');
        store.TryAdd("iota", new DateTime(2022, 01, 01));
        store.TryAdd("kappa", new DateTimeOffset(2022, 01, 01, 12, 00, 00, TimeSpan.Zero));
        store.TryAdd("lambda", new TimeSpan(1, 2, 3));
        store.TryAdd("mu", Guid.Parse("d11b1e03-fd98-442e-8235-2c59dc40e516"));

        return store;

    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    #region TryAdd_ShouldAddNewItem
    private static async Task TryAdd_ShouldAddNewItem<T>(string key, T value) where T : notnull {
        // Arrange
        global::CodeOfChaos.Types.TypedValueStore store = new();

        // Act
        bool result = store.TryAdd(key, value);
        bool resultReturn = store.TryGetValue(key, out T? resultValue);

        // Assert
        await Assert.That(result).IsTrue().Because("Expected item to be added successfully.");
        await Assert.That(resultReturn).IsTrue().Because("Expected item to be returned successfully.");
        await Assert.That(resultValue).IsEqualTo(value).Because("Expected item to be returned successfully.");
        await Assert.That(resultValue).IsTypeOf<T>();
        await Assert.That(store).IsNotEmpty().Because("Expected store to contain the added item.");
    }
    
    [Test]
    public async Task TryAdd_ShouldAddNewItem(){
        // TUnit doesn't allow for the same generic targeting, so this is the current workaround
        //      Not perfect, but it will have to do for now
        Task[] tasks = [
            TryAdd_ShouldAddNewItem("alpha", "something"),
            TryAdd_ShouldAddNewItem("beta", 123),
            TryAdd_ShouldAddNewItem("gamma", 1.23f),
            TryAdd_ShouldAddNewItem("delta", 1.23d),
            TryAdd_ShouldAddNewItem("epsilon", 123L),
            TryAdd_ShouldAddNewItem("zeta", true),
            TryAdd_ShouldAddNewItem("eta", false),
            TryAdd_ShouldAddNewItem("theta", 'c'),
            TryAdd_ShouldAddNewItem("iota", DateTime.Parse("2022-01-01T00:00:00")),
            TryAdd_ShouldAddNewItem("kappa", DateTimeOffset.Parse("2022-01-01T12:00:00+00:00")),
            TryAdd_ShouldAddNewItem("lambda", TimeSpan.Parse("01:02:03")),
            TryAdd_ShouldAddNewItem("mu",     Guid.Parse("d11b1e03-fd98-442e-8235-2c59dc40e516")),
        ];
        
        await Task.WhenAll(tasks);
    }
    #endregion

    #region TryGetValue_ShouldReturnCorrectValue
    private static async Task TryGetValue_ShouldReturnCorrectValue<T>(string key, T value) where T : notnull {
        // Arrange
        global::CodeOfChaos.Types.TypedValueStore store = GetPrefilledStore();

        // Act
        bool result = store.TryGetValue(key, out T? resultValue);

        // Assert
        await Assert.That(result).IsTrue().Because("Expected item to be added successfully.");
        await Assert.That(resultValue)
            .IsNotNull()
            .And.IsEqualTo(value).Because("Expected item to be returned successfully.")
            .And.IsTypeOf<T>();
    }

    [Test]
    public async Task TryGetValue_ShouldReturnCorrectValue() {
        Task[] tasks = [
            TryGetValue_ShouldReturnCorrectValue("alpha", "something"),
            TryGetValue_ShouldReturnCorrectValue("beta", 123),
            TryGetValue_ShouldReturnCorrectValue("gamma", 1.23f),
            TryGetValue_ShouldReturnCorrectValue("delta", 1.23d),
            TryGetValue_ShouldReturnCorrectValue("epsilon", 123L),
            TryGetValue_ShouldReturnCorrectValue("zeta", true),
            TryGetValue_ShouldReturnCorrectValue("eta", false),
            TryGetValue_ShouldReturnCorrectValue("theta", 'c'),
            TryGetValue_ShouldReturnCorrectValue("iota", DateTime.Parse("2022-01-01T00:00:00")),
            TryGetValue_ShouldReturnCorrectValue("kappa", DateTimeOffset.Parse("2022-01-01T12:00:00+00:00")),
            TryGetValue_ShouldReturnCorrectValue("lambda", TimeSpan.Parse("01:02:03")),
            TryGetValue_ShouldReturnCorrectValue("mu", Guid.Parse("d11b1e03-fd98-442e-8235-2c59dc40e516")),
        ];
         
        await Task.WhenAll(tasks);
    }
    #endregion

     [Test]
     [Arguments("alpha")]
     [Arguments("beta")]
     [Arguments("gamma")]
     [Arguments("delta")]
     [Arguments("epsilon")]
     [Arguments("zeta")]
     [Arguments("eta")]
     [Arguments("theta")]
     [Arguments("iota")]
     [Arguments("kappa")]
     [Arguments("lambda")]
     [Arguments("mu")]
     public async Task ContainsKey_ShouldReturnTrue(string key) {
         // Arrange
         global::CodeOfChaos.Types.TypedValueStore store = GetPrefilledStore();

         // Act
         bool result = store.ContainsKey(key);

         // Assert
         await Assert.That(result).IsTrue().Because("Expected key to exist in the TypedValueStore.");
     }

     [Test]
     [Arguments("NOTHING")]
     public async Task ContainsKey_ShouldReturnFalse(string key) {
         // Arrange
         global::CodeOfChaos.Types.TypedValueStore store = GetPrefilledStore();

         // Act
         bool result = store.ContainsKey(key);

         // Assert
         await Assert.That(result).IsFalse().Because("Expected key to not exist in the TypedValueStore.");
     }


     [Test]
     [Arguments("alpha")]
     [Arguments("beta")]
     [Arguments("gamma")]
     [Arguments("delta")]
     [Arguments("epsilon")]
     [Arguments("zeta")]
     [Arguments("eta")]
     [Arguments("theta")]
     [Arguments("iota")]
     [Arguments("kappa")]
     [Arguments("lambda")]
     [Arguments("mu")]
     public async Task TryRemove_ShouldReturnTrue(string key) {
         // Arrange
         global::CodeOfChaos.Types.TypedValueStore store = GetPrefilledStore();

         // Act
         bool result = store.TryRemove(key);

         // Assert
         await Assert.That(result).IsTrue().Because("Expected item to be removed successfully.");
     }

     private async Task TryRemoveWithGeneric_ShouldReturnCorrectValue<T>(string key, T expectedValue) where T : notnull {
         
         // Arrange
         global::CodeOfChaos.Types.TypedValueStore store = GetPrefilledStore();

         // Act
         bool result = store.TryRemove(key, out T? resultValue);

         // Assert
         await Assert.That(result).IsTrue().Because("Expected item to be removed successfully.");
         await Assert.That(resultValue).IsEqualTo(expectedValue).Because("Expected item to be removed successfully.")
             .And.IsTypeOf<T>()
             .And.IsEqualTo(expectedValue);
     }

     [Test]
     public async Task TryRemoveWithGeneric_ShouldReturnCorrectValue() {
         Task[] tasks = [
             TryRemoveWithGeneric_ShouldReturnCorrectValue("alpha", "something"),
             TryRemoveWithGeneric_ShouldReturnCorrectValue("beta", 123),
             TryRemoveWithGeneric_ShouldReturnCorrectValue("gamma", 1.23f),
             TryRemoveWithGeneric_ShouldReturnCorrectValue("delta", 1.23d),
             TryRemoveWithGeneric_ShouldReturnCorrectValue("epsilon", 123L),
             TryRemoveWithGeneric_ShouldReturnCorrectValue("zeta", true),
             TryRemoveWithGeneric_ShouldReturnCorrectValue("eta", false),
             TryRemoveWithGeneric_ShouldReturnCorrectValue("theta", 'c'),
             TryRemoveWithGeneric_ShouldReturnCorrectValue("iota", DateTime.Parse("2022-01-01T00:00:00")),
             TryRemoveWithGeneric_ShouldReturnCorrectValue("kappa", DateTimeOffset.Parse("2022-01-01T12:00:00+00:00")),
             TryRemoveWithGeneric_ShouldReturnCorrectValue("lambda", TimeSpan.Parse("01:02:03")),
             TryRemoveWithGeneric_ShouldReturnCorrectValue("mu", Guid.Parse("d11b1e03-fd98-442e-8235-2c59dc40e516")),
         ];
         
         await Task.WhenAll(tasks);
     }

     [Test]
     public async Task Clear_ShouldClearAllItems() {
         // Arrange
         global::CodeOfChaos.Types.TypedValueStore store = GetPrefilledStore();

         // Act
         store.Clear();

         // Assert
         await Assert.That(store).IsEmpty().Because("Expected store to be empty after Clear.");
     }

     [Test]
     public async Task Count_ShouldReturnCorrectCount() {
         // Arrange
         global::CodeOfChaos.Types.TypedValueStore store = GetPrefilledStore();

         // Act
         int count = store.Count;

         // Assert
         await Assert.That(count).IsEqualTo(PreFilledStoreAmount).Because("Expected store to contain the correct amount of items.");
     }
//
//     [Test]
//     [Arguments("alpha", "else")]
//     [Arguments("beta", 456)]
//     [Arguments("gamma", 456f)]
//     [Arguments("delta", 456d)]
//     [Arguments("epsilon", 456L)]
//     [Arguments("zeta", false)]
//     [Arguments("eta", true)]
//     [Arguments("theta", 'd')]
//     [Arguments("iota", "2024-01-01T00:00:00")]
//     [Arguments("kappa", "2024-01-01T12:00:00+00:00")]
//     [Arguments("lambda", "03:02:01")]
//     [Arguments("mu", "d11b1e03-fd98-442e-8235-2c59dc40e517")]
//     public async Task TryUpdate_ShouldUpdateCorrectly<T>(string key, T value) where T : notnull {
//
//         // Arrange
//         TypedValueStore store = GetPrefilledStore();
//         bool foundOriginalValue = store.TryGetValue(key, out T? originalValue);
//
//         // Act
//         bool result = store.TryUpdate(key, value);
//         bool resultReturn = store.TryGetValue(key, out T? resultValue);
//
//         // Assert
//         Assert.True(foundOriginalValue, "Expected original value to be found.");
//         Assert.True(result, "Expected item to be updated successfully.");
//         Assert.True(resultReturn, "Expected item to be returned successfully.");
//         Assert.IsType<T>(resultValue);
//         Assert.NotEqual(originalValue, resultValue);
//     }
//
//     [Test]
//     [Arguments<string>("alpha", "else")]
//     [Arguments<int>("beta", 456)]
//     [Arguments<float>("gamma", 456f)]
//     [Arguments<double>("delta", 456d)]
//     [Arguments<long>("epsilon", 456L)]
//     [Arguments<bool>("zeta", false)]
//     [Arguments<bool>("eta", true)]
//     [Arguments<char>("theta", 'd')]
//     [Arguments<DateTime>("iota", "2024-01-01T00:00:00")]
//     [Arguments<DateTimeOffset>("kappa", "2024-01-01T12:00:00+00:00")]
//     [Arguments<TimeSpan>("lambda", "03:02:01")]
//     [Arguments<Guid>("mu", "d11b1e03-fd98-442e-8235-2c59dc40e517")]
//     public async Task Indexer_Set_ShouldUpdateValue<T>(string key, T newValue) where T : notnull {
//         // Arrange
//         TypedValueStore store = GetPrefilledStore();
//
//         // Act
//         bool updateResult = store.TryUpdate(key, newValue);
//         bool result = store.TryGetValue(key, out T? resultValue);
//
//         // Assert
//         Assert.True(updateResult);
//         Assert.True(result);
//         Assert.Equal(newValue, resultValue);
//     }
//
//     [Test]
//     [Arguments<string>("alpha", "updated")]
//     [Arguments<int>("beta", 999)]
//     [Arguments<float>("gamma", 9.99f)]
//     [Arguments<double>("delta", 9.99d)]
//     [Arguments<long>("epsilon", 999L)]
//     [Arguments<bool>("zeta", false)]
//     [Arguments<bool>("eta", true)]
//     [Arguments<char>("theta", 'z')]
//     [Arguments<DateTime>("iota", "2025-01-01T00:00:00")]
//     [Arguments<DateTimeOffset>("kappa", "2025-01-01T12:00:00+00:00")]
//     [Arguments<TimeSpan>("lambda", "05:06:07")]
//     [Arguments<Guid>("mu", "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee")]
//     public async Task AddOrUpdate_ShouldUpdateOrAddValue<T>(string key, T newValue) where T : notnull {
//         // Arrange
//         TypedValueStore store = GetPrefilledStore();
//         bool initialContains = store.ContainsKey(key);
//
//         // Act
//         store.AddOrUpdate(key, newValue);
//         bool resultReturn = store.TryGetValue(key, out T? resultValue);
//
//         // Assert
//         Assert.True(resultReturn, "Expected item to be in the store.");
//         Assert.IsType<T>(resultValue);
//         Assert.Equal(newValue, resultValue);
//         if (initialContains && newValue is not false) {
//             Assert.NotEqual(newValue, default);
//         }
//     }
//
     [Test]
     public async Task ToImmutable_ShoutCreateImmutableTypedValueStore() {
         // Arrange
         global::CodeOfChaos.Types.TypedValueStore store = GetPrefilledStore();

         // Act
         ImmutableTypedValueStore<string> immutableStore = store.ToImmutable();

         // Assert
         await Assert.That(store.Count).IsEqualTo(immutableStore.Count).Because("Expected store to contain the correct amount of items.");
         await Assert.That(store.Equals(immutableStore)).IsTrue().Because("Expected store to be equal to the immutable store.");
     }

     [Test]
     public async Task ToFrozen_ShoutCreateFrozenTypedValueStore() {
         // Arrange
         global::CodeOfChaos.Types.TypedValueStore store = GetPrefilledStore();

         // Act
         FrozenTypedValueStore<string> frozenStore = store.ToFrozen();

         // Assert
         await Assert.That(store.Count).IsEqualTo(frozenStore.Count).Because("Expected store to contain the correct amount of items.");
         await Assert.That(store.Equals(frozenStore)).IsTrue().Because("Expected store to be equal to the frozen store.");
     }

     [Test]
     public async Task Equals_ShouldReturnTrue() {
         // Arrange
         global::CodeOfChaos.Types.TypedValueStore store1 = GetPrefilledStore();
         global::CodeOfChaos.Types.TypedValueStore store2 = GetPrefilledStore();

         // Act
         bool result = store1.Equals(store2);

         // Assert
         await Assert.That(result).IsTrue().Because("Expected stores to be equal.");
     }

     [Test]
     public async Task Equals_ShouldReturnFalse() {
         // Arrange
         global::CodeOfChaos.Types.TypedValueStore store1 = GetPrefilledStore();
         global::CodeOfChaos.Types.TypedValueStore store2 = GetPrefilledStore();

         // Act
         bool updateResult = store2.TryUpdate("alpha", "else");
         bool result = store1.Equals(store2);

         // Assert
         await Assert.That(updateResult).IsTrue().Because("Expected item to be updated successfully.");
         await Assert.That(result).IsFalse().Because("Expected stores to be unequal.");
     }

     [Test]
     public async Task Equals_ShouldReturnFalseForDifferentObjectType() {
         // Arrange
         global::CodeOfChaos.Types.TypedValueStore store = GetPrefilledStore();
         object notAStore = new();

         // Act
         bool result = store.Equals(notAStore);

         // Assert
         await Assert.That(result).IsFalse().Because("Expected stores to be unequal.");
     }

     [Test]
     public async Task TryGetValue_ShouldReturnFalseOnNullOrNonExistentKey() {
         // Arrange
         global::CodeOfChaos.Types.TypedValueStore store = new();

         // Act
         bool result = store.TryGetValue("nonExistingKey", out object? retrievedValue);

         // Assert
         await Assert.That(result).IsFalse().Because("Expected item to not exist.");
         await Assert.That(retrievedValue).IsNull().Because("Expected item to not exist.");
     }

     [Test]
     public async Task Enumerator_ShouldReturnAllStoredItems() {
         // Arrange
         global::CodeOfChaos.Types.TypedValueStore store = GetPrefilledStore();
         HashSet<string> expectedKeys = [
             "alpha",
             "beta",
             "gamma",
             "delta",
             "epsilon",
             "zeta",
             "eta",
             "theta",
             "iota",
             "kappa",
             "lambda",
             "mu"
         ];

         // Act & Assert
         foreach (KeyValuePair<string, IValueContainer> kvp in store) {
             await Assert.That(kvp.Value).IsNotNull().Because("Expected item to exist.");
             await Assert.That(expectedKeys).Contains(kvp.Key).Because("Expected item to exist.");
         }
     }

    [Test]
    public async Task PerformanceTest_AddingLargeVolumeOfItems() {
        // Arrange
        const int numberOfItems = 1_000_000;
        var store = new global::CodeOfChaos.Types.TypedValueStore();
        var stopwatch = new Stopwatch();

        // Act
        stopwatch.Start();

        for (int i = 0; i < numberOfItems; i++) {
            store.TryAdd($"key{i}", i);
        }

        stopwatch.Stop();
        long addingTime = stopwatch.ElapsedMilliseconds;

        // Assert
        await Assert.That(store).IsNotEmpty()
            .And.HasCount().EqualTo(numberOfItems);
        Console.WriteLine($"Time to add {numberOfItems} items: {addingTime} ms");

        stopwatch.Reset();

        // Act
        stopwatch.Start();

        for (int i = 0; i < numberOfItems; i++) {
            bool exists = store.TryGetValue($"key{i}", out int value);
            
            await Assert.That(exists).IsTrue();
            await Assert.That(value).IsEqualTo(i);
        }

        stopwatch.Stop();
        long retrievalTime = stopwatch.ElapsedMilliseconds;

        // Assert
        Console.WriteLine($"Time to retrieve {numberOfItems} items: {retrievalTime} ms");
    }
}
