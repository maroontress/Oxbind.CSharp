namespace Maroontress.Oxbind.Util;

using System;
using System.Collections.Concurrent;

/// <summary>
/// Provides the canonical value object corresponding to the key.
/// </summary>
/// <typeparam name="K">
/// The type of the key.
/// </typeparam>
/// <typeparam name="V">
/// The type of the value.
/// </typeparam>
public sealed class InternMap<K, V>
    where K : notnull
    where V : class
{
    private const int DefaultCapacity = 31;

    /// <summary>
    /// Initializes a new instance of the <see cref="InternMap{K, V}"/>
    /// class.
    /// </summary>
    public InternMap()
        : this(DefaultCapacity, DefaultConcurrencyLevel)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InternMap{K, V}"/>
    /// class.
    /// </summary>
    /// <param name="initialCapacity">
    /// The initial capacity.
    /// </param>
    public InternMap(int initialCapacity)
        : this(initialCapacity, DefaultConcurrencyLevel)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InternMap{K, V}"/>
    /// class.
    /// </summary>
    /// <param name="initialCapacity">
    /// The initial capacity.
    /// </param>
    /// <param name="concurrencyLevel">
    /// The concurrency level.
    /// </param>
    public InternMap(int initialCapacity, int concurrencyLevel)
    {
        Map = new(concurrencyLevel, initialCapacity);
    }

    private static int DefaultConcurrencyLevel { get; }
        = Environment.ProcessorCount;

    /// <summary>
    /// Gets the map from a key to the value.
    /// </summary>
    private ConcurrentDictionary<K, V> Map { get; }

    /// <summary>
    /// Gets the canonical value object corresponding to the specified key
    /// object. If the canonical value object does not exist in the
    /// internal object pool, creates a new value object with the specified
    /// function.
    /// </summary>
    /// <remarks>
    /// The specified function can be called concurrently with the equal
    /// keys if the multiple threads call this method. However, the
    /// canonical value object that this method returns is only one.
    /// </remarks>
    /// <param name="key">
    /// The key object.
    /// </param>
    /// <param name="newValue">
    /// The function that returns a new value object corresponding to the
    /// specified argument.
    /// </param>
    /// <returns>
    /// The canonical value object.
    /// </returns>
    public V Intern(K key, Func<K, V> newValue)
    {
        if (newValue is null)
        {
            throw new ArgumentNullException(nameof(newValue));
        }
        return Map.GetOrAdd(key, newValue);
    }

    /// <summary>
    /// Gets the canonical value object corresponding to the specified key
    /// object. If the canonical value object does not exist in the
    /// internal object pool, creates a new value object with the specified
    /// supplier.
    /// </summary>
    /// <remarks>
    /// The specified function can be called concurrently with the equal
    /// keys if the multiple threads call this method. However, the
    /// canonical value object that this method returns is only one.
    /// </remarks>
    /// <param name="key">
    /// The key object.
    /// </param>
    /// <param name="supplier">
    /// The supplier that returns a new value object corresponding to the
    /// specified key object.
    /// </param>
    /// <returns>
    /// The canonical value object.
    /// </returns>
    public V Intern(K key, Func<V> supplier)
    {
        if (supplier is null)
        {
            throw new ArgumentNullException(nameof(supplier));
        }
        return Map.GetOrAdd(key, k => supplier());
    }
}
