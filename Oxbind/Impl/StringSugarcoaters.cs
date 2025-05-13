namespace Maroontress.Oxbind.Impl;

using System;
using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// Provides methods for <see cref="Sugarcoater{T}"/> (<c>T</c> is
/// <c>string</c>).
/// </summary>
public static class StringSugarcoaters
{
    private static readonly Dictionary<Type, Sugarcoater<string>> Map
        = NewMap();

    /// <summary>
    /// Checks if the specified type is <see cref="string"/> or <see
    /// cref="BindResult{T}">BindResult&lt;string&gt;</see>.
    /// </summary>
    /// <param name="type">
    /// The type to test.
    /// </param>
    /// <returns>
    /// <c>true</c> if the specified type is <see cref="string"/> or <see
    /// cref="BindResult{T}">BindResult&lt;string&gt;</see>; otherwise,
    /// <c>false</c>.
    /// </returns>
    public static bool IsValid(Type type)
        => Map.ContainsKey(type);

    /// <summary>
    /// Gets the sugarcoater for the specified type.
    /// </summary>
    /// <param name="type">
    /// The type, which must be <see cref="string"/> or <see
    /// cref="BindResult{T}">BindResult&lt;string&gt;</see>.
    /// </param>
    /// <returns>
    /// The sugarcoater for the specified type.
    /// </returns>
    public static Sugarcoater<string> Of(Type type)
    {
        if (!Map.TryGetValue(type, out var sugarcoater))
        {
            Debug.Fail($"{type}");
        }
        return sugarcoater;
    }

    private static Dictionary<Type, Sugarcoater<string>> NewMap()
        => new()
        {
            [Types.String] = (r, s) => s,
            [Types.BindResultString] = Readers.NewResult,
        };
}
