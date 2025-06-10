namespace Maroontress.Oxbind.Impl;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;

/// <summary>
/// Represents a parameter that is associated with a child element in the XML
/// binding schema.
/// </summary>
/// <param name="UnitType">
/// The type of the unit represented by this parameter. This type must be a
/// class attributed with <see cref="ForElementAttribute"/>.
/// </param>
/// <param name="SchemaType">
/// The schema type that describes how the parameter is mapped and processed in
/// the schema.
/// </param>
/// <param name="ElementName">
/// The qualified name of the XML element associated with the <paramref
/// name="UnitType"/>.
/// </param>
/// <param name="Info">
/// The <see cref="ParameterInfo"/> instance that provides metadata about the
/// parameter.
/// </param>
public record struct ChildParameter(
    Type UnitType,
    SchemaType SchemaType,
    XmlQualifiedName ElementName,
    ParameterInfo Info)
{
    /// <summary>
    /// Gets a mapping from attribute types to their corresponding schema
    /// types.
    /// </summary>
    public static IReadOnlyDictionary<Type, SchemaType> SchemaTypeMap { get; }
        = NewMap();

    /// <summary>
    /// Gets the collection of attribute types that are supported for schema
    /// mapping.
    /// </summary>
    public static IReadOnlyCollection<Type> AttributeTypes { get; }
        = NewMap().Keys;

    /// <summary>
    /// Gets the <see cref="SchemaType"/> associated with the specified
    /// attribute type.
    /// </summary>
    /// <param name="type">
    /// The attribute type.
    /// </param>
    /// <returns>
    /// The corresponding <see cref="SchemaType"/>, or <c>null</c> if the type
    /// is not mapped.
    /// </returns>
    public static SchemaType? ToSchemaType(Type type)
        => SchemaTypeMap.TryGetValue(type, out var schemaType)
            ? schemaType : null;

    /// <summary>
    /// Creates a <see cref="ChildParameter"/> instance from the specified <see
    /// cref="ParameterInfo"/>.
    /// </summary>
    /// <param name="p">
    /// The parameter information.
    /// </param>
    /// <param name="nameBank">
    /// The <see cref="QNameBank"/> instance used to intern XML qualified
    /// names.
    /// </param>
    /// <returns>
    /// A <see cref="ChildParameter"/> representing the parameter.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the parameter is not annotated with a supported attribute for
    /// child element binding (e.g., <see cref="RequiredAttribute"/>, <see
    /// cref="OptionalAttribute"/>, or <see cref="MultipleAttribute"/>).
    /// </exception>
    public static ChildParameter Of(ParameterInfo p, QNameBank nameBank)
    {
        var parameterType = p.ParameterType;
        if (p.GetCustomAttributes()
                .Select(a => a.GetType())
                .Intersect(AttributeTypes)
                .FirstOrDefault() is not {} firstAttributeType
            || ToSchemaType(firstAttributeType) is not {} schemaType)
        {
            throw new InvalidOperationException(
                """
                The parameter does not have any supported attribute.
                """);
        }
        var elementType = Types.IsRawType(parameterType, Types.IEnumerableT)
            ? Types.FirstInnerType(parameterType)
            : parameterType;
        var unitType = Types.IsRawType(elementType, Types.BindResultT)
            ? Types.FirstInnerType(elementType)
            : elementType;
        var name = (elementType.GetCustomAttribute<ForElementAttribute>()
                is {} forElementAttribute)
            ? nameBank.Intern(forElementAttribute.QName)
            : XmlQualifiedName.Empty;
        return new ChildParameter(unitType, schemaType, name, p);
    }

    /// <summary>
    /// Creates a new mapping from attribute types to their corresponding
    /// schema types.
    /// </summary>
    /// <returns>
    /// A <see cref="Dictionary{Type, SchemaType}"/> containing the mapping.
    /// </returns>
    private static Dictionary<Type, SchemaType> NewMap() => new()
    {
        [typeof(RequiredAttribute)] = new RequiredSchemaType(),
        [typeof(OptionalAttribute)] = new OptionalSchemaType(),
        [typeof(MultipleAttribute)] = new MultipleSchemaType(),
    };
}
/*
| Type of value                | UnitType | ElementType     |
| :---                         | :---     | :---            |
| `T`                          | `T`      | `T`             |
| `BindResult<T>`              | `T`      | `BindResult<T>` |
| `IEnumerable<T>`             | `T`      | `T`             |
| `IEnumerable<BindResult<T>>` | `T`      | `BindResult<T>` |
*/
