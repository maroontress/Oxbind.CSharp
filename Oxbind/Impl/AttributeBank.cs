namespace Maroontress.Oxbind.Impl;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
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
    /// <param name="nameBank">
    /// The <see cref="QNameBank"/> instance used to intern XML qualified
    /// names.
    /// </param>
    public AttributeBank(
        ConstructorInfo ctor,
        XmlQualifiedName elementName,
        IEnumerable<AttributeParameter> attributeParameters,
        QNameBank nameBank)
    {
        var map = AttributeReflectorMap.Of(attributeParameters);
        ElementName = elementName;
        ToReflector = (name) => map.TryGetValue(name, out var reflector)
            ? reflector
            : null;
        ParameterCount = ctor.GetParameters().Length;
        Factory = CreateFactory(ctor);
        NameBank = nameBank;
    }

    /// <summary>
    /// Gets a function that creates an instance of the class bound to this
    /// <see cref="AttributeBank"/> instance, given an array of constructor
    /// parameters.
    /// </summary>
    public Func<object?[], object> Factory { get; }

    /// <summary>
    /// Gets the qualified name of the XML element, derived from the <see
    /// cref="ForElementAttribute"/> on the class bound to this metadata.
    /// </summary>
    public XmlQualifiedName ElementName { get; }

    /// <summary>
    /// Gets the <see cref="QNameBank"/> instance used to intern XML qualified
    /// names.
    /// </summary>
    public QNameBank NameBank { get; }

    /// <summary>
    /// Gets a function that maps an attribute name to the <see
    /// cref="Reflector{T}">Reflector&lt;string&gt;</see> object.
    /// </summary>
    public Func<XmlQualifiedName, Reflector<string>?> ToReflector { get; }

    private int ParameterCount { get; }

    private ThreadLocal<Queue<object?[]>> PlaceholderQueue { get; }
        = new(() => new());

    /// <summary>
    /// Retrieves a placeholder array of objects to be used as constructor
    /// parameters.
    /// </summary>
    /// <remarks>
    /// If a previously used placeholder is available in the internal queue,
    /// it is reused. Otherwise, a new placeholder array is created.
    /// </remarks>
    /// <returns>
    /// An array of <see cref="object"/> with a length equal to the number
    /// of constructor parameters.
    /// </returns>
    public object?[] GetPlaceholder()
    {
        return PlaceholderQueue.Value is {} queue
                && queue.Count > 0
            ? queue.Dequeue()
            : NewPlaceholder();
    }

    /// <summary>
    /// Recycles a placeholder array of constructor parameters for future
    /// reuse.
    /// </summary>
    /// <param name="placeholder">
    /// The placeholder array to recycle. Its length must match the number of
    /// constructor parameters.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown if the length of <paramref name="placeholder"/> does not match
    /// the number of constructor parameters.
    /// </exception>
    public void RecyclePlaceholder(object?[] placeholder)
    {
        if (placeholder.Length != ParameterCount)
        {
            throw new ArgumentException(
                """
                The placeholder must have the same length as the number of constructor parameters.
                """,
                nameof(placeholder));
        }
        if (PlaceholderQueue.Value is not {} queue)
        {
            return;
        }
        Array.Clear(placeholder, 0, placeholder.Length);
        queue.Enqueue(placeholder);
    }

    private static Func<object?[], object> CreateFactory(ConstructorInfo ctor)
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
        var lambda = Expression.Lambda<Func<object?[], object>>(
            convertExpr, paramExpr);
        return lambda.Compile();
    }

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
    private object[] NewPlaceholder() => new object[ParameterCount];
}
