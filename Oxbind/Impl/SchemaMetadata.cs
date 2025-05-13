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
    /// Gets the immutable list that contains <see cref="X"/> objects.
    /// </summary>
    private IReadOnlyList<X> ChildList { get; } = NewChildList(children);

    /// <inheritdoc/>
    protected override void HandleComponentsWithContent(
        object[] arguments,
        XmlReader @in,
        Func<Type, Metadata> getMetadata)
    {
        HandleAction(x =>
        {
            var reflector = x.Reflector;
            x.SchemaType.ApplyWithContent(
                x.UnitType,
                @in,
                getMetadata,
                reflector,
                o => reflector.Inject(arguments, o));
        });
    }

    /// <inheritdoc/>
    protected override void HandleComponentsWithEmptyElement(
        object[] arguments,
        XmlReader @in,
        Func<Type, Metadata> getMetadata)
    {
        HandleAction(x =>
        {
            var reflector = x.Reflector;
            x.SchemaType.ApplyWithEmptyElement(
                x.UnitType,
                @in,
                getMetadata,
                reflector,
                o => reflector.Inject(arguments, o));
        });
    }

    private static List<X> NewChildList(IEnumerable<ChildParameter> children)
    {
        return [.. children.Select(p =>
        {
            var schemaType = p.SchemaType;
            var unitType = p.UnitType;
            var reflector = Reflectors.Of(p.Info);
            return new X(schemaType, unitType, reflector);
        })];
    }

    private void HandleAction(Action<X> action)
    {
        foreach (var x in ChildList)
        {
            action(x);
        }
    }

    private record struct X(
        SchemaType SchemaType,
        Type UnitType,
        Reflector<object> Reflector)
    {
    }
}
