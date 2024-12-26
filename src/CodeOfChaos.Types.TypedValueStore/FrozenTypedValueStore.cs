// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;

namespace CodeOfChaos.Types;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents a read-only collection of key-value pairs where each key is of type <typeparamref name="TKey" /> and
///     each value is encapsulated within an <see cref="IValueContainer" />. The collection is stored in a frozen
///     dictionary,
///     providing efficient retrieval and enumeration performance.
/// </summary>
/// <typeparam name="TKey">The type of the keys in the store. Must be a non-nullable type.</typeparam>
/// <remarks>
///     The <see cref="FrozenTypedValueStore{TKey}" /> is intended for scenarios where the collection is constructed once
///     and then used for efficient read-only access. It leverages a frozen dictionary for performance benefits and thus
///     does not allow modifications such as adding or removing items after its creation.
/// </remarks>
public readonly struct FrozenTypedValueStore<TKey>(IDictionary<TKey, IValueContainer>? storage = null)
    : IEnumerable
    where TKey : notnull {
    /// <summary>
    ///     Represents a frozen dictionary that stores key-value pairs where the key is of generic type
    ///     <typeparamref name="TKey" />
    ///     and the value is encapsulated within an <see cref="IValueContainer" />. This storage is immutable and optimized for
    ///     read-heavy scenarios. Once constructed, its contents cannot be modified.
    /// </summary>
    internal readonly FrozenDictionary<TKey, IValueContainer> Storage = storage?.ToFrozenDictionary() ?? FrozenDictionary<TKey, IValueContainer>.Empty;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// Tries to retrieve a value of type
    /// <typeparamref name="T" />
    /// associated with the specified key.
    /// <typeparam name="T">The type of the value to retrieve.</typeparam>
    /// <param name="key">The key whose value to retrieve from the store.</param>
    /// <param name="value">
    ///     When this method returns, contains the value associated with the specified key,
    ///     if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed
    ///     uninitialized.
    /// </param>
    /// <returns>
    ///     true if the store contains an element with the specified key and a value of type <typeparamref name="T" /> can be
    ///     retrieved;
    ///     otherwise, false.
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
    /// <returns>
    ///     <c>true</c> if the store contains an element with the specified key; otherwise, <c>false</c>.
    /// </returns>
    public bool ContainsKey(TKey key) => Storage.ContainsKey(key);

    /// <summary>
    ///     Gets the number of elements contained in the FrozenTypedValueStore.
    /// </summary>
    /// <value>
    ///     The total count of elements that exist within the storage of the FrozenTypedValueStore instance.
    /// </value>
    public int Count => Storage.Count;

    /// <summary>
    ///     Returns an enumerator that iterates through the frozen dictionary of key-value pairs contained in this store.
    /// </summary>
    /// <returns>
    ///     An <see cref="IEnumerator" /> for the frozen dictionary.
    /// </returns>
    public IEnumerator GetEnumerator() => Storage.GetEnumerator();
}
