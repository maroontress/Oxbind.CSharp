namespace Maroontress.Oxbind.Impl;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;

/// <summary>
/// Validates the semantics of Oxbind attributes on classes and constructor
/// parameters.
/// </summary>
public sealed class Validator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Validator"/> class.
    /// </summary>
    /// <param name="clazz">
    /// The type of the class attributed with <see
    /// cref="ForElementAttribute"/>.
    /// </param>
    /// <param name="journal">
    /// The <see cref="Journal"/> instance.
    /// </param>
    /// <param name="nameBank">
    /// The <see cref="QNameBank"/> instance used to intern XML qualified
    /// names.
    /// </param>
    public Validator(Type clazz, Journal journal, QNameBank nameBank)
    {
        BuildSpec NoConstructor()
        {
            journal.Error("must_have_one_valid_constructor");
            return new(null, [], Dependency.Empty);
        }

        BuildSpec HaveConstructor(ConstructorInfo ctor)
        {
            var kit = new ValidationKit(journal, nameBank);
            var attributeParameterList = AttributeParameter.Of(ctor, nameBank)
                .ToList();
            kit.CheckAttributes(attributeParameterList);
            var childParameterList = ctor.GetParameters()
                .Skip(attributeParameterList.Count)
                .ToList();
            var dependency = kit.CheckChildren(childParameterList);
            return new(ctor, attributeParameterList, dependency);
        }

        XmlQualifiedName NoElementNameSpecified()
        {
            journal.Error("must_be_annotated_with_ForElement");
            return XmlQualifiedName.Empty;
        }

        if (clazz.GetTypeInfo().IsInterface)
        {
            journal.Error("must_not_be_interface");
        }
        Logger = journal;
        var qualifiedName = GetElementName(clazz) is not {} elementName
            ? NoElementNameSpecified()
            : elementName;
        ElementName = nameBank.Intern(qualifiedName);
        (Constructor, AttributeParameters, ChildDependency)
                = Types.GetConstructor(clazz) is not {} ctor
            ? NoConstructor()
            : HaveConstructor(ctor);
    }

    /// <summary>
    /// Gets the <see cref="Journal"/> instance.
    /// </summary>
    public Journal Logger { get; }

    /// <summary>
    /// Gets the <see cref="ConstructorInfo"/> for the constructor of the
    /// validated class that has been determined to be used for binding, or
    /// <c>null</c> if no suitable constructor is found.
    /// </summary>
    public ConstructorInfo? Constructor { get; }

    /// <summary>
    /// Gets the XML element name from the <see cref="ForElementAttribute"/>
    /// annotation on the class.
    /// </summary>
    public XmlQualifiedName ElementName { get; }

    /// <summary>
    /// Gets a value indicating whether this validator has detected errors.
    /// </summary>
    public bool IsValid => !Logger.HasError;

    /// <summary>
    /// Gets the collection of the constructor parameters attributed with <see
    /// cref="ForAttributeAttribute"/>.
    /// </summary>
    public IEnumerable<AttributeParameter> AttributeParameters { get; }

    /// <summary>
    /// Gets the <see cref="Dependency"/> object representing the child element
    /// dependencies of the validated class.
    /// </summary>
    public Dependency ChildDependency { get; }

    private static IReadOnlyList<Type> ChildAttributeList { get; }
        = [.. ChildParameter.AttributeTypes];

    private static IReadOnlyList<Type> ForAttributeExclusiveList { get; }
        = [.. ChildParameter.AttributeTypes, typeof(ForTextAttribute)];

    /// <summary>
    /// Returns the XML element name marked with the annotation <see
    /// cref="ForElementAttribute"/>.
    /// </summary>
    /// <param name="clazz">
    /// The type to test.
    /// </param>
    /// <returns>
    /// The XML element name marked with the <see cref="ForElementAttribute"/>
    /// annotation; <c>null</c> otherwise.
    /// </returns>
    private static XmlQualifiedName? GetElementName(Type clazz)
        => clazz.GetTypeInfo()
            .GetCustomAttributes<ForElementAttribute>()
            .Select(a => a.QName)
            .FirstOrDefault();

    /// <summary>
    /// Represents the specification of a class constructor, including the
    /// constructor itself, the parameters annotated with <see
    /// cref="ForAttributeAttribute"/>, and the child element dependencies.
    /// </summary>
    /// <param name="Constructor">
    /// The <see cref="ConstructorInfo"/> of the constructor that has been
    /// validated for correct usage, or <c>null</c> if no valid constructor
    /// was found.
    /// </param>
    /// <param name="AttributeParameters">
    /// The collection of <see cref="AttributeParameter"/> objects representing
    /// the constructor parameters annotated with <see
    /// cref="ForAttributeAttribute"/>.
    /// </param>
    /// <param name="ChildDependency">
    /// The <see cref="Dependency"/> object representing the child element
    /// dependencies of the validated class.
    /// </param>
    private record struct BuildSpec(
        ConstructorInfo? Constructor,
        IEnumerable<AttributeParameter> AttributeParameters,
        Dependency ChildDependency);
}
