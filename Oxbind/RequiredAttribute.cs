namespace Maroontress.Oxbind;

using System;

/// <summary>
/// Marks a constructor parameter to be bound to an XML element that occurs
/// exactly once.
/// </summary>
/// <remarks>
/// <para>
/// If the <see cref="Oxbinder{T}"/> finds the XML child element, it creates
/// the object bound to the element, and then populates the annotated parameter
/// with the object.
/// </para>
/// <para>
/// This annotation must mark a constructor parameter. The class containing
/// this constructor must be annotated with <see cref="ForElementAttribute"/>.
/// </para>
/// <para>
/// The parameter must be of type <c>T</c> or <see
/// cref="BindResult{T}">BindResult&lt;T&gt;</see>, where <c>T</c> is a class
/// annotated with <see cref="ForElementAttribute"/>. If no matching child
/// element is found, a <see cref="BindException"/> is thrown.
/// </para>
/// </remarks>
/// <seealso cref="MultipleAttribute"/>
/// <seealso cref="OptionalAttribute"/>
[AttributeUsage(
    AttributeTargets.Parameter,
    Inherited = false,
    AllowMultiple = false)]
public sealed class RequiredAttribute : Attribute;
