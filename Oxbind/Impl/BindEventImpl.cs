namespace Maroontress.Oxbind.Impl;

using System.Xml;

/// <summary>
/// The default implementation of the <see cref="BindEvent{T}"/> interface.
/// </summary>
/// <typeparam name="T">
/// The type of the deserialized event.
/// </typeparam>
/// <param name="value">
/// The deserialized value.
/// </param>
/// <param name="info">
/// The location information of the XML document.
/// </param>
public sealed class BindEventImpl<T>(T value, IXmlLineInfo info) : BindEvent<T>
    where T : class
{
    /// <inheritdoc/>
    public T Value { get; } = value;

    /// <inheritdoc/>
    public int Line { get; } = info.LineNumber;

    /// <inheritdoc/>
    public int Column { get; } = info.LinePosition;
}
