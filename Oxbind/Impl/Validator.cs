namespace Maroontress.Oxbind.Impl;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using Maroontress.Oxbind.Util;
using StyleChecker.Annotations;

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
    /// <param name="logger">
    /// The logger instance.
    /// </param>
    public Validator(Type clazz, Journal logger)
    {
        BuildSpec NoConstructor()
        {
            logger.Error("must_have_one_valid_constructor");
            return new(null, [], Dependency.Empty);
        }

        BuildSpec HaveConstructor(ConstructorInfo ctor)
        {
            var attributeParameterList = AttributeParameter.Of(ctor)
                .ToList();
            CheckAttributes(logger, attributeParameterList);
            var childParameterList = ctor.GetParameters()
                .Skip(attributeParameterList.Count)
                .ToList();
            var dependency = CheckChildren(logger, childParameterList);
            return new(ctor, attributeParameterList, dependency);
        }

        XmlQualifiedName NoElementNameSpecified()
        {
            logger.Error("must_be_annotated_with_ForElement");
            return XmlQualifiedName.Empty;
        }

        if (clazz.GetTypeInfo().IsInterface)
        {
            logger.Error("must_not_be_interface");
        }
        ValidationLogger = logger;
        ElementName = GetElementName(clazz) is not {} elementName
            ? NoElementNameSpecified()
            : elementName;
        (Constructor, AttributeParameters, ChildDependency)
                = Types.GetConstructor(clazz) is not {} ctor
            ? NoConstructor()
            : HaveConstructor(ctor);
    }

    /// <summary>
    /// Gets the logger instance.
    /// </summary>
    public Journal ValidationLogger { get; }

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
    public bool IsValid => !ValidationLogger.HasError;

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
    /// Returns a new set of types that the specified class depends on.
    /// </summary>
    /// <param name="clazz">
    /// The type of the class.
    /// </param>
    /// <returns>
    /// A new set containing the types of constructor parameters attributed
    /// with <see cref="RequiredAttribute"/> in <paramref name="clazz"/>.
    /// </returns>
    public static ISet<Type> GetDependencies(Type clazz)
    {
        static RequiredAttribute? ToRequired(ParameterInfo p)
            => p.GetCustomAttribute<RequiredAttribute>();

        static IEnumerable<Type> GetRequiredTypes(ConstructorInfo ctor)
            => ctor.GetParameters()
                .Where(p => ToRequired(p) is {})
                .Select(m => m.ParameterType);

        return Types.GetConstructor(clazz) is not {} ctor
            ? []
            : new HashSet<Type>(GetRequiredTypes(ctor));
    }

    /// <summary>
    /// Checks if the specified class is annotated with <see
    /// cref="ForElementAttribute"/>.
    /// </summary>
    /// <param name="clazz">
    /// The type to test.
    /// </param>
    /// <returns>
    /// <c>true</c> if the specified type is annotated with <see
    /// cref="ForElementAttribute"/>; otherwise, <c>false</c>.
    /// </returns>
    private static bool IsElementClass(Type clazz)
        => clazz.GetTypeInfo()
            .GetCustomAttributes<ForElementAttribute>()
            .Any();

    /// <summary>
    /// Returns the XML element name marked with the annotation <see
    /// cref="ForElementAttribute"/>.
    /// </summary>
    /// <param name="clazz">
    /// The type to test.
    /// </param>
    /// <returns>
    /// The XML element name marked with the annotation <see
    /// cref="ForElementAttribute"/>, <c>null</c> otherwise.
    /// </returns>
    private static XmlQualifiedName? GetElementName(Type clazz)
        => clazz.GetTypeInfo()
            .GetCustomAttributes<ForElementAttribute>()
            .Select(a => a.QName)
            .FirstOrDefault();

    /// <summary>
    /// Checks the constructor parameters attributed with <see
    /// cref="ForAttributeAttribute"/>, ensuring there are no duplicate
    /// attribute names, that types are valid, and that <see
    /// cref="ForAttributeAttribute"/> is not combined with other mutually
    /// exclusive attributes. Logs errors for any violations found.
    /// </summary>
    /// <param name="logger">
    /// The <see cref="Journal"/> instance used to record validation errors.
    /// </param>
    /// <param name="attributeParameterList">
    /// The list of <see cref="AttributeParameter"/> objects representing
    /// constructor parameters attributed with <see
    /// cref="ForAttributeAttribute"/>.
    /// </param>
    private static void CheckAttributes(
        Journal logger,
        IReadOnlyList<AttributeParameter> attributeParameterList)
    {
        /*
            Checks the duplication with the attribute name of the
            [ForAttribute].
        */
        var group = attributeParameterList.GroupBy(
                x => x.Name,
                x => x.Info.Name ?? "(no name)")
            .Where(p => p.Count() is not 1)
            .ToList();
        foreach (var i in group)
        {
            var names = Names.SortAndJoin(i);
            logger.Error("duplicated_attribute_name", i.Key, names);
        }

        /*
            Checks that the type of the parameter annotated with [ForAttribute]
            is either string or BindResult<string>.
        */
        static bool IsValidType(Type t)
            => StringSugarcoaters.IsValid(t);

        static bool IsInvalidForAttributeParameter(ParameterInfo p)
            => !IsValidType(p.ParameterType);

        Elements.IfNotEmpty(
            attributeParameterList.Select(x => x.Info)
                .Where(IsInvalidForAttributeParameter),
            t => logger.Error(
                "type_mismatch_ForAttribute", Names.OfParameters(t)));

        /*
            Checks that the parameter annotated with [ForAttribute] is not
            annotated with other attributes: [Required], [Optional],
            [Multiple], [ForText].
        */
        Elements.IfNotEmpty(
            attributeParameterList.Select(x => x.Info)
                .Where(p =>
                {
                    var typeSet = new HashSet<Type>(
                        p.GetCustomAttributes()
                            .Select(a => a.GetType()));
                    typeSet.IntersectWith(ForAttributeExclusiveList);
                    return typeSet.Count > 0;
                }),
            t => logger.Error(
                """
                parameter_must_not_be_annotated_with_both_ForAttribute_and_another
                """,
                Names.OfParameters(t)));
    }

    /// <summary>
    /// Validates the constructor parameters that represent child elements.
    /// Performs several checks to ensure correct usage of Oxbind child-related
    /// attributes, such as [ForText], [Required], [Optional], and [Multiple].
    /// Logs errors for invalid combinations or missing annotations.
    /// </summary>
    /// <param name="logger">
    /// The <see cref="Journal"/> instance used to record validation errors.
    /// </param>
    /// <param name="childParameterList">
    /// The list of constructor parameters to validate as child elements.
    /// </param>
    /// <returns>
    /// The <see cref="Dependency"/> object representing the child element
    /// dependencies.
    /// </returns>
    private static Dependency CheckChildren(
        Journal logger,
        IReadOnlyList<ParameterInfo> childParameterList)
    {
        var forTexts = childParameterList.Where(
                p => p.GetCustomAttribute<ForTextAttribute>() is { })
            .ToList();
        var forChildren = childParameterList.Where(
                p => p.GetCustomAttributes()
                    .Select(a => a.GetType())
                    .Intersect(ChildAttributeList)
                    .Any())
            .ToList();

        /*
            Checks that constructor parameters annotated with [ForAttribute]
            appear consecutively at the beginning.

            For example: Foo([Attribute] string bar, [Required] Baz baz,
                [Attribute] string qux)
                {...}
        */
        var attributeList = childParameterList.Where(
                p => p.GetCustomAttribute<ForAttributeAttribute>() is { })
            .ToList();
        if (attributeList.Count is not 0)
        {
            logger.Error(
                """
                parameters_with_ForAttribute_must_be_listed_consecutively_at_the_beginning
                """,
                Names.OfParameters(attributeList));
        }

        /*
            Checks if a constructor parameter is annotated with both [ForText]
            and one of the child element attributes: [Required], [Optional],
            [Multiple].

            For example: Foo([Required][ForText] Bar bar)
                {...}
        */
        var intersection = forTexts.Intersect(forChildren)
            .ToList();
        if (intersection.Count is not 0)
        {
            logger.Error(
                """
                parameter_must_not_be_annotated_with_both_ForText_and_another
                """,
                Names.OfParameters(intersection));
        }

        /*
            Checks for constructor parameters (after attribute parameters) that
            are not annotated with any child-binding attribute ([ForText],
            [Required], [Optional], [Multiple]).

            For example: Foo(Bar bar)
                {...}
        */
        var noAnnotationList = childParameterList.Except(forChildren)
            .Except(forTexts)
            .ToList();
        if (noAnnotationList.Count is not 0)
        {
            logger.Error(
                """
                parameter_must_be_annotated_with_attributes_for_child_elements
                """,
                Names.OfParameters(intersection));
        }

        /*
            Checks if [ForText] is used in the same constructor as child
            element attributes ([Required], [Optional], [Multiple]).

            For example: Foo([Required] Bar bar, [ForText] Baz baz)
                {...}
        */
        if (forChildren.Count is not 0
            && forTexts.Count is not 0)
        {
            logger.Error(
                """
                parameters_must_not_be_mixed
                """,
                Names.OfParameters(forTexts),
                Names.OfParameters(forChildren));
        }
        CheckForText(logger, forTexts);
        var childParameters = CheckForChildren(logger, forChildren);
        return forTexts.Count is not 0
            ? Dependency.InnerText
            : Dependency.Of(childParameters);
    }

    /// <summary>
    /// Validates the constructor parameters marked with the annotation <see
    /// cref="RequiredAttribute"/>, <see cref="OptionalAttribute"/>, <see
    /// cref="MultipleAttribute"/>.
    /// </summary>
    /// <param name="logger">
    /// The <see cref="Journal"/> instance used to record validation errors.
    /// </param>
    /// <param name="parameterList">
    /// The constructor parameters marked with the annotation <see
    /// cref="RequiredAttribute"/>, <see cref="OptionalAttribute"/>, <see
    /// cref="MultipleAttribute"/>.
    /// </param>
    /// <returns>
    /// An enumerable collection of <see cref="ChildParameter"/> objects that
    /// represent the child element dependencies.
    /// </returns>
    private static IEnumerable<ChildParameter> CheckForChildren(
        Journal logger,
        IReadOnlyList<ParameterInfo> parameterList)
    {
        Elements.IfNotEmpty(
            parameterList.Where(
                p => p.GetCustomAttributes()
                    .Select(a => a.GetType())
                    .Intersect(ChildAttributeList)
                    .Count() > 1),
            t => logger.Error(
                """
                parameter_for_child_elements_must_be_mutually_exclusive
                """,
                Names.OfParameters(t)));

        /*
            Checks that the parameter type is not IEnumerable<T> when the
            parameter is marked with [Multiple], or vice versa.
        */
        static bool IsIEnumerableT(ParameterInfo p)
            => Types.IsRawType(p.ParameterType, Types.IEnumerableT);

        var requiredOrNotGroup = parameterList.GroupBy(
            p => p.GetCustomAttribute<MultipleAttribute>() is {});
        foreach (var g in requiredOrNotGroup)
        {
            var (predicate, messageKey) = g.Key
                ? ((Func<ParameterInfo, bool>)(p => !IsIEnumerableT(p)),
                    "parameter_type_must_be_IEnumerableT")
                : (IsIEnumerableT,
                    "parameter_type_must_not_be_IEnumerableT");
            Elements.IfNotEmpty(
                g.Where(predicate),
                t => logger.Error(messageKey, Names.OfParameters(t)));
        }

        /*
            Checks that each parameter type is annotated with
            [ForElement].
        */
        var dependencies = parameterList.Select(ChildParameter.Of)
            .ToList();
        Elements.IfNotEmpty(
            dependencies.Where(p => !IsElementClass(p.UnitType))
                .Select(p => p.Info),
            t => logger.Error(
                """
                parameter_type_must_be_annotated_with_ForElement
                """,
                Names.OfParameters(t)));

        /*
            Checks additional restrictions on the order for parameters with
            the same element name.
        */
        static bool OptionalPredicate(SchemaType current, SchemaType next)
            => current is OptionalSchemaType
                && (next is RequiredSchemaType or MultipleSchemaType);

        static bool MultiplePredicate(
                SchemaType current,
                [Unused] SchemaType ignored)
            => current is MultipleSchemaType;

        var count = dependencies.Count;
        if (count > 1)
        {
            var currentList = dependencies.Take(count - 1);
            var nextList = dependencies.Skip(1);
            var pairList = currentList.Zip(
                nextList, (c, n) => new ChildParameterPair(c, n));
            var foo = new ChildParameterOrder(pairList, logger);
            foo.CheckSameElementName(
                OptionalPredicate,
                """
                optional_parameter_followed_by_the_one_that_has_the_same_element_name
                """);
            foo.CheckSameElementName(
                MultiplePredicate,
                """
                multiple_parameter_followed_by_the_one_that_has_the_same_element_name
                """);
        }

        return dependencies;
    }

    /// <summary>
    /// Validates the constructor parameters marked with the <see
    /// cref="ForTextAttribute"/> annotation.
    /// </summary>
    /// <param name="logger">
    /// The <see cref="Journal"/> instance used to record validation errors.
    /// </param>
    /// <param name="parameterList">
    /// The constructor parameters marked with the annotation <see
    /// cref="ForTextAttribute"/>.
    /// </param>
    private static void CheckForText(
        Journal logger,
        IReadOnlyList<ParameterInfo> parameterList)
    {
        /*
            Checks if there are two or more parameters annotated with
            [ForText].
        */
        if (parameterList.Count > 1)
        {
            logger.Error(
                "duplicated_ForText", Names.OfParameters(parameterList));
        }

        /*
            Checks that the type of the parameter annotated with [ForText] is
            either string or BindResult<string>.
        */
        Elements.IfNotEmpty(
            parameterList.Where(
                p => !StringSugarcoaters.IsValid(p.ParameterType)),
            t => logger.Error("type_mismatch_ForText", Names.OfParameters(t)));
    }

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

    /// <summary>
    /// Represents an XML element name and a string representation of the
    /// parameter names that are associated with the element name.
    /// </summary>
    /// <param name="ElementName">
    /// The XML element name that is shared by the child parameters.
    /// </param>
    /// <param name="ParameterNames">
    /// A comma-separated string of the names of parameters that share the same
    /// XML element name.
    /// </param>
    private record struct SameElementNamePair(
        XmlQualifiedName ElementName,
        string ParameterNames);

    /// <summary>
    /// Represents a pair of <see cref="ChildParameter"/> objects, where the
    /// first one is the current child parameter and the second one is the next
    /// child parameter in the sequence.
    /// </summary>
    /// <param name="Current">
    /// The current <see cref="ChildParameter"/> in the sequence.
    /// </param>
    /// <param name="Next">
    /// The next <see cref="ChildParameter"/> in the sequence.
    /// </param>
    private record struct ChildParameterPair(
        ChildParameter Current,
        ChildParameter Next);

    /// <summary>
    /// Represents the order of child parameters with the same element name,
    /// providing methods to check the order based on specified predicates and
    /// log errors if the order is invalid.
    /// </summary>
    /// <remarks>
    /// This class encapsulates the logic for checking the order of child
    /// parameters with the same element name and logging errors when the order
    /// does not meet the specified criteria.
    /// </remarks>
    /// <param name="pairs">
    /// An enumerable collection of <see cref="ChildParameterPair"/> objects
    /// representing pairs of child parameters to check.
    /// </param>
    /// <param name="logger">
    /// The <see cref="Logger"/> instance used to log errors when the order of
    /// child parameters is invalid.
    /// </param>
    private class ChildParameterOrder(
        IEnumerable<ChildParameterPair> pairs, Journal logger)
    {
        private List<ChildParameterPair> PairList { get; } = [.. pairs];

        private Journal Logger { get; } = logger;

        /// <summary>
        /// Checks the order of child parameters with the same element name
        /// and logs errors if the specified predicate is true for any
        /// consecutive pair.
        /// </summary>
        /// <param name="predicate">
        /// A function that takes the <see cref="SchemaType"/> of the current
        /// and next <see cref="ChildParameter"/> and returns <c>true</c> if
        /// the order is invalid.
        /// </param>
        /// <param name="resourceKey">
        /// The resource key for the error message to log when an invalid
        /// order is detected.
        /// </param>
        public void CheckSameElementName(
            Func<SchemaType, SchemaType, bool> predicate,
            string resourceKey)
        {
            IEnumerable<SameElementNamePair> ToPairs(ChildParameterPair p)
            {
                var (current, next) = p;
                var schemaType = current.SchemaType;
                var elementName = current.ElementName;
                var nextSchemaType = next.SchemaType;
                return (predicate(schemaType, nextSchemaType)
                        && elementName == next.ElementName
                        && !ReferenceEquals(
                            elementName, XmlQualifiedName.Empty))
                    ? [ToSameElementNamePair(elementName, current, next)]
                    : [];
            }

            void WriteErrors(IEnumerable<SameElementNamePair> all)
            {
                foreach (var (elementName, parameterNames) in all)
                {
                    Logger.Error(resourceKey, elementName, parameterNames);
                }
            }

            Elements.IfNotEmpty(PairList.SelectMany(ToPairs), WriteErrors);
        }

        private static SameElementNamePair ToSameElementNamePair(
            XmlQualifiedName elementName,
            ChildParameter current,
            ChildParameter next)
        {
            var parameterNames = Names.OfParameters(
                new[] { current, next }.Select(i => i.Info));
            return new(elementName, parameterNames);
        }
    }
}
