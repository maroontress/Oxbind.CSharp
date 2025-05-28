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
        var parameterIndex = info.Position;
        var type = info.ParameterType;
        var sugarcoater = StringSugarcoaters.Of(type);
        return new(parameterIndex, type, sugarcoater);
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
        var t = Doublet.Of(info.ParameterType);
        return Of(info.Position, t.ElementType, t.Sugarcoater);
    }

    private static Reflector<T> Of<T>(
        int parameterIndex, Type unitType, Sugarcoater<T> sugarcoater)
    {
        return new Reflector<T>(parameterIndex, unitType, sugarcoater);
    }
}
