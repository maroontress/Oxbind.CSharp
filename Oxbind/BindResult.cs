namespace Maroontress.Oxbind;

/// <summary>
/// A result that occurs during XML deserialization, providing the deserialized
/// value along with its location in the XML document.
/// </summary>
/// <typeparam name="T">
/// The type of the deserialized value. When binding to an XML element, this is
/// typically a class annotated with <see cref="ForElementAttribute"/>. When
/// binding to an XML attribute or text content, this is typically <see
/// cref="string"/>.
/// </typeparam>
public interface BindResult<out T>
    where T : class
{
    /// <summary>
    /// Gets the deserialized value.
    /// </summary>
    T Value { get; }

    /// <summary>
    /// Gets the line number where this result occurs in the XML document. The
    /// value is 1-based.
    /// </summary>
    int Line { get; }

    /// <summary>
    /// Gets the column number where this result occurs in the XML document.
    /// The value is 1-based.
    /// </summary>
    int Column { get; }
}
