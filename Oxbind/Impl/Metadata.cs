namespace Maroontress.Oxbind.Impl;

using System;
using System.Xml;
using Maroontress.Oxbind.Util;

/// <summary>
/// Represents metadata that binds a class and its constructor parameters to an
/// XML element.
/// </summary>
/// <remarks>
/// <c>Metadata</c> objects are immutable.
/// </remarks>
/// <param name="bank">
/// The attribute bank to be associated with this metadata.
/// </param>
public abstract class Metadata(AttributeBank bank)
{
    /// <summary>
    /// Gets the attribute bank associated with this metadata.
    /// </summary>
    public AttributeBank Bank { get; } = bank;

    /// <summary>
    /// Returns a new instance bound to the root XML element that is read from
    /// the specified XML reader.
    /// </summary>
    /// <param name="in">
    /// The XmlReader providing the input stream.
    /// </param>
    /// <param name="getMetadata">
    /// The function that returns the <see cref="Metadata"/> object for the
    /// specified type.
    /// </param>
    /// <returns>
    /// A new instance bound to the root XML element.
    /// </returns>
    public object RequiredElement(
        XmlReader @in,
        Func<Type, Metadata> getMetadata)
    {
        _ = Readers.SkipCharacters(@in);
        Readers.ConfirmStartElement(@in, Bank.ElementName);
        return CreateInstance(@in, getMetadata);
    }

    /// <summary>
    /// Creates a new instance bound to the next XML element in the specified
    /// XML reader.
    /// </summary>
    /// <param name="in">
    /// The XML reader.
    /// </param>
    /// <param name="getMetadata">
    /// The function that returns the <see cref="Metadata"/> object for the
    /// specified type.
    /// </param>
    /// <returns>
    /// A new instance bound to the next XML element in the specified XML
    /// reader.
    /// </returns>
    public object CreateInstance(
        XmlReader @in, Func<Type, Metadata> getMetadata)
    {
        var arguments = Bank.NewPlaceholder();
        Elements.ForEach(@in.AttributeCount, k =>
        {
            @in.MoveToAttribute(k);
            DispatchAttribute(@in, arguments);
        });
        @in.MoveToElement();
        if (@in.IsEmptyElement)
        {
            HandleComponentsWithEmptyElement(arguments, @in, getMetadata);
        }
        else
        {
            @in.Read();
            HandleComponentsWithContent(arguments, @in, getMetadata);

            _ = Readers.SkipCharacters(@in);
            Readers.ConfirmEndElement(@in, Bank.ElementName);
        }
        @in.Read();
        var instance = Bank.ElementConstructor.Invoke(arguments);
        return instance;
    }

    /// <summary>
    /// Handles the components of the instance using the content of the current
    /// XML element provided by the specified XML reader.
    /// </summary>
    /// <param name="arguments">
    /// The array of constructor arguments to populate.
    /// </param>
    /// <param name="in">
    /// The XML reader.
    /// </param>
    /// <param name="getMetadata">
    /// The function that returns the <see cref="Metadata"/> object for the
    /// specified type.
    /// </param>
    protected abstract void HandleComponentsWithContent(
        object[] arguments,
        XmlReader @in,
        Func<Type, Metadata> getMetadata);

    /// <summary>
    /// Handles the component of the specified instance with empty element that
    /// the specified XML reader is providing.
    /// </summary>
    /// <param name="arguments">
    /// The arguments for the constructor parameters to be injected.
    /// </param>
    /// <param name="in">
    /// The XML reader.
    /// </param>
    /// <param name="getMetadata">
    /// The function that returns the <see cref="Metadata"/> object for the
    /// specified type.
    /// </param>
    protected abstract void HandleComponentsWithEmptyElement(
        object[] arguments,
        XmlReader @in,
        Func<Type, Metadata> getMetadata);

    /// <summary>
    /// Invokes the <see cref="Reflector{T}"/> delegate associated with the
    /// attribute name read from the XML reader, injecting the value into the
    /// array of arguments for the constructor.
    /// </summary>
    /// <param name="in">
    /// The XML reader.
    /// </param>
    /// <param name="args">
    /// The array of arguments for the constructor.
    /// </param>
    private void DispatchAttribute(XmlReader @in, object[] args)
    {
        var key = Readers.NewQName(@in);
        if (Bank.ToReflector(key) is not {} reflector)
        {
            // just ignore the attribute if it is not recognized.
            return;
        }
        var value = @in.Value;
        var info = Readers.AsXmlLineInfo(@in);
        reflector.Inject(args, reflector.Sugarcoater(info, value));
    }
}
