namespace Maroontress.Oxbind.Impl;

using System;
using System.IO;
using System.Xml;

/// <summary>
/// The default implementation of the <see cref="Oxbinder{T}"/> interface.
/// </summary>
/// <typeparam name="T">
/// The type of the instance to create.
/// </typeparam>
/// <param name="getMetadata">
/// The function that returns the <see cref="Metadata"/> associated with the
/// specified class.
/// </param>
public sealed class OxBinderImpl<T>(Func<Type, Metadata> getMetadata)
    : Oxbinder<T>
    where T : class
{
    /// <summary>
    /// Gets the function that returns the <see cref="Impl.Metadata"/>
    /// associated with the specified class.
    /// </summary>
    private Func<Type, Metadata> MetadataSupplier { get; } = getMetadata;

    /// <inheritdoc/>
    public T NewInstance(TextReader reader)
    {
        var settings = new XmlReaderSettings
        {
            IgnoreComments = true,
            IgnoreProcessingInstructions = true,
        };
        return NewInstance(XmlReader.Create(reader, settings));
    }

    /// <summary>
    /// Creates a new instance from the specified XML reader.
    /// </summary>
    /// <param name="in">
    /// The XML reader.
    /// </param>
    /// <returns>
    /// A new instance.
    /// </returns>
    private T NewInstance(XmlReader @in)
    {
        Readers.ConfirmNext(@in);
        @in.Read();
        if (@in.NodeType == XmlNodeType.XmlDeclaration)
        {
            Readers.ConfirmNext(@in);
            @in.Read();
        }
        var m = MetadataSupplier(typeof(T));
        var instance = m.MandatoryElement(@in, MetadataSupplier);
        Readers.ConfirmEndOfStream(@in);
        return (T)instance;
    }
}
