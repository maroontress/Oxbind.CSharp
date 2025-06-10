namespace Maroontress.Oxbind.Impl;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;

/// <summary>
/// Represents a constructor parameter associated with an XML attribute, as
/// specified by a <see cref="ForAttributeAttribute"/>.
/// </summary>
/// <param name="Name">
/// The qualified name of the XML attribute associated with the parameter.
/// </param>
/// <param name="Info">
/// The <see cref="ParameterInfo"/> instance containing metadata about the
/// parameter.
/// </param>
public record struct AttributeParameter(
    XmlQualifiedName Name, ParameterInfo Info)
{
    /// <summary>
    /// Gets a collection of <see cref="AttributeParameter"/> instances for
    /// constructor parameters attributed with <see
    /// cref="ForAttributeAttribute"/> within the specified constructor.
    /// </summary>
    /// <param name="ctor">
    /// The constructor to analyze.
    /// </param>
    /// <param name="nameBank">
    /// The <see cref="QNameBank"/> instance used to intern XML qualified
    /// names.
    /// </param>
    /// <returns>
    /// A collection of <see cref="AttributeParameter"/> instances for
    /// constructor parameters marked with <see cref="ForAttributeAttribute"/>.
    /// </returns>
    public static IEnumerable<AttributeParameter> Of(
        ConstructorInfo ctor, QNameBank nameBank)
    {
        Func<ParameterInfo, AttributeParameter?> ToAttributeParameter()
            => p => Of(p, nameBank);

        return ctor.GetParameters()
            .Select(ToAttributeParameter())
            .TakeWhile(x => x is {})
            .OfType<AttributeParameter>()
            .AsEnumerable();
    }

    /// <summary>
    /// Converts a <see cref="ParameterInfo"/> to an <see
    /// cref="AttributeParameter"/> if it is associated with a <see
    /// cref="ForAttributeAttribute"/>.
    /// </summary>
    /// <param name="p">
    /// The parameter to convert.
    /// </param>
    /// <param name="nameBank">
    /// The <see cref="QNameBank"/> instance used to intern XML qualified
    /// names.
    /// </param>
    /// <returns>
    /// An <see cref="AttributeParameter"/> if the parameter is associated
    /// with a <see cref="ForAttributeAttribute"/>; otherwise, <c>null</c>.
    /// </returns>
    private static AttributeParameter? Of(ParameterInfo p, QNameBank nameBank)
        => p.GetCustomAttribute<ForAttributeAttribute>() is not {} a
            ? null
            : new AttributeParameter(nameBank.Intern(a.QName), p);
}
