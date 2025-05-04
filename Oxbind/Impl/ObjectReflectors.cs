namespace Maroontress.Oxbind.Impl;

using System;

/// <summary>
/// Provides methods for <see cref="Reflector{T}"/> (<c>T</c> is
/// <c>object</c>).
/// </summary>
public static class ObjectReflectors
{
    /// <summary>
    /// Creates a new <see cref="Reflector{T}"/> (<c>T</c> is
    /// <c>object</c>) and perform the specified action
    /// with the placeholder type and the new reflector
    /// associated with the specified type and injector.
    /// </summary>
    /// <param name="type">
    /// The type of the value to be injected.
    /// </param>
    /// <param name="injector">
    /// The injector that injects the value to the field or with the
    /// method.
    /// </param>
    /// <param name="action">
    /// The action that consumes two parameters,
    /// the one is the placeholder type,
    /// the other is the reflector object.
    /// </param>
    public static void Associate(
        Type type,
        Injector injector,
        Action<Type, Reflector<object>> action)
    {
        var t = Triplet.Of(type);
        var reflector = Reflectors.Of(injector, t.UnitType, t.Sugarcoater);
        action(t.PlaceHolderType, reflector);
    }
}
