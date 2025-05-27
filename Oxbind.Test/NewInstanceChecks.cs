namespace Maroontress.Oxbind.Test;

using System.IO;

/// <summary>
/// Provides check methods shared with various tests.
/// </summary>
public static class NewInstanceChecks
{
    /// <summary>
    /// Checks the <see cref="BindException"/> that <see
    /// cref="Oxbinder{T}.NewInstance(TextReader)"/> with the specified XML
    /// document throws.
    /// </summary>
    /// <typeparam name="T">
    /// The class associated with the root element.
    /// </typeparam>
    /// <param name="xml">
    /// The XML document.
    /// </param>
    /// <param name="message">
    /// The expected message that the <see cref="BindException"/> contains.
    /// </param>
    public static void ThrowBindException<T>(string xml, string message)
        where T : class
    {
        var factory = new OxbinderFactory();
        var binder = factory.Of<T>();
        ThrowBindException(binder, xml, message);
    }

    /// <summary>
    /// Checks the <see cref="BindException"/> that <see
    /// cref="Oxbinder{T}.NewInstance(TextReader)"/> with the specified XML
    /// document throws.
    /// </summary>
    /// <typeparam name="T">
    /// The class associated with the root element.
    /// </typeparam>
    /// <param name="binder">
    /// The <see cref="Oxbinder{T}"/> instance.
    /// </param>
    /// <param name="xml">
    /// The XML document.
    /// </param>
    /// <param name="message">
    /// The expected message that the <see cref="BindException"/> contains.
    /// </param>
    public static void ThrowBindException<T>(
        Oxbinder<T> binder, string xml, string message)
        where T : class
    {
        var reader = new StringReader(xml);
        Checks.ThrowBindException(
            () => _ = binder.NewInstance(reader),
            "NewInstance()",
            message);
    }
}
