namespace Maroontress.Oxbind.Impl;

using System;
using System.Reflection;
using System.Text;
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
        object[] arguments,
        XmlReader @in,
        [Unused] Func<Type, Metadata> getMetadata)
    {
        var info = Readers.ToXmlLineInfo(@in);
        var b = new StringBuilder();
        for (;;)
        {
            Readers.ConfirmNext(@in);
            var nodeType = @in.NodeType;
            if (nodeType != XmlNodeType.Text)
            {
                break;
            }
            b.Append(@in.Value);
            @in.Read();
        }
        Reflector.Inject(arguments, Reflector.Sugarcoater(info, b.ToString()));
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Treats an empty element like <c><![CDATA[<element/>]]></c> the same as
    /// <c><![CDATA[<element></element>]]></c>, resulting in an empty string.
    /// </remarks>
    protected override void HandleComponentsWithEmptyElement(
        object[] arguments,
        XmlReader @in,
        [Unused] Func<Type, Metadata> getMetadata)
    {
        var info = Readers.ToXmlLineInfo(@in);
        Reflector.Inject(arguments, Reflector.Sugarcoater(info, string.Empty));
    }
}
