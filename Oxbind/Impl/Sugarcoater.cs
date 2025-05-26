namespace Maroontress.Oxbind.Impl;

using System.Xml;

/// <summary>
/// A delegate that transforms or wraps a raw deserialized value, often to
/// include metadata such as line information.
/// </summary>
/// <typeparam name="T">
/// The type of the value.
/// </typeparam>
/// <param name="info">
/// The location information.
/// </param>
/// <param name="value">
/// The raw value to process.
/// </param>
/// <returns>
/// The processed value, which may be the original value or a new object
/// wrapping it (e.g., a <see cref="BindResult{T}"/>).
/// </returns>
public delegate object Sugarcoater<in T>(IXmlLineInfo info, T value);
