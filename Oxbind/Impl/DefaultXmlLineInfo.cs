namespace Maroontress.Oxbind.Impl;

using System.Xml;

/// <summary>
/// The default implementation of the <see cref="IXmlLineInfo"/> interface.
/// </summary>
/// <param name="lineNumber">
/// The 1-based line number in the XML document.
/// </param>
/// <param name="linePosition">
/// The 1-based line position (column) in the XML document.
/// </param>
/// <param name="hasLineInfo">
/// Indicates whether the line information is available.
/// </param>
public sealed class DefaultXmlLineInfo(
    int lineNumber, int linePosition, bool hasLineInfo)
    : IXmlLineInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultXmlLineInfo"/>
    /// class with the specified <see cref="IXmlLineInfo"/>.
    /// </summary>
    /// <param name="info">
    /// The location information to be copied.
    /// </param>
    public DefaultXmlLineInfo(IXmlLineInfo info)
        : this(info.LineNumber, info.LinePosition, info.HasLineInfo())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultXmlLineInfo"/>
    /// class, representing no line information.
    /// </summary>
    private DefaultXmlLineInfo()
        : this(0, 0, false)
    {
    }

    /// <summary>
    /// Gets the <see cref="IXmlLineInfo"/> representing no line information.
    /// </summary>
    public static IXmlLineInfo NoLineInfo { get; } = new DefaultXmlLineInfo();

    /// <inheritdoc/>
    public int LineNumber { get; } = lineNumber;

    /// <inheritdoc/>
    public int LinePosition { get; } = linePosition;

    private bool LineInfo { get; } = hasLineInfo;

    /// <inheritdoc/>
    public bool HasLineInfo() => LineInfo;
}
