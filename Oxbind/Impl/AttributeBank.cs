namespace Maroontress.Oxbind.Impl;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml;

/// <summary>
/// Represents metadata that binds a class and its constructor parameters to
/// XML attributes.
/// </summary>
/// <remarks>
/// <c>AttributeBank</c> objects are immutable.
/// </remarks>
public sealed class AttributeBank
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AttributeBank"/> class.
    /// </summary>
    /// <param name="ctor">
    /// The constructor of the class annotated with <see
    /// cref="ForElementAttribute"/>.
    /// </param>
    /// <param name="elementName">
    /// The qualified name of the XML element, derived from the <see
    /// cref="ForElementAttribute"/> on the class bound to this metadata.
    /// </param>
    /// <param name="attributeParameters">
    /// The collection of <see cref="AttributeParameter"/> instances,
    /// representing constructor parameters attributed with <see
    /// cref="ForAttributeAttribute"/>.
    /// </param>
    public AttributeBank(
        ConstructorInfo ctor,
        XmlQualifiedName elementName,
        IEnumerable<AttributeParameter> attributeParameters)
    {
        var map = AttributeReflectorMap.Of(attributeParameters);
        ElementName = elementName;
        ToReflector = (name) => map.TryGetValue(name, out var reflector)
            ? reflector
            : null;
        ParameterCount = ctor.GetParameters().Length;
        Factory = CreateFactory(ctor);
    }

    /// <summary>
    /// Gets a function that creates an instance of the class bound to this
    /// <see cref="AttributeBank"/> instance, given an array of constructor
    /// parameters.
    /// </summary>
    public Func<object[], object> Factory { get; }

    /// <summary>
    /// Gets the qualified name of the XML element, derived from the <see
    /// cref="ForElementAttribute"/> on the class bound to this metadata.
    /// </summary>
    public XmlQualifiedName ElementName { get; }

    /// <summary>
    /// Gets a function that maps an attribute name to the <see
    /// cref="Reflector{T}">Reflector&lt;string&gt;</see> object.
    /// </summary>
    public Func<XmlQualifiedName, Reflector<string>?> ToReflector { get; }

    private int ParameterCount { get; }

    /// <summary>
    /// Creates a new array of objects to be used as placeholders for the
    /// constructor parameters.
    /// </summary>
    /// <remarks>
    /// The length of the returned array matches the number of parameters of
    /// the constructor associated with this <see cref="AttributeBank"/>
    /// instance.
    /// </remarks>
    /// <returns>
    /// An array of <see cref="object"/> with a length equal to the number of
    /// constructor parameters.
    /// </returns>
    public object[] NewPlaceholder() => new object[ParameterCount];

    private static Func<object[], object> CreateFactory(ConstructorInfo ctor)
    {
        var paramExpr = Expression.Parameter(typeof(object[]), "args");
        var argsExprs = ctor.GetParameters()
            .Select(p => Expression.Convert(
                Expression.ArrayIndex(
                    paramExpr, Expression.Constant(p.Position)),
                p.ParameterType))
            .ToArray();
        var newExpr = Expression.New(ctor, argsExprs);
        var convertExpr = Expression.Convert(newExpr, typeof(object));
        var lambda = Expression.Lambda<Func<object[], object>>(
            convertExpr, paramExpr);
        return lambda.Compile();
    }
}
