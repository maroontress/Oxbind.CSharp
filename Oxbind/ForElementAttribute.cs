namespace Maroontress.Oxbind;

using System;
using System.Xml;

/// <summary>
/// Marks a class to be bound with the XML element.
/// </summary>
/// <param name="name">
/// The element name.
/// </param>
/// <param name="ns">
/// The namespace URI.
/// </param>
[AttributeUsage(
    AttributeTargets.Class,
    Inherited = false,
    AllowMultiple = false)]
public sealed class ForElementAttribute(string name, string ns = "")
    : Attribute
{
    /// <summary>
    /// Gets the qualified name of the element.
    /// </summary>
    public XmlQualifiedName QName { get; } = new(name, ns);
}
