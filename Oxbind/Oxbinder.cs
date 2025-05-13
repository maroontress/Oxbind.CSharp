namespace Maroontress.Oxbind;

using System.IO;
using System.Xml;

/// <summary>
/// Defines a contract for deserializing XML from a text reader into an object
/// of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">
/// The type of the instance to create. This must be a class attributed with
/// <see cref="ForElementAttribute"/>.
/// </typeparam>
public interface Oxbinder<out T>
    where T : class
{
    /// <summary>
    /// Creates a new instance from the specified text reader.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The XML document must have a root element that matches type
    /// <typeparamref name="T"/> (i.e., the element name and namespace must
    /// correspond to the <see cref="ForElementAttribute"/> on <typeparamref
    /// name="T"/>).
    /// </para>
    /// </remarks>
    /// <param name="reader">
    /// The text reader that provides the XML stream.
    /// </param>
    /// <returns>
    /// A new instance corresponding to the XML root element.
    /// </returns>
    /// <exception cref="BindException">
    /// Thrown when the input XML does not conform to the expected binding
    /// schema for the target type <typeparamref name="T"/> or any of its
    /// transitively referenced types (e.g., when required elements or
    /// attributes are missing, unexpected elements are present, or when data
    /// types do not match).
    /// </exception>
    /// <exception cref="XmlException">
    /// Thrown when the XML in <paramref name="reader"/> is not well-formed or
    /// cannot be parsed.
    /// </exception>
    /// <exception cref="IOException">
    /// Thrown when an I/O error occurs while reading from <paramref
    /// name="reader"/>.
    /// </exception>
    T NewInstance(TextReader reader);
}
