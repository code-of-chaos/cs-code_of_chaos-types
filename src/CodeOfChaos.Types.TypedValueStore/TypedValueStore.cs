// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace CodeOfChaos.Types;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     The TypedValueStore class provides a key-value storage system where the key can be of any string.
///     Is a Helper class of <see cref="TypedValueStore{TKey}"/> already predefined as a string based key storage.
/// </summary>
public class TypedValueStore : TypedValueStore<string>;

/// <summary>
///     Represents a storage system that maintains key-value pairs where the keys are of a specified type
///     and the values are encapsulated within an <see cref="IValueContainer" />.
/// </summary>
/// <typeparam name="TKey">
///     The type of keys stored within the <see cref="TypedValueStore{TKey}" />. The key type must be non-nullable.
/// </typeparam>
/// <remarks>
///     This class provides thread-safe operations for adding, updating, retrieving, and removing values
///     associated with specific keys. It also offers conversions to immutable and frozen states.
/// </remarks>
public class TypedValueStore<TKey> :
    IEnumerable<KeyValuePair<TKey, IValueContainer>>,
    IEquatable<ImmutableTypedValueStore<TKey>>,
    IEquatable<FrozenTypedValueStore<TKey>>
    where TKey : notnull 
{
    /// <summary>
    ///     A thread-safe collection that stores key-value pairs where the key is of type <typeparamref name="TKey" /> and the
    ///     value
    ///     is encapsulated within an <see cref="IValueContainer" />. Provides functionalities to add, remove, and update
    ///     entries and
    ///     supports conversion to immutable or frozen stores.
    /// </summary>
    /// <remarks>
    ///     The <c>_storage</c> uses <see cref="ConcurrentDictionary{TKey,TValue}" /> to ensure thread-safety.
    /// </remarks>
    private readonly ConcurrentDictionary<TKey, IValueContainer> _storage = new();

    /// <summary>
    ///     Gets the number of key/value pairs contained in the TypedValueStore.
    ///     This property provides the current count of elements stored,
    ///     reflecting the total number of entries in the underlying storage.
    /// </summary>
    public int Count => _storage.Count;

    /// <summary>
    ///     Returns an enumerator that iterates through the collection of key-value pairs stored
    ///     in the <see cref="TypedValueStore{TKey}" />.
    /// </summary>
    /// <returns>
    ///     An enumerator that can be used to iterate through the collection of key-value
    ///     pairs, where each key is of type <typeparamref name="TKey" /> and each value is
    ///     encapsulated in an <see cref="IValueContainer" />.
    /// </returns>
    public IEnumerator GetEnumerator() => _storage.GetEnumerator();

    /// <summary>
    ///     Returns an enumerator that iterates through the collection of key-value pairs stored in the
    ///     <see cref="TypedValueStore{TKey}" />.
    /// </summary>
    /// <returns>An enumerator for the collection of key-value pairs.</returns>
    IEnumerator<KeyValuePair<TKey, IValueContainer>> IEnumerable<KeyValuePair<TKey, IValueContainer>>.GetEnumerator() => _storage.GetEnumerator();

    // -----------------------------------------------------------------------------------------------------------------
    // Conversions
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Converts the current instance of TypedValueStore to an ImmutableTypedValueStore.
    /// </summary>
    /// <returns>An instance of ImmutableTypedValueStore containing the current values of the TypedValueStore.</returns>
    public ImmutableTypedValueStore<TKey> ToImmutable() => new(_storage);

    /// <summary>
    ///     Converts the current TypedValueStore instance into a FrozenTypedValueStore instance.
    ///     This function creates a new instance of FrozenTypedValueStore with the same storage
    ///     content as the current TypedValueStore, ensuring that the data cannot be modified further.
    /// </summary>
    /// <returns>A new instance of FrozenTypedValueStore containing the same elements as the current TypedValueStore.</returns>
    public FrozenTypedValueStore<TKey> ToFrozen() => new(_storage);

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------

    /// <summary>
    ///     Tries to add a new key-value pair to the store.
    ///     If the key already exists, the operation fails.
    /// </summary>
    /// <param name="key">The key associated with the value to be added. Must not be null.</param>
    /// <param name="value">The value to be added, encapsulated in a ValueContainer. Must not be null.</param>
    /// <typeparam name="T">The type of the value being added. This type must be non-nullable.</typeparam>
    /// <returns>True if the key-value pair was successfully added; otherwise, false.</returns>
    public bool TryAdd<T>(TKey key, T value) where T : notnull => _storage
        .TryAdd(key, new ValueContainer<T>(value));

    /// <summary>
    ///     Adds a new value with the specified key to the store or updates it if the key already exists.
    /// </summary>
    /// <typeparam name="T">The type of the value to be stored. Must be a non-nullable type.</typeparam>
    /// <param name="key">The key associated with the value. Cannot be null.</param>
    /// <param name="value">The value to be added or updated in the store. Cannot be null.</param>
    public void AddOrUpdate<T>(TKey key, T value) where T : notnull => _storage
        .AddOrUpdate(key, new ValueContainer<T>(value), updateValueFactory: (_, _) => new ValueContainer<T>(value));

    /// <summary>
    ///     Attempts to retrieve a value of the specified type associated with the given key.
    /// </summary>
    /// <param name="key">The key whose value to retrieve from the store.</param>
    /// <param name="value">
    ///     When this method returns, contains the retrieved value associated with the specified key if the key is found and
    ///     the type matches; otherwise, the default value for the type T.
    ///     This parameter is passed uninitialized.
    /// </param>
    /// <typeparam name="T">The type of the value to retrieve, which must be non-nullable.</typeparam>
    /// <returns>
    ///     True if a value is found with the specified key and the type matches; otherwise, false.
    /// </returns>
    public bool TryGetValue<T>(TKey key, [NotNullWhen(true)] out T? value) where T : notnull {
        value = default;
        return _storage.TryGetValue(key, out IValueContainer? container)
            && container.TryGetAsValue(out value);
    }

    /// <summary>
    ///     Determines whether the store contains a specific key.
    /// </summary>
    /// <param name="key">The key to locate in the store.</param>
    /// <returns>True if the store contains an element with the specified key; otherwise, false.</returns>
    public bool ContainsKey(TKey key) => _storage.ContainsKey(key);

    /// <summary>
    ///     Attempts to remove and return the value associated with the specified key.
    ///     If the key is found and the value is removed, it returns true; otherwise, false.
    /// </summary>
    /// <param name="key">The key of the element to remove.</param>
    /// <returns>True if the element is successfully found and removed; otherwise, false.</returns>
    public bool TryRemove(TKey key) => _storage.TryRemove(key, out _);

    /// <summary>
    ///     Tries to remove a value associated with the specified key from the store.
    ///     If the value is found and successfully removed, it is returned.
    /// </summary>
    /// <typeparam name="T">The type of the value to be retrieved and removed.</typeparam>
    /// <param name="key">The key of the value to be removed.</param>
    /// <param name="value">
    ///     When this method returns, contains the value that was removed, if the key was found;
    ///     otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.
    /// </param>
    /// <returns>
    ///     true if the value was found and successfully removed; otherwise, false.
    /// </returns>
    public bool TryRemove<T>(TKey key, [NotNullWhen(true)] out T? value) where T : notnull {
        value = default;
        return _storage.TryGetValue(key, out IValueContainer? container)
            && container.TryGetAsValue(out value)
            && _storage.TryRemove(key, out _);
    }

    /// <summary>
    ///     Removes all items from the TypedValueStore, clearing its contents.
    /// </summary>
    public void Clear() => _storage.Clear();

    /// <summary>
    ///     Attempts to update the value associated with the specified key to a new value.
    /// </summary>
    /// <param name="key">
    ///     The key of the element to update. It must not be null.
    /// </param>
    /// <param name="newValue">
    ///     The new value to set for the specified key. It must not be null.
    /// </param>
    /// <typeparam name="T">
    ///     The type of the value to be updated. The type must be not null.
    /// </typeparam>
    /// <returns>
    ///     Returns true if the value was successfully updated; otherwise, false. The update will succeed only if
    ///     the key exists in the store and the existing and expected values match.
    /// </returns>
    public bool TryUpdate<T>(TKey key, T newValue) where T : notnull =>
        _storage.TryGetValue(key, out IValueContainer? container)
        && _storage.TryUpdate(key, new ValueContainer<T>(newValue), container);


    /// <summary>
    ///     Returns a hash code for the current instance of the TypedValueStore.
    /// </summary>
    /// <returns>A hash code representing the current TypedValueStore instance.</returns>
    public override int GetHashCode() => _storage.GetHashCode();

    #region Equals
    /// <summary>
    ///     Determines whether the specified TypedValueStore is equal to the current TypedValueStore.
    /// </summary>
    /// <param name="other">The TypedValueStore to compare with the current TypedValueStore.</param>
    /// <returns>true if the specified TypedValueStore is equal to the current TypedValueStore; otherwise, false.</returns>
    public bool Equals(TypedValueStore<TKey> other) {
        if (other._storage.Count != _storage.Count) return false;

        foreach (KeyValuePair<TKey, IValueContainer> kvp in _storage) {
            if (!other._storage.TryGetValue(kvp.Key, out IValueContainer? otherValue) ||
                !Equals(kvp.Value, otherValue)) {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    ///     Determines whether the current <see cref="TypedValueStore{TKey}" /> instance is equal to a specified
    ///     <see cref="ImmutableTypedValueStore{TKey}" />.
    /// </summary>
    /// <param name="other">The <see cref="ImmutableTypedValueStore{TKey}" /> to compare with the current instance.</param>
    /// <returns>
    ///     <c>true</c> if the current instance and the specified object have the same keys and associated values;
    ///     otherwise, <c>false</c>.
    /// </returns>
    public bool Equals(ImmutableTypedValueStore<TKey> other) {
        if (other.Storage.Count != _storage.Count) return false;

        foreach (KeyValuePair<TKey, IValueContainer> kvp in _storage) {
            if (!other.Storage.TryGetValue(kvp.Key, out IValueContainer? otherValue) ||
                !Equals(kvp.Value, otherValue)) {
                return false;
            }
        }

        return true;
    }


    /// <summary>
    ///     Determines whether the current instance is equal to a specified FrozenTypedValueStore instance.
    /// </summary>
    /// <param name="other">The FrozenTypedValueStore to compare with the current instance.</param>
    /// <returns>True if the current instance is equal to the specified FrozenTypedValueStore; otherwise, false.</returns>
    public bool Equals(FrozenTypedValueStore<TKey> other) {
        if (other.Storage.Count != _storage.Count) return false;

        foreach (KeyValuePair<TKey, IValueContainer> kvp in _storage) {
            if (!other.Storage.TryGetValue(kvp.Key, out IValueContainer? otherValue) ||
                !Equals(kvp.Value, otherValue)) {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    ///     Determines whether the specified object is equal to the current instance.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns>
    ///     true if the specified object is of the type TypedValueStore, ImmutableTypedValueStore, or FrozenTypedValueStore,
    ///     and is equal to the current instance;
    ///     otherwise, false.
    /// </returns>
    public override bool Equals(object? obj) =>
        obj switch {
            TypedValueStore<TKey> typedValueStore => Equals(typedValueStore),
            ImmutableTypedValueStore<TKey> immutableTypedValueStore => Equals(immutableTypedValueStore),
            FrozenTypedValueStore<TKey> frozenTypedValueStore => Equals(frozenTypedValueStore),
            _ => false
        };
    #endregion
}
