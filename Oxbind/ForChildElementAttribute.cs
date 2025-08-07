namespace Maroontress.Oxbind;

using System;
using System.Xml;

/// <summary>
/// Marks a constructor parameter to be bound to the child XML element that
/// only contains text content.
/// </summary>
/// <remarks>
/// <para>
/// The <see cref="ForChildElementAttribute"/> associates a constructor
/// parameter with an XML element name and namespace. Use an empty string
/// (<c>""</c>) for <paramref name="ns"/> to specify the null namespace (i.e.,
/// no namespace).
/// </para>
/// <para>
/// If the <see cref="Oxbinder{T}"/> encounters an XML element, it populates
/// the constructor parameter corresponding to the element with the text
/// content inside the element.
/// </para>
/// <para>
/// This attribute must mark a constructor parameter and the parameter must be
/// annotated with one of the following attributes: <see
/// cref="RequiredAttribute"/>, <see cref="OptionalAttribute"/>, <see
/// cref="MultipleAttribute"/>. The parameter must be one of the following
/// types:
/// <list type="bullet">
/// <item>
/// <description>
/// <see cref="string"/> or
/// <see cref="BindResult{T}"><![CDATA[BindResult<string>]]></see>
/// when it has a <see cref="RequiredAttribute"/>
/// </description>
/// </item>
/// <item>
/// <description>
/// <see cref="string"/>? or
/// <see cref="BindResult{T}"><![CDATA[BindResult<string>]]></see>?
/// when it has a <see cref="OptionalAttribute"/>
/// </description>
/// </item>
/// <item>
/// <description>
/// <see cref="System.Collections.Generic.IEnumerable{T}">
/// <![CDATA[IEnumerable<string>]]>
/// </see> or
/// <see cref="System.Collections.Generic.IEnumerable{T}">
/// <![CDATA[IEnumerable<BindResult<string>>]]>
/// </see> when it has a <see cref="MultipleAttribute"/>
/// </description>
/// </item>
/// </list>
/// Additionally, the class containing this constructor must be annotated with
/// <see cref="ForElementAttribute"/>.
/// </para>
/// <para>
/// The <see cref="ForChildElementAttribute"/> is used to bind the text content
/// of a child element. This is distinct from <see cref="ForTextAttribute"/>,
/// which binds the text content of the current element associated with the
/// class.
/// </para>
/// </remarks>
/// <param name="name">
/// The element name. This must be a valid XML element name.
/// </param>
/// <param name="ns">
/// The namespace URI. Use an empty string ("") for elements without a
/// namespace.
/// </param>
/// <example>
/// <code>
/// [ForElement("person")]
/// public record class Person(
///   [Required][ForChildElement("id")] string Id,
///   [Optional][ForChildElement("name")] string? Name,
///   [Multiple][ForChildElement("email")] IEnumerable&lt;string&gt; Emails);
///
/// // Corresponding XML:
/// // <![CDATA[<person>]]>
/// //   <![CDATA[<id>42</id>]]>
/// //   <![CDATA[<name>John Doe</name>]]>
/// //   <![CDATA[<email>john.doe@example.com</email>]]>
/// // <![CDATA[</person>]]>
/// </code>
/// </example>
[AttributeUsage(
    AttributeTargets.Parameter,
    Inherited = false,
    AllowMultiple = false)]
public sealed class ForChildElementAttribute(string name, string ns = "")
    : Attribute
{
    /// <summary>
    /// Gets the qualified name of the child element.
    /// </summary>
    public XmlQualifiedName QName { get; } = new(name, ns);
}
