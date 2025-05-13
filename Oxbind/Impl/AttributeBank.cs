namespace Maroontress.Oxbind.Impl;

using System;
using System.Collections.Generic;
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
    /// The name of the XML element, which is the value of the attribute <see
    /// cref="ForElementAttribute"/> for the class bound to this metadata.
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
        ElementConstructor = ctor;
        ElementName = elementName;
        ToReflector = (name) => map.TryGetValue(name, out var reflector)
            ? reflector
            : null;
        ParameterCount = ctor.GetParameters().Length;
    }

    /// <summary>
    /// Gets the constructor of the class that represents the XML element.
    /// </summary>
    public ConstructorInfo ElementConstructor { get; }

    /// <summary>
    /// Gets the name of the XML element, which is the value from the <see
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
}
