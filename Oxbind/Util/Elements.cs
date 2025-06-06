namespace Maroontress.Oxbind.Util;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A utility class for common operations on collections (primarily <see
/// cref="IEnumerable{T}"/>) and iteration.
/// </summary>
public static class Elements
{
    /// <summary>
    /// Accepts the specified consumer function if the specified collection is
    /// not empty.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the collection's element.
    /// </typeparam>
    /// <param name="all">
    /// The collection.
    /// </param>
    /// <param name="consumer">
    /// The consumer function to apply when the collection is not empty.
    /// </param>
    public static void IfNotEmpty<T>(
        IEnumerable<T> all, Action<IEnumerable<T>> consumer)
    {
        if (!all.Any())
        {
            return;
        }
        consumer(all);
    }

    /// <summary>
    /// Returns the relative complement of B in A (also called the
    /// set-theoretic difference of A and B), denoted by A \ B (or A &#x2212;
    /// B). This is the set of all elements that are members of A but not
    /// members of B.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the element.
    /// </typeparam>
    /// <param name="a">
    /// The collection A.
    /// </param>
    /// <param name="b">
    /// The collection B.
    /// </param>
    /// <returns>
    /// The new <see cref="ISet{T}"/> object that contains the relative
    /// complement of B in A.
    /// </returns>
    public static ISet<T> DifferenceOf<T>(
        IEnumerable<T> a, IEnumerable<T> b)
    {
        var set = new HashSet<T>(b);
        return new HashSet<T>(a.Where(e => !set.Contains(e)));
    }
}
