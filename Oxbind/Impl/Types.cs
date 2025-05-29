namespace Maroontress.Oxbind.Impl;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

/// <summary>
/// Provides utility methods for type reflection and manipulation, particularly
/// for types used in Oxbind.
/// </summary>
public static class Types
{
    /// <summary>
    /// Gets the type of <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static Type IEnumerableT { get; } = typeof(IEnumerable<>);

    /// <summary>
    /// Gets the type of <see cref="BindResult{T}"/>.
    /// </summary>
    public static Type BindResultT { get; } = typeof(BindResult<>);

    /// <summary>
    /// Gets the type of <see cref="BindResultImpl{T}"/>.
    /// </summary>
    public static Type BindResultImplT { get; } = typeof(BindResultImpl<>);

    /// <summary>
    /// Gets the type of <see cref="string"/>.
    /// </summary>
    public static Type String { get; } = typeof(string);

    /// <summary>
    /// Gets the type of <see cref="void"/>.
    /// </summary>
    public static Type Void { get; } = typeof(void);

    /// <summary>
    /// Gets the type of <see cref="BindResult{T}"/> (<c>T</c> is
    /// <c>string</c>).
    /// </summary>
    public static Type BindResultString { get; } = typeof(BindResult<string>);

    /// <summary>
    /// Retrieves the constructor of the specified class that is not annotated
    /// with the <see cref="IgnoredAttribute"/>.
    /// </summary>
    /// <param name="clazz">
    /// The type of the class whose constructor is to be retrieved.
    /// </param>
    /// <returns>
    /// The <see cref="ConstructorInfo"/> of the constructor that is not
    /// annotated with the <see cref="IgnoredAttribute"/>, or <c>null</c> if no
    /// such constructor exists.
    /// </returns>
    public static ConstructorInfo? GetConstructor(Type clazz)
        => clazz.GetConstructors()
            .FirstOrDefault(HasNotIgnored);

    /// <summary>
    /// Gets the type of the first type parameter of the specified generic
    /// type.
    /// </summary>
    /// <param name="t">
    /// The generic type.
    /// </param>
    /// <returns>
    /// The type of the first type parameter.
    /// </returns>
    public static Type FirstInnerType(Type t)
        => t.GenericTypeArguments[0];

    /// <summary>
    /// Gets whether the raw type of the specified type is equal to the
    /// specified expected type.
    /// </summary>
    /// <param name="type">
    /// The generic type.
    /// </param>
    /// <param name="expectedType">
    /// The expected open generic type (e.g.,
    /// <c>typeof(IEnumerable&lt;&gt;)</c>).
    /// </param>
    /// <returns>
    /// <c>true</c> if <paramref name="type"/> is a generic type and if its
    /// generic type definition equals <paramref name="expectedType"/>, or if
    /// <paramref name="type"/> is not a generic type and it equals <paramref
    /// name="expectedType"/>; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsRawType(Type type, Type expectedType)
        => ToRawType(type).Equals(expectedType);

    private static Type ToRawType(Type t)
    {
        var info = t.GetTypeInfo();
        return info.IsGenericType
            ? info.GetGenericTypeDefinition() : t;
    }

    private static bool HasNotIgnored(ConstructorInfo info)
        => info.GetCustomAttribute<IgnoredAttribute>() is not {};
}
