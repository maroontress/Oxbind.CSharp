namespace Maroontress.Oxbind.Impl;

using System;

/// <summary>
/// Encapsulates the logic for value injection, including type transformation.
/// </summary>
/// <typeparam name="T">
/// The type of the value to be injected.
/// </typeparam>
/// <param name="parameterIndex">
/// The parameter index in the constructor where the value is injected.
/// </param>
/// <param name="unitType">
/// The type of the unit.
/// </param>
/// <param name="sugarcoater">
/// The delegate that sugarcoats the specified value.
/// </param>
public sealed class Reflector<T>(
    int parameterIndex, Type unitType, Sugarcoater<T> sugarcoater)
{
    /// <summary>
    /// Gets the parameter index.
    /// </summary>
    public int ParameterIndex { get; } = parameterIndex;

    /// <summary>
    /// Gets the type of the unit.
    /// </summary>
    public Type UnitType { get; } = unitType;

    /// <summary>
    /// Gets the sugarcoater.
    /// </summary>
    public Sugarcoater<T> Sugarcoater { get; } = sugarcoater;

    /// <summary>
    /// Injects a value into an array of constructor arguments.
    /// </summary>
    /// <param name="arguments">
    /// The constructor arguments to inject the value into.
    /// </param>
    /// <param name="value">
    /// The value to be injected.
    /// </param>
    public void Inject(object?[] arguments, object value)
        => arguments[ParameterIndex] = value;
}
