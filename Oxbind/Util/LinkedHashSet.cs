namespace Maroontress.Oxbind.Util;

using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Hash table and linked list implementation of the <see cref="ISet{T}"/>
/// interface, with predictable iteration order.
/// </summary>
/// <remarks>
/// This implementation differs from <see cref="HashSet{T}"/> in that it
/// maintains a doubly-linked list running through all of its entries. This
/// linked list defines the iteration ordering, which is the order in which
/// elements were inserted into the set (insertion-order). Note that insertion
/// order is not affected if an element is re-inserted into the set. (An
/// element <c>e</c> is reinserted into a set <c>s</c> if <c>s.Add(e)</c> is
/// invoked when <c>s.Contains(e)</c> would return <c>true</c> immediately
/// prior to the invocation.)
/// </remarks>
/// <typeparam name="T">
/// The type of elements maintained by this set.
/// </typeparam>
public sealed class LinkedHashSet<T> : ISet<T>
    where T : notnull
{
    private Func<ISet<T>> keySet;

    /// <summary>
    /// Initializes a new instance of the <see cref="LinkedHashSet{T}"/> class.
    /// </summary>
    public LinkedHashSet()
        : this(16)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinkedHashSet{T}"/> class.
    /// </summary>
    /// <param name="initialCapacity">
    /// The initial capacity.
    /// </param>
    public LinkedHashSet(int initialCapacity)
    {
        if (initialCapacity < 0)
        {
            throw new ArgumentException(
                $"Illegal initial capacity: {initialCapacity}");
        }
        Map = new(initialCapacity);
        NewKeySet = () =>
        {
            var set = new HashSet<T>(Map.Keys);
            keySet = () => set;
            return set;
        };
        keySet = NewKeySet;
    }

    /// <inheritdoc/>
    public int Count => List.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    private LinkedList<T> List { get; } = new();

    private Dictionary<T, LinkedListNode<T>> Map { get; set; }

    private Func<ISet<T>> NewKeySet { get; }

    /// <inheritdoc/>
    public bool Add(T item)
    {
        if (Map.ContainsKey(item))
        {
            return false;
        }
        var listNode = List.AddLast(item);
        Map[item] = listNode;
        keySet = NewKeySet;
        return true;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        Map.Clear();
        List.Clear();
        keySet = NewKeySet;
    }

    /// <inheritdoc/>
    public bool Contains(T item) => Map.ContainsKey(item);

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex)
    {
        if (array is null)
        {
            throw new ArgumentNullException(nameof(array));
        }
        if (arrayIndex < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(arrayIndex),
                $"Illegal array index: {arrayIndex}");
        }
        if (arrayIndex > array.Length
            || array.Length < Count
            || arrayIndex > array.Length - Count)
        {
            throw new ArgumentException(
                "Destination array is not long enough");
        }
        List.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public void ExceptWith(IEnumerable<T> other)
    {
        if (other is null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        foreach (var e in other)
        {
            Remove(e);
        }
        keySet = NewKeySet;
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => List.GetEnumerator();

    /// <inheritdoc/>
    public void IntersectWith(IEnumerable<T> other)
    {
        if (other is null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        var newMap = new Dictionary<T, LinkedListNode<T>>();
        foreach (var e in other)
        {
            if (!Map.TryGetValue(e, out var node))
            {
                continue;
            }
            Map.Remove(e);
            newMap[e] = node;
        }
        foreach (var v in Map.Values)
        {
            List.Remove(v);
        }
        Map = newMap;
        keySet = NewKeySet;
    }

    /// <inheritdoc/>
    public bool IsProperSubsetOf(IEnumerable<T> other)
        => keySet().IsProperSubsetOf(other);

    /// <inheritdoc/>
    public bool IsProperSupersetOf(IEnumerable<T> other)
        => keySet().IsProperSupersetOf(other);

    /// <inheritdoc/>
    public bool IsSubsetOf(IEnumerable<T> other)
        => keySet().IsSubsetOf(other);

    /// <inheritdoc/>
    public bool IsSupersetOf(IEnumerable<T> other)
        => keySet().IsSupersetOf(other);

    /// <inheritdoc/>
    public bool Overlaps(IEnumerable<T> other)
        => keySet().Overlaps(other);

    /// <inheritdoc/>
    public bool Remove(T item)
    {
        if (!Map.TryGetValue(item, out var listNode))
        {
            return false;
        }
        List.Remove(listNode);
        Map.Remove(item);
        keySet = NewKeySet;
        return true;
    }

    /// <inheritdoc/>
    public bool SetEquals(IEnumerable<T> other)
        => keySet().SetEquals(other);

    /// <inheritdoc/>
    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        if (other is null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        if (ReferenceEquals(other, this))
        {
            Clear();
            return;
        }
        var set = new HashSet<T>();
        foreach (var e in other)
        {
            if (!set.Add(e))
            {
                continue;
            }
            if (!Remove(e))
            {
                Add(e);
            }
        }
        keySet = NewKeySet;
    }

    /// <inheritdoc/>
    public void UnionWith(IEnumerable<T> other)
    {
        if (other is null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        foreach (var e in other)
        {
            Add(e);
        }
        keySet = NewKeySet;
    }

    /// <inheritdoc/>
    void ICollection<T>.Add(T item) => Add(item);

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
