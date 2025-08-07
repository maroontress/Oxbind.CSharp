namespace Maroontress.Oxbind.Impl;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using StyleChecker.Annotations;

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
            x.MainAction(arguments, @in, getMetadata);
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
            x.EmptyAction(arguments, @in, getMetadata);
        }
    }

    private static List<Child> NewChildList(
        IEnumerable<ChildParameter> children)
    {
        return [.. children.Select(ToChild)];
    }

    private static Child ToChild(ChildParameter p)
    {
        var schemaType = p.SchemaType;
        var unitType = p.UnitType;
        var name = p.ElementName;
        var reflector = Reflectors.Of(p.Info);

        void ElementAction(
            object?[] arguments,
            XmlReader @in,
            Func<Type, Metadata> getMetadata)
        {
            schemaType.ApplyWithContent(
                unitType, @in, getMetadata, reflector, arguments);
        }

        void TextAction(
            object?[] arguments,
            XmlReader @in,
            [Unused] Func<Type, Metadata> getMetadata)
        {
            schemaType.ApplyWithTextContent(name, @in, reflector, arguments);
        }

        void EmptyAction(
            object?[] arguments,
            XmlReader @in,
            Func<Type, Metadata> getMetadata)
        {
            schemaType.ApplyWithEmptyElement(
                unitType, @in, getMetadata, reflector, arguments);
        }

        var elementAction = ElementAction;
        var textAction = TextAction;
        var mainAction = unitType == typeof(string)
            ? textAction
            : elementAction;
        return new(mainAction, EmptyAction);
    }

    private record struct Child(
        Action<object?[], XmlReader, Func<Type, Metadata>> MainAction,
        Action<object?[], XmlReader, Func<Type, Metadata>> EmptyAction);
}
