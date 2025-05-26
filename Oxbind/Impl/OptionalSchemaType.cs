namespace Maroontress.Oxbind.Impl;

using System;
using System.Xml;
using StyleChecker.Annotations;

/// <summary>
/// Represents a schema type that is associated with <see
/// cref="OptionalAttribute"/>.
/// </summary>
public sealed class OptionalSchemaType
    : SchemaType
{
    /// <inheritdoc/>
    public override void ApplyWithContent(
        Type unitType,
        XmlReader input,
        Func<Type, Metadata> getMetadata,
        Reflector<object> reflector,
        Action<object> setChildValue)
    {
        var m = getMetadata(unitType);
        var nodeType = Readers.SkipCharacters(input);
        if (nodeType != XmlNodeType.Element)
        {
            return;
        }
        if (!Readers.Equals(input, m.Bank.ElementName))
        {
            return;
        }
        var info = Readers.ToXmlLineInfo(input);
        var child = m.CreateInstance(input, getMetadata);
        setChildValue(reflector.Sugarcoater(info, child));
    }

    /// <inheritdoc/>
    public override void ApplyWithEmptyElement(
        [Unused] Type unitType,
        [Unused] XmlReader input,
        [Unused] Func<Type, Metadata> getMetadata,
        [Unused] Reflector<object> reflector,
        [Unused] Action<object> setChildValue)
    {
    }
}
