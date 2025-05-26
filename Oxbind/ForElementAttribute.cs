namespace Maroontress.Oxbind;

using System;
using System.Xml;

/// <summary>
/// Marks a class to be bound to the XML element.
/// </summary>
/// <remarks>
/// <para>
/// The <see cref="ForElementAttribute"/> associates a class with an XML
/// element name and namespace. Use an empty string (<c>""</c>) for <paramref
/// name="ns"/> to specify the null namespace (no namespace).
/// </para>
/// <para>
/// Multiple classes can be mapped to the same element name, as Oxbind
/// determines the correct mapping contextually based on the constructor
/// parameter order.
/// </para>
/// </remarks>
/// <param name="name">
/// The element name.
/// </param>
/// <param name="ns">
/// The namespace URI. Use an empty string ("") to specify the null namespace
/// (no namespace).
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
