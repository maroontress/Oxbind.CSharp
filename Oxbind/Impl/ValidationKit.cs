namespace Maroontress.Oxbind.Impl;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using Maroontress.Oxbind.Util;
using StyleChecker.Annotations;

/// <summary>
/// Validates the semantics of Oxbind attributes.
/// </summary>
/// <param name="journal">
/// The <see cref="Journal"/> instance.
/// </param>
/// <param name="nameBank">
/// The <see cref="QNameBank"/> instance used to intern XML qualified names.
/// </param>
public sealed class ValidationKit(Journal journal, QNameBank nameBank)
{
    private static IReadOnlyList<Type> ForAttributeExclusiveList { get; }
        = [.. ChildParameter.AttributeTypes, typeof(ForTextAttribute)];

    private static IReadOnlyList<Type> ChildAttributeList { get; }
        = [.. ChildParameter.AttributeTypes];

    private Journal Journal { get; } = journal;

    private QNameBank NameBank { get; } = nameBank;

    /// <summary>
    /// Checks the constructor parameters attributed with <see
    /// cref="ForAttributeAttribute"/>, ensuring there are no duplicate
    /// attribute names, that types are valid, and that <see
    /// cref="ForAttributeAttribute"/> is not combined with other mutually
    /// exclusive attributes. Logs errors for any violations found.
    /// </summary>
    /// <param name="attributeParameterList">
    /// The list of <see cref="AttributeParameter"/> objects representing
    /// constructor parameters attributed with <see
    /// cref="ForAttributeAttribute"/>.
    /// </param>
    public void CheckAttributes(
        IReadOnlyList<AttributeParameter> attributeParameterList)
    {
        /*
            Checks for duplicate attribute names from [ForAttribute]
            annotations.
        */
        var group = attributeParameterList.GroupBy(
                x => x.Name,
                x => x.Info.Name ?? "(no name)")
            .Where(p => p.Count() is not 1)
            .ToList();
        foreach (var i in group)
        {
            var names = Names.SortAndJoin(i);
            Journal.Error("duplicated_attribute_name", i.Key, names);
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
            t => Journal.Error(
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
            t => Journal.Error(
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
    /// <param name="childParameterList">
    /// The list of constructor parameters to validate as child elements.
    /// </param>
    /// <returns>
    /// The <see cref="Dependency"/> object representing the child element
    /// dependencies.
    /// </returns>
    public Dependency CheckChildren(
        IReadOnlyList<ParameterInfo> childParameterList)
    {
        static ForAttributeAttribute? ToForAttribute(ParameterInfo info)
            => info.GetCustomAttribute<ForAttributeAttribute>();

        static ForTextAttribute? ToForText(ParameterInfo info)
            => info.GetCustomAttribute<ForTextAttribute>();

        var forTexts = childParameterList.Where(p => ToForText(p) is { })
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
                p => ToForAttribute(p) is { })
            .ToList();
        if (attributeList.Count is not 0)
        {
            Journal.Error(
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
            Journal.Error(
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
            .Except(attributeList)
            .ToList();
        if (noAnnotationList.Count is not 0)
        {
            Journal.Error(
                """
                parameter_must_be_annotated_with_attributes_for_child_elements
                """,
                Names.OfParameters(noAnnotationList));
        }

        /*
            Checks if [ForText] is used in the same constructor as child
            element attributes ([Required], [Optional], [Multiple]).

            For example: Foo([Required] Bar bar, [ForText] Baz baz)
                {...}
        */
        var onlyForChildren = forChildren.Except(intersection)
            .ToList();
        var onlyForText = forTexts.Except(intersection)
            .ToList();
        if (onlyForChildren.Count is not 0
            && onlyForText.Count is not 0)
        {
            Journal.Error(
                """
                parameters_must_not_be_mixed
                """,
                Names.OfParameters(onlyForText),
                Names.OfParameters(onlyForChildren));
        }
        CheckForText(forTexts);
        var childParameters = CheckForChildren(forChildren);
        return forTexts.Count is not 0
            ? Dependency.InnerText
            : Dependency.Of(childParameters);
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
    /// Validates the constructor parameters marked with the <see
    /// cref="ForTextAttribute"/> annotation.
    /// </summary>
    /// <param name="parameterList">
    /// The constructor parameters marked with the annotation <see
    /// cref="ForTextAttribute"/>.
    /// </param>
    private void CheckForText(IReadOnlyList<ParameterInfo> parameterList)
    {
        /*
            Checks if there are two or more parameters annotated with
            [ForText].
        */
        if (parameterList.Count > 1)
        {
            Journal.Error(
                "duplicated_ForText", Names.OfParameters(parameterList));
        }

        /*
            Checks that the type of the parameter annotated with [ForText] is
            either string or BindResult<string>.
        */
        Elements.IfNotEmpty(
            parameterList.Where(
                p => !StringSugarcoaters.IsValid(p.ParameterType)),
            t => Journal.Error(
                "type_mismatch_ForText", Names.OfParameters(t)));
    }

    /// <summary>
    /// Validates the constructor parameters marked with the annotation <see
    /// cref="RequiredAttribute"/>, <see cref="OptionalAttribute"/>, <see
    /// cref="MultipleAttribute"/>.
    /// </summary>
    /// <param name="parameterList">
    /// The constructor parameters marked with the annotation <see
    /// cref="RequiredAttribute"/>, <see cref="OptionalAttribute"/>, <see
    /// cref="MultipleAttribute"/>.
    /// </param>
    /// <returns>
    /// An enumerable collection of <see cref="ChildParameter"/> objects that
    /// represent the child element dependencies.
    /// </returns>
    private IEnumerable<ChildParameter> CheckForChildren(
        IReadOnlyList<ParameterInfo> parameterList)
    {
        Elements.IfNotEmpty(
            parameterList.Where(
                p => p.GetCustomAttributes()
                    .Select(a => a.GetType())
                    .Intersect(ChildAttributeList)
                    .Count() > 1),
            t => Journal.Error(
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
            p => p.GetCustomAttribute<MultipleAttribute>() is { });
        foreach (var g in requiredOrNotGroup)
        {
            var (predicate, messageKey) = g.Key
                ? ((Func<ParameterInfo, bool>)(p => !IsIEnumerableT(p)),
                    "parameter_type_must_be_IEnumerableT")
                : (IsIEnumerableT,
                    "parameter_type_must_not_be_IEnumerableT");
            Elements.IfNotEmpty(
                g.Where(predicate),
                t => Journal.Error(messageKey, Names.OfParameters(t)));
        }

        /*
            Checks that the unit type of each child parameter is annotated with
            [ForElement].
        */
        Func<ParameterInfo, ChildParameter> ToChildParameter()
            => p => ChildParameter.Of(p, NameBank);

        var dependencies = parameterList.Select(ToChildParameter())
            .ToList();
        Elements.IfNotEmpty(
            dependencies.Where(p => !IsElementClass(p.UnitType))
                .Select(p => p.Info),
            t => Journal.Error(
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
            var foo = new ChildParameterOrder(pairList, Journal);
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
    /// <param name="journal">
    /// The <see cref="Journal"/> instance used to log errors when the order of
    /// child parameters is invalid.
    /// </param>
    private class ChildParameterOrder(
        IEnumerable<ChildParameterPair> pairs, Journal journal)
    {
        private List<ChildParameterPair> PairList { get; } = [.. pairs];

        private Journal Logger { get; } = journal;

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
