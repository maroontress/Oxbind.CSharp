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

    /// <inheritdoc/>
    protected override void HandleComponentsWithContent(
        object?[] arguments,
        XmlReader @in,
        [Unused] Func<Type, Metadata> getMetadata)
    {
        static string GetInnerText(XmlReader reader)
        {
            Readers.ConfirmNext(reader);
            if (reader.NodeType != XmlNodeType.Text)
            {
                return string.Empty;
            }
            var text = reader.Value;
            reader.Read();
            Readers.ConfirmNext(reader);
            if (reader.NodeType != XmlNodeType.Text)
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
                if (reader.NodeType != XmlNodeType.Text)
                {
                    return string.Concat(textList);
                }
                textList.Add(reader.Value);
            }
        }

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
