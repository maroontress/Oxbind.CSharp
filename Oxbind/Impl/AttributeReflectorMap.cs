namespace Maroontress.Oxbind.Impl;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;

/// <summary>
/// The map of an attribute name to the <see cref="Reflector{T}"/> object that
/// dispatches a string value to a constructor argument.
/// </summary>
public sealed class AttributeReflectorMap
    : Dictionary<XmlQualifiedName, Reflector<string>>
{
    private AttributeReflectorMap(
        IEnumerable<AttributeParameter> attributeParameters)
    {
        foreach (var p in attributeParameters)
        {
            var reflector = Reflectors.OfString(p.Info);
            Add(p.Name, reflector);
        }
    }

    /// <summary>
    /// Returns a new read-only dictionary of an attribute name to the <see
    /// cref="Reflector{T}"/> object.
    /// </summary>
    /// <param name="attributeParameters">
    /// The collection of <see cref="AttributeParameter"/> instances that
    /// represent constructor parameters already processed to be associated
    /// with XML attributes (originating from a class attributed with <see
    /// cref="ForElementAttribute"/>).
    /// </param>
    /// <returns>
    /// A new read-only dictionary. Each key in the map is the attribute name
    /// specified as an argument to <see cref="ForAttributeAttribute"/>. The
    /// value associated with the key is the <see cref="Reflector{T}"/> object
    /// that dispatches the attribute value to the constructor parameters.
    /// </returns>
    public static IReadOnlyDictionary<XmlQualifiedName, Reflector<string>>
        Of(IEnumerable<AttributeParameter> attributeParameters)
    {
        return new ReadOnlyDictionary<XmlQualifiedName, Reflector<string>>(
            new AttributeReflectorMap(attributeParameters));
    }
}
