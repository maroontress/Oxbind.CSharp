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
        object?[] arguments)
    {
        var m = getMetadata(unitType);
        _ = Readers.SkipCharacters(input);
        Readers.ConfirmStartElement(input, m.Bank.ElementName);
        var s = reflector.Sugarcoater;
        var info = s.NewLineInfo(input);
        var child = m.CreateInstance(input, getMetadata);
        var o = s.NewInstance(info, child);
        reflector.Inject(arguments, o);
    }

    /// <inheritdoc/>
    public override void ApplyWithTextContent(
        XmlQualifiedName name,
        XmlReader input,
        Reflector<object> reflector,
        object?[] arguments)
    {
        _ = Readers.SkipCharacters(input);
        Readers.ConfirmStartElement(input, name);
        var o = Readers.NewTextOnlyObject(input, name, reflector.Sugarcoater);
        reflector.Inject(arguments, o);
    }

    /// <inheritdoc/>
    public override void ApplyWithEmptyElement(
        Type unitType,
        XmlReader input,
        Func<Type, Metadata> getMetadata,
        [Unused] Reflector<object> reflector,
        [Unused] object?[] arguments)
    {
        var m = getMetadata(unitType);
        throw Readers.NewBindExceptionDueToEmptyElement(
            input, m.Bank.ElementName);
    }
}
