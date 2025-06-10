namespace Maroontress.Oxbind.Impl;

using System;
using System.Xml;

/// <summary>
/// Represents a utility for creating instances of a type with metadata.
/// </summary>
/// <typeparam name="T">
/// The type of the value.
/// </typeparam>
/// <param name="NewInstance">
/// A function that transforms or wraps a raw deserialized value, often to
/// include metadata such as line information.
/// </param>
/// <param name="NewLineInfo">
/// A function that creates and returns a new <see cref="IXmlLineInfo"/>
/// object with a given XML reader.
/// </param>
public record struct Sugarcoater<T>(
    Func<IXmlLineInfo, T, object> NewInstance,
    Func<XmlReader, IXmlLineInfo> NewLineInfo);
