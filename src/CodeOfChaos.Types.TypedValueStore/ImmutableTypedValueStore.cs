// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace CodeOfChaos.Types;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents an immutable store that holds typed values associated with keys of type <typeparamref name="TKey" />.
/// </summary>
/// <typeparam name="TKey">The type of keys in the store. Must be a non-nullable type.</typeparam>
/// <remarks>
///     This struct provides a read-only collection that maps keys to values contained within an
///     <see cref="IValueContainer" />.
///     Values can be retrieved by key, with type-safety ensured at retrieval time.
/// </remarks>
public readonly struct ImmutableTypedValueStore<TKey>(IDictionary<TKey, IValueContainer>? storage = null) :
    IEnumerable
    where TKey : notnull {
    /// <summary>
    ///     Represents an immutable dictionary-like storage for typed values that implements a key-value pair structure.
    /// </summary>
    /// <remarks>
    ///     The <c>Storage</c> field provides the underlying storage for the <see cref="ImmutableTypedValueStore{TKey}" />.
    ///     It is initialized as an immutable dictionary, either from a provided dictionary or as an empty collection if no
    ///     initial data is supplied.
    /// </remarks>
    /// <typeparam name="TKey">The type of keys maintained by this store. Keys must be non-nullable.</typeparam>
    internal readonly ImmutableDictionary<TKey, IValueContainer> Storage = storage?.ToImmutableDictionary() ?? ImmutableDictionary<TKey, IValueContainer>.Empty;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Tries to retrieve a value associated with the specified key from the store.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve.</typeparam>
    /// <param name="key">The key whose value to get.</param>
    /// <param name="value">
    ///     When this method returns, contains the value associated with the specified key,
    ///     if the key is found; otherwise, the default value for the type of the value parameter.
    ///     This parameter is passed uninitialized.
    /// </param>
    /// <returns>
    ///     true if the store contains an element with the specified key and the value could be retrieved
    ///     as type T; otherwise, false.
    /// </returns>
    public bool TryGetValue<T>(TKey key, [NotNullWhen(true)] out T? value) where T : notnull {
        if (Storage.TryGetValue(key, out IValueContainer? container)) {
            return container.TryGetAsValue(out value);
        }

        value = default;
        return false;
    }

    /// <summary>
    ///     Determines whether the store contains a specific key.
    /// </summary>
    /// <param name="key">The key to locate in the store.</param>
    /// <returns>true if the store contains an element with the specified key; otherwise, false.</returns>
    public bool ContainsKey(TKey key) => Storage.ContainsKey(key);

    /// <summary>
    ///     Gets the number of key/value pairs contained in the <see cref="ImmutableTypedValueStore{TKey}" />.
    /// </summary>
    /// <remarks>
    ///     This property provides the total count of elements present in the underlying immutable dictionary storage.
    /// </remarks>
    public int Count => Storage.Count;
    /// <summary>
    ///     Returns an enumerator that iterates through the collection of key-value pairs.
    /// </summary>
    /// <returns>
    ///     An <see cref="IEnumerator" /> that can be used to iterate through the collection.
    /// </returns>
    public IEnumerator GetEnumerator() => Storage.GetEnumerator();
}
