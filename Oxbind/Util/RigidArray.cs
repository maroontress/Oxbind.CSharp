namespace Maroontress.Oxbind.Util;

using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Represents an array that is immutable; meaning it cannot be changed once it
/// is created.
/// </summary>
/// <typeparam name="T">
/// The type of elements stored in the array.
/// </typeparam>
/// <param name="args">
/// The array to be copied.
/// </param>
public sealed class RigidArray<T>(T[] args) : IReadOnlyList<T>
{
    /// <summary>
    /// Gets the number of elements in the collection.
    /// </summary>
    /// <value>
    /// The number of elements in the collection.
    /// </value>
    public int Count => Elements.Length;

    /// <summary>
    /// Gets the back-end array that represents this.
    /// </summary>
    private T[] Elements { get; } = (args.Length == 0)
        ? []
        : (T[])args.Clone();

    /// <summary>
    /// Gets the element at the specified index in the read-only list.
    /// </summary>
    /// <param name="index">
    /// The zero-based index of the element to get.
    /// </param>
    /// <value>
    /// The element at the specified index in the read-only list.
    /// </value>
    public T this[int index] => Elements[index];

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>
    /// An enumerator that can be used to iterate through the collection.
    /// </returns>
    public IEnumerator<T> GetEnumerator()
    {
        var n = Elements.Length;
        for (var k = 0; k < n; ++k)
        {
            yield return Elements[k];
        }
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>
    /// An enumerator that can be used to iterate through the collection.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator()
        => Elements.GetEnumerator();
}
