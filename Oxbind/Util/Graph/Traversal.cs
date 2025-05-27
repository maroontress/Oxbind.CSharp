namespace Maroontress.Oxbind.Util.Graph;

using System;
using System.Collections.Generic;

/// <summary>
/// Provides a mechanism for graph traversal to visit all reachable nodes from
/// a starting node.
/// </summary>
/// <remarks>
/// For more information, see <a
/// href="https://en.wikipedia.org/wiki/Graph_traversal">
/// Graph traversal</a>:
///
/// <para>Unlike tree traversal, graph traversal may require that some nodes be
/// visited more than once, since it is not necessarily known before
/// transitioning to a node that it has already been explored. As graphs become
/// more dense, this redundancy becomes more prevalent, causing computation
/// time to increase; as graphs become more sparse, the opposite holds
/// true.</para>
///
/// <para>Thus, it is usually necessary to remember which nodes have already
/// been explored by the algorithm, so that nodes are revisited as infrequently
/// as possible (or in the worst case, to prevent the traversal from continuing
/// indefinitely). This may be accomplished by associating each node of the
/// graph with a "color" or "visitation" state during the traversal, which is
/// then checked and updated as the algorithm visits each node. If the node has
/// already been visited, it is ignored and the path is pursued no further;
/// otherwise, the algorithm checks/updates the node and continues down its
/// current path.</para>
/// </remarks>
/// <typeparam name="T">
/// The type of the node.
/// </typeparam>
/// <param name="getDependencies">
/// The function that returns the dependencies for the specified node.
/// </param>
public sealed class Traversal<T>(Func<T, IEnumerable<T>> getDependencies)
{
    /// <summary>
    /// Gets the set of nodes that have already been visited during the current
    /// traversal process.
    /// </summary>
    private ISet<T> VisitingSet { get; } = new HashSet<T>();

    /// <summary>
    /// Gets the function that returns the dependencies for the specified node.
    /// </summary>
    private Func<T, IEnumerable<T>> GetDependencies { get; } = getDependencies;

    /// <summary>
    /// Visits all the reachable nodes from the specified node.
    /// </summary>
    /// <param name="node">
    /// The starting node for the traversal.
    /// </param>
    public void Visit(T node)
    {
        lock (VisitingSet)
        {
            IfNotVisited([node]);
        }
    }

    /// <summary>
    /// Visits recursively all the specified nodes that have never been
    /// visited.
    /// </summary>
    /// <remarks>
    /// First, it calls the <see cref="GetDependencies"/> function for each
    /// unvisited node in the input. Then, it recursively calls itself with the
    /// dependencies of those nodes that had not been visited prior to this
    /// call.
    /// </remarks>
    /// <param name="all">
    /// The nodes to process in the current step of the traversal.
    /// </param>
    private void IfNotVisited(IEnumerable<T> all)
    {
        foreach (var node in all)
        {
            var dependencies = GetDependencies(node);
            if (VisitingSet.Add(node))
            {
                IfNotVisited(dependencies);
            }
        }
    }
}
