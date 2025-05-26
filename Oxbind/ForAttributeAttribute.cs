namespace Maroontress.Oxbind;

using System;
using System.Xml;

/// <summary>
/// Marks a constructor parameter to be bound to the XML attribute.
/// </summary>
/// <remarks>
/// <para>
/// The <see cref="ForAttributeAttribute"/> associates a constructor parameter
/// with an XML attribute name and namespace. Use an empty string (<c>""</c>)
/// for <paramref name="ns"/> to specify the null namespace (i.e., no
/// namespace).
/// </para>
/// <para>
/// If the <see cref="Oxbinder{T}"/> encounters an XML attribute, it populates
/// the constructor parameter corresponding to the attribute name with the
/// attribute's value.
/// </para>
/// <para>
/// This attribute must mark a constructor parameter and the parameter must be
/// one of the following types:
/// <list type="bullet">
/// <item><description><see cref="string"/>?</description></item>
/// <item><description><see
/// cref="BindResult{T}">BindResult&lt;string&gt;</see>?</description></item>
/// </list>
/// Additionally, the class containing this constructor must be annotated with
/// <see cref="ForElementAttribute"/>.
/// </para>
/// <para>
/// The attribute name specified in <see cref="ForAttributeAttribute"/> must be
/// unique within a single class. For example, if a constructor parameter is
/// annotated with <c>[ForAttribute("foo")]</c>, no other parameter in the same
/// class can be annotated with <c>[ForAttribute("foo")]</c>.
/// </para>
/// <para>
/// All parameters marked with <see cref="ForAttributeAttribute"/> must be
/// placed at the beginning of the constructor parameter list, before any other
/// parameters marked with other Oxbind binding attributes (such as <see
/// cref="RequiredAttribute"/>, <see cref="OptionalAttribute"/>, <see
/// cref="MultipleAttribute"/>, or <see cref="ForTextAttribute"/>).
/// </para>
/// <para>
/// If the XML attribute does not exist in the element, the parameter is
/// populated with <see langword="null"/>. Therefore, the parameter type must
/// be a nullable reference type.
/// </para>
/// </remarks>
/// <param name="name">
/// The attribute name. This must be a valid XML attribute name.
/// </param>
/// <param name="ns">
/// The namespace URI. Use an empty string ("") for attributes without a
/// namespace.
/// </param>
/// <example>
/// <code>
/// [ForElement("person")] public record class Person( [ForAttribute("id")]
/// string? Id, [ForAttribute("name")] string? Name);
///
/// // Corresponding XML: // <![CDATA[<person id="42" name="John Doe"/>]]>
/// </code>
/// </example>
[AttributeUsage(
    AttributeTargets.Parameter,
    Inherited = false,
    AllowMultiple = false)]
public sealed class ForAttributeAttribute(string name, string ns = "")
    : Attribute
{
    /// <summary>
    /// Gets the qualified name of the attribute.
    /// </summary>
    public XmlQualifiedName QName { get; } = new(name, ns);
}
