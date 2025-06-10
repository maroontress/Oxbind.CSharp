namespace Maroontress.Oxbind.Impl;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

/// <summary>
/// Metadata of the classes that have constructor parameters annotated with
/// the <see cref="RequiredAttribute"/>, <see cref="OptionalAttribute"/>,
/// or <see cref="MultipleAttribute"/>.
/// </summary>
/// <param name="bank">
/// The attribute bank to be associated with this metadata.
/// </param>
/// <param name="children">
/// A collection of <see cref="ChildParameter"/> objects.
/// </param>
public sealed class SchemaMetadata(
        AttributeBank bank, IEnumerable<ChildParameter> children)
    : Metadata(bank)
{
    /// <summary>
    /// Gets the immutable list containing <see cref="Child"/> objects.
    /// </summary>
    private IReadOnlyList<Child> ChildList { get; } = NewChildList(children);

    /// <inheritdoc/>
    protected override void HandleComponentsWithContent(
        object?[] arguments,
        XmlReader @in,
        Func<Type, Metadata> getMetadata)
    {
        var n = ChildList.Count;
        for (var i = 0; i < n; ++i)
        {
            var x = ChildList[i];
            x.SchemaType.ApplyWithContent(
                x.UnitType,
                @in,
                getMetadata,
                x.Reflector,
                arguments);
        }
    }

    /// <inheritdoc/>
    protected override void HandleComponentsWithEmptyElement(
        object?[] arguments,
        XmlReader @in,
        Func<Type, Metadata> getMetadata)
    {
        var n = ChildList.Count;
        for (var i = 0; i < n; ++i)
        {
            var x = ChildList[i];
            x.SchemaType.ApplyWithEmptyElement(
                x.UnitType,
                @in,
                getMetadata,
                x.Reflector,
                arguments);
        }
    }

    private static List<Child> NewChildList(
        IEnumerable<ChildParameter> children)
    {
        static Child ToChild(ChildParameter p)
        {
            var schemaType = p.SchemaType;
            var unitType = p.UnitType;
            var reflector = Reflectors.Of(p.Info);
            return new(schemaType, unitType, reflector);
        }

        return [.. children.Select(ToChild)];
    }

    private record struct Child(
        SchemaType SchemaType,
        Type UnitType,
        Reflector<object> Reflector);
}
