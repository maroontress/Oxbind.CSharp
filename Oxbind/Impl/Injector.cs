namespace Maroontress.Oxbind.Impl;

/// <summary>
/// A delegate that injects a value into an array of constructor arguments.
/// </summary>
/// <param name="arguments">
/// The constructor arguments to inject the value into.
/// </param>
/// <param name="value">
/// The value to be injected.
/// </param>
public delegate void Injector(object[] arguments, object value);
