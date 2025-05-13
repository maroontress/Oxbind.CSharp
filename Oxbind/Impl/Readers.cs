namespace Maroontress.Oxbind.Impl;

using System;
using System.Xml;

/// <summary>
/// Provides utility methods for reading and validating XML content using an
/// <see cref="XmlReader"/>.
/// </summary>
public static class Readers
{
    /// <summary>
    /// Ensures that more parsing events are available in the reader before
    /// attempting to read further. Throws a <see cref="BindException"/> if the
    /// end of the stream is reached unexpectedly.
    /// </summary>
    /// <param name="in">
    /// The XML reader.
    /// </param>
    public static void ConfirmNext(XmlReader @in)
    {
        if (!@in.EOF)
        {
            return;
        }
        throw new BindException(
            "unexpected end of stream.", AsXmlLineInfo(@in));
    }

    /// <summary>
    /// Skips any trailing character data (text, whitespace) and confirms that
    /// the reader is at the end of the stream. Throws a <see
    /// cref="BindException"/> if additional content is found beyond the
    /// expected end.
    /// </summary>
    /// <param name="in">
    /// The XML reader.
    /// </param>
    public static void ConfirmEndOfStream(XmlReader @in)
    {
        while (IsCharacters(@in.NodeType))
        {
            @in.Read();
        }
        if (@in.EOF)
        {
            return;
        }
        throw new BindException(
            "expected end of stream", AsXmlLineInfo(@in));
    }

    /// <summary>
    /// Confirms that the current node in the XML reader is a start element
    /// matching the specified qualified name. Throws a <see
    /// cref="BindException"/> otherwise.
    /// </summary>
    /// <param name="in">
    /// The XML reader.
    /// </param>
    /// <param name="expectedName">
    /// The expected qualified name of the element.
    /// </param>
    public static void ConfirmStartElement(
        XmlReader @in, XmlQualifiedName expectedName)
    {
        ConfirmElement(@in, expectedName, XmlNodeType.Element, "start");
    }

    /// <summary>
    /// Confirms that the current node in the XML reader is an end element
    /// matching the specified qualified name. Throws a <see
    /// cref="BindException"/> otherwise.
    /// </summary>
    /// <param name="in">
    /// The XML reader.
    /// </param>
    /// <param name="expectedName">
    /// The expected qualified name of the element.
    /// </param>
    public static void ConfirmEndElement(
        XmlReader @in, XmlQualifiedName expectedName)
    {
        ConfirmElement(@in, expectedName, XmlNodeType.EndElement, "end");
    }

    /// <summary>
    /// Skips any character data (text, whitespace, CDATA sections) in the
    /// specified XML reader.
    /// </summary>
    /// <param name="in">
    /// The XML reader.
    /// </param>
    /// <returns>
    /// The <see cref="XmlNodeType"/> of the current node after skipping any
    /// character data.
    /// </returns>
    public static XmlNodeType SkipCharacters(XmlReader @in)
    {
        for (;;)
        {
            ConfirmNext(@in);
            var nodeType = @in.NodeType;
            if (!IsCharacters(nodeType))
            {
                return nodeType;
            }
            @in.Read();
        }
    }

    /// <summary>
    /// Returns a new <see cref="BindResult{T}"/> object with the specified
    /// value and the specified location information.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the result value.
    /// </typeparam>
    /// <param name="info">
    /// The location information.
    /// </param>
    /// <param name="value">
    /// The value of the result.
    /// </param>
    /// <returns>
    /// The new bind result.
    /// </returns>
    public static BindResult<T> NewResult<T>(IXmlLineInfo info, T value)
        where T : class
        => new BindResultImpl<T>(value, info);

    /// <summary>
    /// Returns a new <see cref="BindResult{T}"/> object with the specified
    /// value and the specified location information.
    /// </summary>
    /// <param name="info">
    /// The location information.
    /// </param>
    /// <param name="value">
    /// The value of the result.
    /// </param>
    /// <returns>
    /// The new bind result.
    /// </returns>
    public static object NewResultObject(IXmlLineInfo info, object value)
    {
        var type = value.GetType();
        var resultType = Types.BindResultImplT.MakeGenericType(type);
        return Activator.CreateInstance(resultType, value, info)
            /*
                Activator.CreateInstance() returns 'null' only if the return
                type is a 'Nullable<T>' and it represents 'null'. This should
                not happen here as T is constrained to 'class'.
            */
            ?? throw new NullReferenceException(
                "unexpected type (maybe Nullable<T>)");
    }

    /// <summary>
    /// Creates a new <see cref="IXmlLineInfo"/> object from the specified XML
    /// reader.
    /// </summary>
    /// <param name="in">
    /// The XML reader.
    /// </param>
    /// <returns>
    /// The <see cref="IXmlLineInfo"/> object which is immutable.
    /// </returns>
    public static IXmlLineInfo ToXmlLineInfo(XmlReader @in)
        => @in is IXmlLineInfo info
            ? new DefaultXmlLineInfo(info)
            : DefaultXmlLineInfo.NoLineInfo;

    /// <summary>
    /// Gets the <see cref="IXmlLineInfo"/> object from the specified XML
    /// reader, if available.
    /// </summary>
    /// <param name="in">
    /// The XML reader.
    /// </param>
    /// <returns>
    /// The <see cref="IXmlLineInfo"/> object that the XML reader object is
    /// cast to, or the default <see cref="IXmlLineInfo"/> object if the cast
    /// fails.
    /// </returns>
    public static IXmlLineInfo AsXmlLineInfo(XmlReader @in)
        => @in is IXmlLineInfo info
            ? info
            : DefaultXmlLineInfo.NoLineInfo;

    /// <summary>
    /// Creates a new <see cref="BindException"/> for when an element is empty
    /// but a required child element was expected.
    /// </summary>
    /// <param name="in">
    /// The XML reader.
    /// </param>
    /// <param name="expectedChildElementName">
    /// The name of the element that is expected as the child element.
    /// </param>
    /// <returns>
    /// The new <see cref="BindException"/>.
    /// </returns>
    public static BindException NewBindExceptionDueToEmptyElement(
        XmlReader @in, XmlQualifiedName expectedChildElementName)
    {
        var actualNodeType = @in.NodeType;
        var actualName = NewQName(@in);
        var reason = $"""
            {actualNodeType} '{actualName}' is empty. (It was expected to contain the child element '{expectedChildElementName}'.)
            """;
        return new(reason, AsXmlLineInfo(@in));
    }

    /// <summary>
    /// Creates a <see cref="XmlQualifiedName"/> from the current node (element
    /// or attribute) of the specified <see cref="XmlReader"/>.
    /// </summary>
    /// <param name="in">
    /// The XML reader.
    /// </param>
    /// <returns>
    /// A new <see cref="XmlQualifiedName"/> object representing the name of
    /// the current node in the XML reader.
    /// </returns>
    public static XmlQualifiedName NewQName(XmlReader @in)
        => new(@in.LocalName, @in.NamespaceURI);

    /// <summary>
    /// Checks if the qualified name of the current node in the <see
    /// cref="XmlReader"/> matches the specified <see
    /// cref="XmlQualifiedName"/>.
    /// </summary>
    /// <param name="in">
    /// The XML reader.
    /// </param>
    /// <param name="qName">
    /// The qualified name.
    /// </param>
    /// <returns>
    /// <c>true</c> if the qualified name of the current node in the XML reader
    /// matches the specified <paramref name="qName"/>; otherwise,
    /// <c>false</c>.
    /// </returns>
    public static bool Equals(XmlReader @in, XmlQualifiedName qName)
        => @in.LocalName == qName.Name
            && @in.NamespaceURI == qName.Namespace;

    /// <summary>
    /// Creates a new <see cref="BindException"/> for an unexpected node type
    /// encountered by the XML reader.
    /// </summary>
    /// <param name="in">
    /// The XML reader.
    /// </param>
    /// <param name="hint">
    /// A hint message indicating what was expected.
    /// </param>
    /// <returns>
    /// The new <see cref="BindException"/>.
    /// </returns>
    private static BindException NewBindExceptionDueToUnexpectedNodeType(
        XmlReader @in, string hint)
    {
        static string GetResult(XmlReader reader)
        {
            reader.MoveToElement();
            return $" of the element '{NewQName(reader)}'";
        }

        var actualNodeType = @in.NodeType;
        var result = (actualNodeType is XmlNodeType.Element
                || actualNodeType is XmlNodeType.EndElement)
            ? GetResult(@in)
            : string.Empty;
        var reason = $"""
            Unexpected node type: {actualNodeType}{result}. ({hint})
            """;
        return new(reason, AsXmlLineInfo(@in));
    }

    /// <summary>
    /// Confirms that the current node in the XML reader matches the expected
    /// node type and qualified name. Throws a <see cref="BindException"/>
    /// otherwise.
    /// </summary>
    /// <param name="in">
    /// The XML reader.
    /// </param>
    /// <param name="expectedName">
    /// The qualified name that is expected.
    /// </param>
    /// <param name="expectedNodeType">
    /// <see cref="XmlNodeType.Element"/> or <see
    /// cref="XmlNodeType.EndElement"/>.
    /// </param>
    /// <param name="startOrEnd">
    /// A hint for the exception message (e.g., <c>"start"</c> or <c>"end"</c>)
    /// indicating if an element start or end was expected.
    /// </param>
    private static void ConfirmElement(
        XmlReader @in,
        XmlQualifiedName expectedName,
        XmlNodeType expectedNodeType,
        string startOrEnd)
    {
        var actualNodeType = @in.NodeType;
        if (actualNodeType == expectedNodeType
            && Equals(@in, expectedName))
        {
            return;
        }
        throw NewBindExceptionDueToUnexpectedNodeType(
            @in,
            $"""
            It was expected for the element '{expectedName}' to {startOrEnd}.
            """);
    }

    /// <summary>
    /// Gets whether the specified node type represents characters or
    /// whitespace.
    /// </summary>
    /// <param name="nodeType">
    /// The node type to check.
    /// </param>
    /// <returns>
    /// <c>true</c> if the node type is <see cref="XmlNodeType.Text"/> or <see
    /// cref="XmlNodeType.Whitespace"/>; otherwise, <c>false</c>.
    /// </returns>
    private static bool IsCharacters(XmlNodeType nodeType)
        => nodeType is XmlNodeType.Text
            || nodeType is XmlNodeType.Whitespace;
}
