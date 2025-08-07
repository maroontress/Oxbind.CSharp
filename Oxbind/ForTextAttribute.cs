namespace Maroontress.Oxbind;

using System;

/// <summary>
/// Marks a constructor parameter to be bound to the text inside an XML
/// element.
/// </summary>
/// <remarks>
/// <para>
/// If the <see cref="Oxbinder{T}"/> encounters an XML element containing a
/// text node, it populates the constructor parameter with the content of the
/// text node.
/// </para>
/// <para>
/// This attribute must be applied to a constructor parameter of type <see
/// cref="string"/> or <see
/// cref="BindResult{T}"><![CDATA[BindResult<string>]]></see>. The class
/// containing this constructor must also be annotated with <see
/// cref="ForElementAttribute"/>.
/// </para>
/// <para>
/// The parameter attributed with <see cref="ForTextAttribute"/> must be the
/// last parameter of the constructor.
/// </para>
/// <para>
/// A constructor with a parameter attributed with <see
/// cref="ForTextAttribute"/> must not contain:
/// <list type="bullet">
/// <item>
/// <description>
/// Any other parameter attributed with <see cref="ForTextAttribute"/>.
/// </description>
/// </item>
/// <item>
/// <description>
/// Any parameters attributed with <see cref="RequiredAttribute"/>, <see
/// cref="OptionalAttribute"/>, or <see cref="MultipleAttribute"/>.
/// </description>
/// </item>
/// </list>
/// </para>
/// <para>
/// The <see cref="ForTextAttribute"/> is designed to bind the text content of
/// the current element, making it mutually exclusive with attributes that bind
/// to child elements. To bind to the text content of a child element, use
/// the <see cref="ForChildElementAttribute"/> in combination with <see
/// cref="RequiredAttribute"/>, <see cref="OptionalAttribute"/>, or <see
/// cref="MultipleAttribute"/>.
/// </para>
/// <para>
/// The parameter will never be <see langword="null"/>. If the element does not
/// contain a text node, or the text node is empty, the parameter will be set
/// to an empty string (<c>""</c>).
/// </para>
/// </remarks>
[AttributeUsage(
    AttributeTargets.Parameter,
    Inherited = false,
    AllowMultiple = false)]
public sealed class ForTextAttribute : Attribute;
