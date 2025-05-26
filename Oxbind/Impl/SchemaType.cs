namespace Maroontress.Oxbind.Impl;

using System;
using System.Xml;

/// <summary>
/// The base class for the schema type.
/// </summary>
/// <remarks>
/// <see cref="SchemaType"/> objects are immutable.
/// </remarks>
public abstract class SchemaType
{
    /// <summary>
    /// Processes a child element that has content, applying the binding logic.
    /// </summary>
    /// <param name="unitType">
    /// The unit type of the child element(s) being bound.
    /// </param>
    /// <param name="input">
    /// The <see cref="XmlReader"/> object.
    /// </param>
    /// <param name="getMetadata">
    /// The function that returns the <see cref="Metadata"/> object for the
    /// specified type.
    /// </param>
    /// <param name="reflector">
    /// The reflector.
    /// </param>
    /// <param name="setChildValue">
    /// An action that injects the processed child value into the parent
    /// object's constructor arguments.
    /// </param>
    public abstract void ApplyWithContent(
        Type unitType,
        XmlReader input,
        Func<Type, Metadata> getMetadata,
        Reflector<object> reflector,
        Action<object> setChildValue);

    /// <summary>
    /// Processes an empty child element, applying the binding logic.
    /// </summary>
    /// <param name="unitType">
    /// The unit type of the child element(s) being bound.
    /// </param>
    /// <param name="input">
    /// The <see cref="XmlReader"/> object.
    /// </param>
    /// <param name="getMetadata">
    /// The function that returns the <see cref="Metadata"/> object for the
    /// specified type.
    /// </param>
    /// <param name="reflector">
    /// The reflector.
    /// </param>
    /// <param name="setChildValue">
    /// An action that injects the processed child value into the parent
    /// object's constructor arguments.
    /// </param>
    public abstract void ApplyWithEmptyElement(
        Type unitType,
        XmlReader input,
        Func<Type, Metadata> getMetadata,
        Reflector<object> reflector,
        Action<object> setChildValue);
}
