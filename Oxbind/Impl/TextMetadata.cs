namespace Maroontress.Oxbind.Impl;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using StyleChecker.Annotations;

/// <summary>
/// Represents metadata that binds the text content of an XML element to a
/// constructor parameter marked with the <see cref="ForTextAttribute"/>
/// annotation.
/// </summary>
/// <param name="bank">
/// The attribute bank to be associated with this metadata.
/// </param>
/// <param name="info">
/// The constructor parameter marked with the attribute <see
/// cref="ForTextAttribute"/>. Its type must be <see cref="string"/> or
/// <see cref="BindResult{T}">BindResult&lt;string&gt;</see>.
/// </param>
public sealed class TextMetadata(AttributeBank bank, ParameterInfo info)
    : Metadata(bank)
{
    /// <summary>
    /// Gets the reflector used to process and inject the text content.
    /// </summary>
    private Reflector<string> Reflector { get; } = Reflectors.OfString(info);

    /// <summary>
    /// Reads the text content of the current element from the specified XML
    /// reader.
    /// </summary>
    /// <remarks>
    /// This method reads all consecutive text and CDATA nodes. The method
    /// leaves the reader positioned on the node that follows the last text
    /// node (typically the end element tag), but does not consume the end
    /// element tag itself. The caller is responsible for reading past the end
    /// element.
    /// </remarks>
    /// <param name="reader">
    /// The XML reader, positioned at the first text node inside an element.
    /// </param>
    /// <returns>
    /// The concatenated text content.
    /// </returns>
    public static string GetInnerText(XmlReader reader)
    {
        static bool IsTextNodeType(XmlReader reader)
            => reader.NodeType is XmlNodeType.Text
                || reader.NodeType is XmlNodeType.CDATA;

        Readers.ConfirmNext(reader);
        if (!IsTextNodeType(reader))
        {
            return string.Empty;
        }
        var text = reader.Value;
        reader.Read();
        Readers.ConfirmNext(reader);
        if (!IsTextNodeType(reader))
        {
            return text;
        }

        var textList = new List<string>()
        {
            text,
            reader.Value,
        };
        for (;;)
        {
            reader.Read();
            Readers.ConfirmNext(reader);
            if (!IsTextNodeType(reader))
            {
                return string.Concat(textList);
            }
            textList.Add(reader.Value);
        }
    }

    /// <inheritdoc/>
    protected override void HandleComponentsWithContent(
        object?[] arguments,
        XmlReader @in,
        [Unused] Func<Type, Metadata> getMetadata)
    {
        var s = Reflector.Sugarcoater;
        var info = s.NewLineInfo(@in);
        var value = GetInnerText(@in);
        Reflector.Inject(arguments, s.NewInstance(info, value));
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Treats an empty element like <c><![CDATA[<element/>]]></c> the same as
    /// <c><![CDATA[<element></element>]]></c>, resulting in an empty string.
    /// </remarks>
    protected override void HandleComponentsWithEmptyElement(
        object?[] arguments,
        XmlReader @in,
        [Unused] Func<Type, Metadata> getMetadata)
    {
        var s = Reflector.Sugarcoater;
        var info = s.NewLineInfo(@in);
        Reflector.Inject(arguments, s.NewInstance(info, string.Empty));
    }
}
