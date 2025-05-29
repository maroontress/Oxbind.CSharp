namespace Maroontress.Oxbind.Util;

using System;
using System.Collections.Concurrent;

/// <summary>
/// Provides a mechanism to obtain a canonical (interned) instance of a value
/// for a given key, typically used for caching or reducing memory footprint.
/// </summary>
/// <typeparam name="K">
/// The type of the key.
/// </typeparam>
/// <typeparam name="V">
/// The type of the value.
/// </typeparam>
/// <param name="initialCapacity">
/// The initial capacity.
/// </param>
/// <param name="concurrencyLevel">
/// The concurrency level.
/// </param>
public sealed class InternMap<K, V>(int initialCapacity, int concurrencyLevel)
    where K : notnull
    where V : class
{
    private const int DefaultCapacity = 31;

    /// <summary>
    /// Initializes a new instance of the <see cref="InternMap{K, V}"/> class.
    /// </summary>
    public InternMap()
        : this(DefaultCapacity, DefaultConcurrencyLevel)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InternMap{K, V}"/> class.
    /// </summary>
    /// <param name="initialCapacity">
    /// The initial capacity.
    /// </param>
    public InternMap(int initialCapacity)
        : this(initialCapacity, DefaultConcurrencyLevel)
    {
    }

    private static int DefaultConcurrencyLevel { get; }
        = Environment.ProcessorCount;

    /// <summary>
    /// Gets the map from a key to the value.
    /// </summary>
    private ConcurrentDictionary<K, V> Map { get; }
        = new(concurrencyLevel, initialCapacity);

    /// <summary>
    /// Gets the canonical value object corresponding to the specified key.
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <returns>
    /// The canonical value object associated with the specified key,
    /// or <c>null</c> if no value is found.
    /// </returns>
    public V? Get(K key)
    {
        return Map.TryGetValue(key, out var value)
            ? value
            : null;
    }

    /// <summary>
    /// Gets the canonical value object corresponding to the specified key. If
    /// a canonical value does not already exist in the map, it is created
    /// using the specified function.
    /// </summary>
    /// <remarks>
    /// If multiple threads call this method concurrently with the same key,
    /// the <paramref name="newValue"/> function might be invoked multiple
    /// times. However, only one canonical value object will be stored and
    /// returned for that key.
    /// </remarks>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <param name="newValue">
    /// The function that creates a new value object for the specified key.
    /// </param>
    /// <returns>
    /// The canonical value object.
    /// </returns>
    public V Intern(K key, Func<K, V> newValue)
    {
        return newValue is null
            ? throw new ArgumentNullException(nameof(newValue))
            : Map.GetOrAdd(key, newValue);
    }

    /// <summary>
    /// Gets the canonical value object corresponding to the specified key. If
    /// a canonical value does not already exist in the map, it is created
    /// using the specified supplier.
    /// </summary>
    /// <remarks>
    /// If multiple threads call this method concurrently with the same key,
    /// the <paramref name="supplier"/> function might be invoked multiple
    /// times. However, only a single canonical value object will be stored and
    /// returned for that key.
    /// </remarks>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <param name="supplier">
    /// The supplier that returns a new value object. This function is called
    /// if the key is not already present in the map.
    /// </param>
    /// <returns>
    /// The canonical value object.
    /// </returns>
    public V Intern(K key, Func<V> supplier)
    {
        return supplier is null
            ? throw new ArgumentNullException(nameof(supplier))
            : Map.GetOrAdd(key, k => supplier());
    }
}
