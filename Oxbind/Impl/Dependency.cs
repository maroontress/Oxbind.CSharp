namespace Maroontress.Oxbind.Impl;

using System.Collections.Generic;

/// <summary>
/// Represents a dependency for XML binding that specifies whether an element
/// has inner text or child elements.
/// </summary>
/// <param name="HasInnerText">
/// A boolean value indicating whether the element can contain inner text. If
/// this is <c>true</c>, the <see cref="ChildParameters"/> collection must be
/// empty.
/// </param>
/// <param name="ChildParameters">
/// A collection of child parameters that define the expected child elements
/// and their binding behavior.
/// </param>
public record struct Dependency(
    bool HasInnerText,
    IEnumerable<ChildParameter> ChildParameters)
{
    /// <summary>
    /// Gets an empty dependency with no inner text and no child parameters.
    /// </summary>
    public static Dependency Empty => new(false, []);

    /// <summary>
    /// Gets a dependency that has inner text but no child parameters.
    /// </summary>
    public static Dependency InnerText => new(true, []);

    /// <summary>
    /// Creates a dependency with the specified child parameters and no inner
    /// text.
    /// </summary>
    /// <param name="childParameters">
    /// The collection of child parameters.
    /// </param>
    /// <returns>
    /// A new <see cref="Dependency"/> instance.
    /// </returns>
    public static Dependency Of(IEnumerable<ChildParameter> childParameters)
    {
        return new(false, childParameters);
    }
}
