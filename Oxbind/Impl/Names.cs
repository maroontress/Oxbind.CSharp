namespace Maroontress.Oxbind.Impl;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

/// <summary>
/// Provides helper methods to format type and parameter names.
/// </summary>
public static class Names
{
    /// <summary>
    /// The initial capacity of the <see cref="StringBuilder"/>.
    /// </summary>
    private const int InitialCapacity = 80;

    /// <summary>
    /// The common delimiter between names.
    /// </summary>
    private const string Delimiter = ", ";

    /// <summary>
    /// Returns the joined string of the specified strings with the default
    /// delimiter.
    /// </summary>
    /// <param name="all">
    /// The strings.
    /// </param>
    /// <returns>
    /// The joined string.
    /// </returns>
    public static string Join(IEnumerable<string> all)
    {
        return string.Join(Delimiter, all);
    }

    /// <summary>
    /// Sorts the specified strings and returns a string joining them with the
    /// default delimiter.
    /// </summary>
    /// <param name="all">
    /// The strings.
    /// </param>
    /// <returns>
    /// The sorted and joined string.
    /// </returns>
    public static string SortAndJoin(IEnumerable<string> all)
    {
        return Join(all.OrderBy(s => s));
    }

    /// <summary>
    /// Returns the joined string of the parameter names with the default
    /// delimiter.
    /// </summary>
    /// <param name="all">
    /// The parameters.
    /// </param>
    /// <returns>
    /// A comma-separated string of parameter names, sorted alphabetically.
    /// </returns>
    public static string OfParameters(IEnumerable<ParameterInfo> all)
    {
        return JoinedNames(all, p => p.Name ?? "(no name)");
    }

    /// <summary>
    /// Sorts the names of the specified objects and returns a string joining
    /// them with the default delimiter.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the objects.
    /// </typeparam>
    /// <param name="all">
    /// The objects.
    /// </param>
    /// <param name="getName">
    /// The function that returns the name of its argument.
    /// </param>
    /// <returns>
    /// A comma-separated string of names, obtained by the <paramref
    /// name="getName"/> function and sorted alphabetically.
    /// </returns>
    private static string JoinedNames<T>(
        IEnumerable<T> all, Func<T, string> getName)
    {
        return Join(all.Select(m => getName(m))
            .OrderBy(s => s));
    }
}
