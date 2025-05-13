namespace Maroontress.Oxbind.Impl;

using System.Xml;

/// <summary>
/// The default implementation of the <see cref="BindResult{T}"/> interface.
/// </summary>
/// <typeparam name="T">
/// The type of the deserialized value.
/// </typeparam>
/// <param name="value">
/// The deserialized value.
/// </param>
/// <param name="info">
/// The location in the XML document where this result occurred.
/// </param>
public sealed class BindResultImpl<T>(T value, IXmlLineInfo info)
    : BindResult<T>
    where T : class
{
    /// <inheritdoc/>
    public T Value { get; } = value;

    /// <inheritdoc/>
    public int Line { get; } = info.LineNumber;

    /// <inheritdoc/>
    public int Column { get; } = info.LinePosition;
}
