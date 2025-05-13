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
public sealed class DagChecker<T>
    where T : notnull
{
    /// <summary>
    /// A set used to track the current path during cycle detection (acting as
    /// breadcrumbs).
    /// </summary>
    private readonly LinkedHashSet<T> set = [];

    /// <summary>
    /// A set of nodes that have already been checked and confirmed to be free
    /// of circular dependencies.
    /// </summary>
    private readonly ISet<T> checkedSet;

    /// <summary>
    /// The function that returns the dependencies of the specified node.
    /// </summary>
    private readonly Func<T, ISet<T>> getDependencies;

    /// <summary>
    /// Initializes a new instance of the <see cref="DagChecker{T}"/> class.
    /// </summary>
    /// <param name="getDependencies">
    /// The function that returns the dependencies of the specified node.
    /// </param>
    public DagChecker(Func<T, ISet<T>> getDependencies)
    {
        this.getDependencies = getDependencies;
        checkedSet = new HashSet<T>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DagChecker{T}"/> class,
    /// with the specified node set containing the nodes that have no circular
    /// dependencies.
    /// </summary>
    /// <remarks>
    /// The <see cref="Check(T)"/> method may add the nodes to the specified
    /// node set.
    /// </remarks>
    /// <param name="getDependencies">
    /// The function that returns the dependencies of the specified node.
    /// </param>
    /// <param name="checkedSet">
    /// The node set containing the nodes that have no circular dependencies.
    /// </param>
    public DagChecker(Func<T, ISet<T>> getDependencies, ISet<T> checkedSet)
    {
        this.getDependencies = getDependencies;
        this.checkedSet = checkedSet;
    }

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
        if (!set.Add(node))
        {
            var list = new List<T>(set)
            {
                node,
            };
            var index = list.IndexOf(node);
            var loop = list.GetRange(index, list.Count - index);
            throw CircularDependencyException.Of(loop);
        }
        try
        {
            var dependencies = getDependencies(node)
                .Where(d => !checkedSet.Contains(d));
            foreach (var d in dependencies)
            {
                Check(d);
            }
            checkedSet.Add(node);
        }
        finally
        {
            set.Remove(node);
        }
    }
}
