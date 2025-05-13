namespace Maroontress.Oxbind.Util.Graph;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Thrown when a circular dependency is detected in a directed acyclic graph
/// (DAG).
/// </summary>
/// <see cref="DagChecker{T}"/>
public sealed class CircularDependencyException
    : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see
    /// cref="CircularDependencyException"/> class.
    /// </summary>
    public CircularDependencyException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see
    /// cref="CircularDependencyException"/> class, with the specified detail
    /// message.
    /// </summary>
    /// <param name="message">
    /// A detail message.
    /// </param>
    public CircularDependencyException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see
    /// cref="CircularDependencyException"/> class.
    /// </summary>
    /// <param name="message">
    /// A detail message.
    /// </param>
    /// <param name="innerException">
    /// The cause.
    /// </param>
    public CircularDependencyException(
        string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="CircularDependencyException"/>
    /// class, with a message detailing the circular dependency based on the
    /// provided list of nodes.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the node.
    /// </typeparam>
    /// <param name="list">
    /// A list of nodes representing the cycle. The first node must equal the
    /// last node, and the <c>k</c>-th node depends on the (<c>k</c> + 1)-th
    /// node.
    /// </param>
    /// <returns>
    /// The new instance.
    /// </returns>
    public static CircularDependencyException Of<T>(IEnumerable<T> list)
        where T : notnull
    {
        return new CircularDependencyException(
            string.Join(" -> ", list.Select(o => o.ToString())));
    }
}
