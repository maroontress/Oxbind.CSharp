namespace Maroontress.Oxbind;

using System;
using System.Collections.Generic;

/// <summary>
/// Marks a constructor parameter to be bound to an XML element that can occur
/// zero or more times.
/// </summary>
/// <remarks>
/// <para>
/// If the <see cref="Oxbinder{T}"/> finds matching XML child elements, it
/// creates objects bound to those elements, and then populates the annotated
/// parameter with a collection of these objects.
/// </para>
/// <para>
/// This attribute must mark a constructor parameter. The class containing
/// this constructor must be annotated with <see cref="ForElementAttribute"/>.
/// </para>
/// <para>
/// The parameter must be of type <see cref="IEnumerable{T}"/> or <see
/// cref="IEnumerable{T}">IEnumerable&lt;BindResult&lt;T&gt;&gt;</see>, where
/// <c>T</c> is a class annotated with <see cref="ForElementAttribute"/>. If no
/// matching child elements are found, an empty collection is provided.
/// </para>
/// <para>
/// After a parameter with <see cref="MultipleAttribute"/>, you cannot specify
/// another parameter for the same element name.
/// </para>
/// </remarks>
/// <seealso cref="OptionalAttribute"/>
/// <seealso cref="RequiredAttribute"/>
[AttributeUsage(
    AttributeTargets.Parameter,
    Inherited = false,
    AllowMultiple = false)]
public sealed class MultipleAttribute : Attribute;
