namespace Maroontress.Oxbind;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Maroontress.Oxbind.Impl;
using Maroontress.Oxbind.Util;
using Maroontress.Oxbind.Util.Graph;

/// <summary>
/// A factory for <see cref="Oxbinder{T}"/> objects.
/// </summary>
public sealed class OxbinderFactory
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OxbinderFactory"/> class.
    /// </summary>
    /// <param name="ignoreWarnings">
    /// If <c>true</c>, the <see cref="Of{T}"/> method ignores warning
    /// messages related to attributes on the target types and their
    /// dependencies. Otherwise, the method treats warnings as errors,
    /// causing a <see cref="BindException"/>.
    /// </param>
    public OxbinderFactory(bool ignoreWarnings = false)
    {
        static IEnumerable<Type> ToDependingTypes(Validator v)
            => v.ChildDependency
                .ChildParameters
                .Where(p => p.SchemaType is RequiredSchemaType)
                .Select(p => p.UnitType);

        ValidationTraversal = new(type =>
        {
            var validator = ValidatorCache.Intern(
                type,
                () => new(type, new(type.Name)));
            var logger = validator.Logger;
            var messages = logger.GetMessages();
            if (!validator.IsValid
                || (!ignoreWarnings && messages.Any()))
            {
                var m = string.Join(Environment.NewLine, messages);
                throw new BindException(
                    $"{type.Name} failed to validate annotations: {m}");
            }
            var dependencies = ToDependingTypes(validator);
            return [.. dependencies];
        });
        DagChecker = new(
            type => ValidatorCache.Get(type) is {} validator
                ? new HashSet<Type>(ToDependingTypes(validator))
                : throw new NullReferenceException($"{type}"),
            x => x.Name);
    }

    /// <summary>
    /// Gets the traversal mechanism that validates and caches types.
    /// </summary>
    private Traversal<Type> ValidationTraversal { get; }

    /// <summary>
    /// Gets the cache of types that have been verified as forming a Directed
    /// Acyclic Graph (DAG).
    /// </summary>
    private DagChecker<Type> DagChecker { get; }

    /// <summary>
    /// Gets the cache of the metadata.
    /// </summary>
    private InternMap<Type, Metadata> MetadataCache { get; } = new();

    /// <summary>
    /// Gets the cache of the validator.
    /// </summary>
    private InternMap<Type, Validator> ValidatorCache { get; } = new();

    /// <summary>
    /// Creates an <see cref="Oxbinder{T}"/> object for the specified class.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the class annotated with <see cref="ForElementAttribute"/>
    /// that represents the root element.
    /// </typeparam>
    /// <returns>
    /// The <see cref="Oxbinder{T}"/> object to create new objects of the
    /// specified class from XML streams.
    /// </returns>
    /// <remarks>
    /// When this method is called, the factory validates the attribute schema
    /// and dependency graph for the specified type <typeparamref name="T"/>
    /// and all its transitively referenced types (i.e., types of constructor
    /// parameters annotated with <see cref="RequiredAttribute"/>, <see
    /// cref="OptionalAttribute"/>, or <see cref="MultipleAttribute"/>). If any
    /// validation fails or a circular dependency is detected, a <see
    /// cref="BindException"/> is thrown.
    /// </remarks>
    /// <exception cref="BindException">
    /// Thrown when binding validation fails for the target type <typeparamref
    /// name="T"/> or any of its transitively referenced types (e.g., due to a
    /// type mismatch, a missing required element or attribute, or any other
    /// binding rule violation).
    /// </exception>
    /// <seealso cref="Oxbinder{T}.NewInstance(TextReader)"/>
    public Oxbinder<T> Of<T>()
        where T : class
    {
        _ = GetSharedMetadata(typeof(T));
        return new OxBinderImpl<T>(GetSharedMetadata);
    }

    /// <summary>
    /// Returns the shared <see cref="Metadata"/> object for the specified
    /// type.
    /// </summary>
    /// <param name="type">
    /// The type corresponding to an XML element.
    /// </param>
    /// <returns>
    /// The <see cref="Metadata"/> object for the specified type.
    /// </returns>
    private Metadata GetSharedMetadata(Type type)
    {
        return MetadataCache.Intern(type, () => NewMetadata(type));
    }

    private Metadata NewMetadata(Type type)
    {
        ValidationTraversal.Visit(type);
        try
        {
            DagChecker.Check(type);
        }
        catch (CircularDependencyException e)
        {
            throw new BindException(
                $"{type.Name} has a circular dependency: " + e.Message, e);
        }
        var validator = ValidatorCache.Get(type)
            ?? throw new NullReferenceException($"{type}");
        if (validator.Constructor is not {} ctor)
        {
            // Something was missed during the validation
            throw new InvalidOperationException($"{type}");
        }
        var bank = new AttributeBank(
            ctor,
            validator.ElementName,
            validator.AttributeParameters);

        var dependency = validator.ChildDependency;
        if (dependency.HasInnerText)
        {
            var parameters = ctor.GetParameters();
            var firstChildIndex = validator.AttributeParameters.Count();
            var firstChild = parameters[firstChildIndex];
            return new TextMetadata(bank, firstChild);
        }
        return new SchemaMetadata(bank, dependency.ChildParameters);
    }
}
