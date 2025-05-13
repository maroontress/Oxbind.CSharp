namespace Maroontress.Oxbind;

using System;

/// <summary>
/// Marks a constructor that Oxbind should ignore during deserialization.
/// </summary>
/// <remarks>
/// <para>
/// When a class defines multiple constructors, Oxbind needs to determine which
/// one to use for deserialization. Annotate constructors that Oxbind should
/// not use with this attribute.
/// </para>
/// <para>
/// If a class has multiple constructors and Oxbind cannot unambiguously
/// determine which one to use (i.e., more than one constructor is not
/// annotated with <see cref="IgnoredAttribute"/>), a runtime error occurs.
/// </para>
/// </remarks>
[AttributeUsage(
    AttributeTargets.Constructor,
    Inherited = false,
    AllowMultiple = false)]
public sealed class IgnoredAttribute : Attribute;
