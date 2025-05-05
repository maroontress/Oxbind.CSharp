namespace Maroontress.Oxbind.Impl;

using System;

/// <summary>
/// Capsulizes each step to realize the injection.
/// </summary>
/// <typeparam name="T">
/// The type of the value to be injected.
/// </typeparam>
/// <param name="inject">
/// The delegate that injects the sugarcoated value to the field or with
/// the method.
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
