namespace Maroontress.Oxbind.Impl;

using System;

/// <summary>
/// Encapsulates the element type and the value transformation logic
/// (sugarcoater) for a constructor parameter representing a child XML element.
/// </summary>
/// <remarks>
/// This is used to determine the actual type of the object to be created for a
/// child element and how to wrap it (e.g., with <see cref="BindResult{T}"/>)
/// if necessary.
/// </remarks>
public readonly struct Doublet
{
    private static readonly Sugarcoater<object> ToBindResult
        = Readers.NewResultObject;

    private static readonly Sugarcoater<object> PassThrough
        = (r, v) => v;

    private Doublet(
        Type elementType,
        Sugarcoater<object> sugarcoater)
    {
        ElementType = elementType;
        Sugarcoater = sugarcoater;
    }

    /// <summary>
    /// Gets the element type (e.g., <c>T</c> or <see cref="BindResult{T}"/>).
    /// </summary>
    public Type ElementType { get; }

    /// <summary>
    /// Gets the sugarcoater.
    /// </summary>
    public Sugarcoater<object> Sugarcoater { get; }

    /// <summary>
    /// Creates a new <see cref="Doublet"/> for the specified constructor
    /// parameter type.
    /// </summary>
    /// <param name="type">
    /// The type.
    /// </param>
    /// <returns>
    /// The new triplet.
    /// </returns>
    public static Doublet Of(Type type)
    {
        static Doublet Dispatch(Type u)
            => !Types.IsRawType(u, Types.BindResultT)
                ? new(u, PassThrough)
                : new(u, ToBindResult);

        return !Types.IsRawType(type, Types.IEnumerableT)
            ? Dispatch(type)
            : Dispatch(Types.FirstInnerType(type));
    }
}
/*
| Type of value                | ElementType     | Sugarcoater  |
| :---                         | :---            | :---         |
| `T`                          | `T`             | PassThrough  |
| `BindResult<T>`              | `BindResult<T>` | ToBindResult |
| `IEnumerable<T>`             | `T`             | PassThrough  |
| `IEnumerable<BindResult<T>>` | `BindResult<T>` | ToBindResult |
*/
