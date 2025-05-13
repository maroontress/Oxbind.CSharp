namespace Maroontress.Oxbind.Impl;

using System;
using System.Reflection;

/// <summary>
/// Provides factory methods for <see cref="Reflector{T}"/> instances.
/// </summary>
public static class Reflectors
{
    /// <summary>
    /// Creates a new instance of the <see
    /// cref="Reflector{T}">Reflector&lt;string&gt;</see> class.
    /// </summary>
    /// <param name="info">
    /// The constructor parameter marked with the attribute <see
    /// cref="ForAttributeAttribute"/> or <see cref="ForTextAttribute"/>.
    /// The parameter type must be a <see cref="string"/> or <see
    /// cref="BindResult{T}">BindResult&lt;string&gt;</see>.
    /// </param>
    /// <returns>
    /// The new instance of the <see cref="Reflector{T}"/> class.
    /// </returns>
    public static Reflector<string> OfString(ParameterInfo info)
    {
        var injector = ToInjector(info);
        var type = info.ParameterType;
        var sugarcoater = StringSugarcoaters.Of(type);
        return new(injector, type, sugarcoater);
    }

    /// <summary>
    /// Creates a new instance of the <see
    /// cref="Reflector{T}">Reflector&lt;object&gt;</see> class.
    /// </summary>
    /// <param name="info">
    /// The constructor parameter marked with the attribute <see
    /// cref="RequiredAttribute"/>, <see cref="OptionalAttribute"/>, or <see
    /// cref="MultipleAttribute"/>.
    /// </param>
    /// <returns>
    /// The new instance of the <see cref="Reflector{T}"/> class.
    /// </returns>
    public static Reflector<object> Of(ParameterInfo info)
    {
        var injector = ToInjector(info);
        var t = Doublet.Of(info.ParameterType);
        return Of(injector, t.ElementType, t.Sugarcoater);
    }

    private static Reflector<T> Of<T>(
        Injector inject, Type unitType, Sugarcoater<T> sugarcoater)
    {
        return new Reflector<T>(inject, unitType, sugarcoater);
    }

    private static Injector ToInjector(ParameterInfo info)
    {
        var position = info.Position;
        return (a, v) => a[position] = v;
    }
}
