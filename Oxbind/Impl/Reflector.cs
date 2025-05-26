namespace Maroontress.Oxbind.Impl;

using System;

/// <summary>
/// Encapsulates the logic for value injection, including type transformation.
/// </summary>
/// <typeparam name="T">
/// The type of the value to be injected.
/// </typeparam>
/// <param name="inject">
/// The delegate that injects the processed value into an array of constructor
/// arguments.
/// </param>
/// <param name="unitType">
/// The type of the unit.
/// </param>
/// <param name="sugarcoater">
/// The delegate that sugarcoats the specified value.
/// </param>
public sealed class Reflector<T>(
    Injector inject, Type unitType, Sugarcoater<T> sugarcoater)
{
    /// <summary>
    /// Gets the injector.
    /// </summary>
    public Injector Inject { get; } = inject;

    /// <summary>
    /// Gets the type of the unit.
    /// </summary>
    public Type UnitType { get; } = unitType;

    /// <summary>
    /// Gets the sugarcoater.
    /// </summary>
    public Sugarcoater<T> Sugarcoater { get; } = sugarcoater;
}
