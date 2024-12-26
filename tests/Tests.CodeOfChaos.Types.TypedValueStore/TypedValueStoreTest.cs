// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.Types;
using JetBrains.Annotations;
using System.Diagnostics;

namespace Tests.CodeOfChaos.Types;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[TestSubject(typeof(TypedValueStore))]
public class TypedValueStoreTest {

    private const int PreFilledStoreAmount = 12;
    private static TypedValueStore GetPrefilledStore() {
        var store = new TypedValueStore();

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
        TypedValueStore store = new();

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
        TypedValueStore store = GetPrefilledStore();

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
         TypedValueStore store = GetPrefilledStore();

         // Act
         bool result = store.ContainsKey(key);

         // Assert
         await Assert.That(result).IsTrue().Because("Expected key to exist in the TypedValueStore.");
     }

     [Test]
     [Arguments("NOTHING")]
     public async Task ContainsKey_ShouldReturnFalse(string key) {
         // Arrange
         TypedValueStore store = GetPrefilledStore();

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
         TypedValueStore store = GetPrefilledStore();

         // Act
         bool result = store.TryRemove(key);

         // Assert
         await Assert.That(result).IsTrue().Because("Expected item to be removed successfully.");
     }

     private static async Task TryRemoveWithGeneric_ShouldReturnCorrectValue<T>(string key, T expectedValue) where T : notnull {
         
         // Arrange
         TypedValueStore store = GetPrefilledStore();

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
         TypedValueStore store = GetPrefilledStore();

         // Act
         store.Clear();

         // Assert
         await Assert.That(store).IsEmpty().Because("Expected store to be empty after Clear.");
     }

     [Test]
     public async Task Count_ShouldReturnCorrectCount() {
         // Arrange
         TypedValueStore store = GetPrefilledStore();

         // Act
         int count = store.Count;

         // Assert
         await Assert.That(count).IsEqualTo(PreFilledStoreAmount).Because("Expected store to contain the correct amount of items.");
     }
     
     private static async Task TryUpdate_ShouldUpdateCorrectly<T>(string key, T value) where T : notnull {
         // Arrange
         TypedValueStore store = GetPrefilledStore();
         bool foundOriginalValue = store.TryGetValue(key, out T? originalValue);
         
         // Act
         bool result = store.TryUpdate(key, value);
         bool resultReturn = store.TryGetValue(key, out T? resultValue);
         
         // Assert
         await Assert.That(foundOriginalValue).IsTrue().Because("Expected original value to be found.");
         await Assert.That(result).IsTrue().Because("Expected item to be updated successfully.");
         await Assert.That(resultReturn).IsTrue().Because("Expected item to be returned successfully.");
         await Assert.That(resultValue).IsTypeOf<T>()
             .And.IsEqualTo(value)
             .And.IsNotEqualTo(originalValue);
     }
     

     [Test]
     public async Task TryUpdate_ShouldUpdateCorrectly() {
         Task[] tasks = [
             TryUpdate_ShouldUpdateCorrectly("alpha", "else"),
             TryUpdate_ShouldUpdateCorrectly("beta", 456),
             TryUpdate_ShouldUpdateCorrectly("gamma", 456f),
             TryUpdate_ShouldUpdateCorrectly("delta", 456d),
             TryUpdate_ShouldUpdateCorrectly("epsilon", 456L),
             TryUpdate_ShouldUpdateCorrectly("zeta", false),
             TryUpdate_ShouldUpdateCorrectly("eta", true),
             TryUpdate_ShouldUpdateCorrectly("theta", 'd'),
             TryUpdate_ShouldUpdateCorrectly("iota", DateTime.Parse("2024-01-01T00:00:00")),
             TryUpdate_ShouldUpdateCorrectly("kappa", DateTimeOffset.Parse("2024-01-01T12:00:00+00:00")),
             TryUpdate_ShouldUpdateCorrectly("lambda", TimeSpan.Parse("03:02:01")),
             TryUpdate_ShouldUpdateCorrectly("mu", Guid.Parse("d11b1e03-fd98-442e-8235-2c59dc40e517")),
         ];
         
         await Task.WhenAll(tasks);
     }

     private async Task AddOrUpdate_ShouldUpdateOrAddValue<T>(string key, T value) where T : notnull {
         // Arrange
         TypedValueStore store = GetPrefilledStore();

         // Act
         store.AddOrUpdate(key, value);
         bool resultReturn = store.TryGetValue(key, out T? resultValue);

         // Assert
         await Assert.That(resultReturn).IsTrue().Because("Expected item to be in the store.");
         await Assert.That(resultValue).IsTypeOf<T>()
             .And.IsEqualTo(value);
     }

     [Test]
     public async Task AddOrUpdate_ShouldUpdateOrAddValue() {
         Task[] tasks = [
             AddOrUpdate_ShouldUpdateOrAddValue("alpha", "updated"),
             AddOrUpdate_ShouldUpdateOrAddValue("beta", 999),
             AddOrUpdate_ShouldUpdateOrAddValue("gamma", 9.99f),
             AddOrUpdate_ShouldUpdateOrAddValue("delta", 9.99d),
             AddOrUpdate_ShouldUpdateOrAddValue("epsilon", 999L),
             AddOrUpdate_ShouldUpdateOrAddValue("zeta", false),
             AddOrUpdate_ShouldUpdateOrAddValue("eta", true),
             AddOrUpdate_ShouldUpdateOrAddValue("theta", 'z'),
             AddOrUpdate_ShouldUpdateOrAddValue("iota", DateTime.Parse("2025-01-01T00:00:00")),
             AddOrUpdate_ShouldUpdateOrAddValue("kappa", DateTimeOffset.Parse("2025-01-01T12:00:00+00:00")),
             AddOrUpdate_ShouldUpdateOrAddValue("lambda", TimeSpan.Parse("05:06:07")),
             AddOrUpdate_ShouldUpdateOrAddValue("mu", Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee")),
         ];
         
         await Task.WhenAll(tasks);
     }
     
     [Test]
     public async Task ToImmutable_ShoutCreateImmutableTypedValueStore() {
         // Arrange
         TypedValueStore store = GetPrefilledStore();

         // Act
         ImmutableTypedValueStore<string> immutableStore = store.ToImmutable();

         // Assert
         await Assert.That(store.Count).IsEqualTo(immutableStore.Count).Because("Expected store to contain the correct amount of items.");
         await Assert.That(store.Equals(immutableStore)).IsTrue().Because("Expected store to be equal to the immutable store.");
     }

     [Test]
     public async Task ToFrozen_ShoutCreateFrozenTypedValueStore() {
         // Arrange
         TypedValueStore store = GetPrefilledStore();

         // Act
         FrozenTypedValueStore<string> frozenStore = store.ToFrozen();

         // Assert
         await Assert.That(store.Count).IsEqualTo(frozenStore.Count).Because("Expected store to contain the correct amount of items.");
         await Assert.That(store.Equals(frozenStore)).IsTrue().Because("Expected store to be equal to the frozen store.");
     }

     [Test]
     public async Task Equals_ShouldReturnTrue() {
         // Arrange
         TypedValueStore store1 = GetPrefilledStore();
         TypedValueStore store2 = GetPrefilledStore();

         // Act
         bool result = store1.Equals(store2);

         // Assert
         await Assert.That(result).IsTrue().Because("Expected stores to be equal.");
     }

     [Test]
     public async Task Equals_ShouldReturnFalse() {
         // Arrange
         TypedValueStore store1 = GetPrefilledStore();
         TypedValueStore store2 = GetPrefilledStore();

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
         TypedValueStore store = GetPrefilledStore();
         object notAStore = new();

         // Act
         bool result = store.Equals(notAStore);

         // Assert
         await Assert.That(result).IsFalse().Because("Expected stores to be unequal.");
     }

     [Test]
     public async Task TryGetValue_ShouldReturnFalseOnNullOrNonExistentKey() {
         // Arrange
         TypedValueStore store = new();

         // Act
         bool result = store.TryGetValue("nonExistingKey", out object? retrievedValue);

         // Assert
         await Assert.That(result).IsFalse().Because("Expected item to not exist.");
         await Assert.That(retrievedValue).IsNull().Because("Expected item to not exist.");
     }

     [Test]
     public async Task Enumerator_ShouldReturnAllStoredItems() {
         // Arrange
         TypedValueStore store = GetPrefilledStore();
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
        var store = new TypedValueStore();
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
