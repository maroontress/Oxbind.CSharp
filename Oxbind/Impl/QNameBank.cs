namespace Maroontress.Oxbind.Impl;

using System.Xml;
using Maroontress.Oxbind.Util;

/// <summary>
/// Represents a bank interning <see cref="XmlQualifiedName"/> instances.
/// </summary>
public sealed class QNameBank
{
    /// <summary>
    /// Gets the map for names without a namespace.
    /// </summary>
    private InternMap<string, XmlQualifiedName> NoNamespace { get; } = new();

    /// <summary>
    /// Gets the map for names with a namespace.
    /// </summary>
    private InternMap<string, QualifiedNameMap> NamespaceMap { get; } = new();

    /// <summary>
    /// Interns the specified <see cref="XmlQualifiedName"/> instance.
    /// </summary>
    /// <param name="qualifiedName">
    /// The qualified name to intern.
    /// </param>
    /// <returns>
    /// The canonical <see cref="XmlQualifiedName"/> instance.
    /// </returns>
    public XmlQualifiedName Intern(XmlQualifiedName qualifiedName)
    {
        var ns = qualifiedName.Namespace;
        var localName = qualifiedName.Name;
        if (ns.Length is 0)
        {
            return NoNamespace.Intern(localName, qualifiedName);
        }
        var namespaceMap = NamespaceMap.Intern(ns, NewQualifiedNameMap);
        return namespaceMap.LocalNameMap.Intern(localName, qualifiedName);
    }

    /// <summary>
    /// Finds the <see cref="XmlQualifiedName"/> instance with the specified
    /// namespace and local name.
    /// </summary>
    /// <param name="ns">
    /// The namespace of the qualified name.
    /// </param>
    /// <param name="localName">
    /// The local name of the qualified name.
    /// </param>
    /// <returns>
    /// The <see cref="XmlQualifiedName"/> instance if found; otherwise, <see
    /// langword="null"/>.
    /// </returns>
    public XmlQualifiedName? Find(string ns, string localName)
    {
        return ns.Length is 0
            ? NoNamespace.Get(localName)
            : NamespaceMap.Get(ns) is not {} namespaceMap
            ? null
            : namespaceMap.LocalNameMap.Get(localName);
    }

    /// <summary>
    /// Creates a new instance of <see cref="QualifiedNameMap"/>.
    /// </summary>
    /// <returns>A new <see cref="QualifiedNameMap"/> instance.</returns>
    private static QualifiedNameMap NewQualifiedNameMap()
        => new();

    /// <summary>
    /// Represents a map for managing qualified names within a namespace.
    /// </summary>
    private class QualifiedNameMap
    {
        /// <summary>
        /// Gets the map for local names within the namespace.
        /// </summary>
        public InternMap<string, XmlQualifiedName> LocalNameMap { get; }
            = new();
    }
}
