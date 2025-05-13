namespace Maroontress.Oxbind.Impl;

using System;
using System.Xml;
using StyleChecker.Annotations;

/// <summary>
/// Represents a schema type that is associated with <see
/// cref="RequiredAttribute"/>.
/// </summary>
public sealed class RequiredSchemaType
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
        _ = Readers.SkipCharacters(input);
        Readers.ConfirmStartElement(input, m.Bank.ElementName);
        var info = Readers.ToXmlLineInfo(input);
        var child = m.CreateInstance(input, getMetadata);
        setChildValue(reflector.Sugarcoater(info, child));
    }

    /// <inheritdoc/>
    public override void ApplyWithEmptyElement(
        Type unitType,
        XmlReader input,
        Func<Type, Metadata> getMetadata,
        [Unused] Reflector<object> reflector,
        [Unused] Action<object> setChildValue)
    {
        var m = getMetadata(unitType);
        throw Readers.NewBindExceptionDueToEmptyElement(
            input, m.Bank.ElementName);
    }
}
