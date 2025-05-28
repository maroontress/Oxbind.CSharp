namespace Maroontress.Oxbind.Util.Graph;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A checker for directed acyclic graphs (DAGs). A DAG is a directed graph
/// with no directed cycles.
/// </summary>
/// <remarks>
/// For more information about DAG, see <a
/// href="https://en.wikipedia.org/wiki/Directed_acyclic_graph">
/// Directed acyclic graph</a>.
/// </remarks>
/// <typeparam name="T">
/// The type of the nodes in the graph.
/// </typeparam>
/// <param name="getDependencies">
/// The function that returns the dependencies of the specified node.
/// </param>
/// <param name="toNodeName">
/// The function that converts a node to its string representation.
/// </param>
/// <param name="checkedSet">
/// The set of nodes already confirmed to be free of circular dependencies.
/// Note that the <see cref="Check(T)"/> method may add new nodes to the
/// specified node set.
/// </param>
public sealed class DagChecker<T>(
    Func<T, ISet<T>> getDependencies,
    Func<T, string> toNodeName,
    ISet<T> checkedSet)
    where T : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DagChecker{T}"/> class.
    /// </summary>
    /// <param name="getDependencies">
    /// The function that returns the dependencies of the specified node.
    /// </param>
    /// <param name="toNodeName">
    /// The function that converts a node to its string representation.
    /// </param>
    public DagChecker(
        Func<T, ISet<T>> getDependencies,
        Func<T, string> toNodeName)
        : this(getDependencies, toNodeName, new HashSet<T>())
    {
    }

    /// <summary>
    /// Gets a set of nodes that have already been checked and confirmed to be
    /// free of circular dependencies.
    /// </summary>
    private ISet<T> CheckedSet { get; } = checkedSet;

    /// <summary>
    /// Gets the function that returns the dependencies of the specified node.
    /// </summary>
    private Func<T, ISet<T>> GetDependencies { get; } = getDependencies;

    /// <summary>
    /// Gets a set used to track the current path during cycle detection
    /// (acting as breadcrumbs).
    /// </summary>
    private LinkedHashSet<T> Breadcrumbs { get; } = [];

    private Func<T, string> ToNodeName { get; } = toNodeName;

    /// <summary>
    /// Recursively checks if the specified node or its dependencies form a
    /// circular dependency.
    /// </summary>
    /// <param name="node">
    /// The node to start checking from.
    /// </param>
    /// <exception cref="CircularDependencyException">
    /// Thrown when a circular dependency is detected.
    /// </exception>
    public void Check(T node)
    {
        if (CheckedSet.Contains(node))
        {
            return;
        }
        if (!Breadcrumbs.Add(node))
        {
            var list = new List<T>(Breadcrumbs)
            {
                node,
            };
            var index = list.IndexOf(node);
            var loop = list.GetRange(index, list.Count - index);
            throw CircularDependencyException.Of(loop.Select(ToNodeName));
        }
        try
        {
            var dependencies = GetDependencies(node);
            foreach (var d in dependencies)
            {
                Check(d);
            }
            CheckedSet.Add(node);
        }
        finally
        {
            Breadcrumbs.Remove(node);
        }
    }
}
