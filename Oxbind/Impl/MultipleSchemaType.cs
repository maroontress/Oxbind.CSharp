namespace Maroontress.Oxbind.Impl;

using System;
using System.Collections.Generic;
using System.Xml;
using StyleChecker.Annotations;

/// <summary>
/// Represents a schema type that is associated with <see
/// cref="MultipleAttribute"/>.
/// </summary>
public sealed class MultipleSchemaType
    : SchemaType
{
    /// <inheritdoc/>
    public override void ApplyWithContent(
        Type unitType,
        XmlReader input,
        Func<Type, Metadata> getMetadata,
        Reflector<object> reflector,
        Action<object> setChildValue)
    {
        var m = getMetadata(unitType);
        var list = new List<object>();
        for (;;)
        {
            var nodeType = Readers.SkipCharacters(input);
            if (nodeType != XmlNodeType.Element)
            {
                break;
            }
            if (!Readers.Equals(input, m.Bank.ElementName))
            {
                break;
            }
            var info = Readers.ToXmlLineInfo(input);
            var child = m.CreateInstance(input, getMetadata);
            list.Add(reflector.Sugarcoater(info, child));
        }
        var count = list.Count;
        // Create an array of the specific element type (e.g., T or
        // BindResult<T>)
        var array = Array.CreateInstance(reflector.UnitType, count);
        for (var k = 0; k < count; ++k)
        {
            array.SetValue(list[k], k);
        }
        setChildValue(array);
    }

    /// <inheritdoc/>
    public override void ApplyWithEmptyElement(
        [Unused] Type unitType,
        [Unused] XmlReader input,
        [Unused] Func<Type, Metadata> getMetadata,
        Reflector<object> reflector,
        Action<object> setChildValue)
    {
        var array = Array.CreateInstance(reflector.UnitType, 0);
        setChildValue(array);
    }
}
