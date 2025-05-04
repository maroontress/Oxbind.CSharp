namespace Maroontress.Oxbind;

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Maroontress.Oxbind.Impl;
using Maroontress.Oxbind.Util;
using Maroontress.Oxbind.Util.Graph;

/// <summary>
/// The factory of the <see cref="Oxbinder{T}"/> object.
/// </summary>
public sealed class OxbinderFactory
{
    /// <summary>
    /// The cache of the metadata.
    /// </summary>
    private readonly InternMap<Type, Metadata> metadataCache;

    /// <summary>
    /// The cache of the validated classes.
    /// </summary>
    private readonly Traversal<Type> validationTraversal;

    /// <summary>
    /// The cache of the classes that have been checked for DAG.
    /// </summary>
    private readonly DagChecker<Type> dagChecker;

    /// <summary>
    /// Initializes a new instance of the <see cref="OxbinderFactory"/>
    /// class.
    /// </summary>
    /// <param name="ignoreWarnings">
    /// If the value is <c>true</c>, the <see
    /// cref="OxbinderFactory.Of{T}"/> ignores warning messages to the
    /// annotations of type <c>T</c>. Otherwise, the warning messages are
    /// treated as errors, and then the method throws <see
    /// cref="BindException"/>.
    /// </param>
    public OxbinderFactory(bool ignoreWarnings = false)
    {
        metadataCache = new InternMap<Type, Metadata>();
        validationTraversal = new Traversal<Type>(type =>
        {
            var v = new Validator(type);
            var messages = v.GetMessages();
            if (!v.IsValid
                || (!ignoreWarnings && messages.Any()))
            {
                var log = string.Join(
                    Environment.NewLine, messages);
                throw new BindException(
                    $"{type.Name} has failed to validate annotations: "
                        + $"{log}");
            }
            return v.SchemaClasses;
        });
        dagChecker = new DagChecker<Type>(Validator.GetDependencies);
    }

    /// <summary>
    /// Creates an <see cref="Oxbinder{T}"/> object for the specified
    /// class.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the class annotated with <see
    /// cref="ForElementAttribute"/> that stands for the root element.
    /// </typeparam>
    /// <returns>
    /// The <see cref="Oxbinder{T}"/> object to create new objects of the
    /// specified class with XML streams.
    /// </returns>
    /// <seealso cref="Oxbinder{T}.NewInstance(TextReader)"/>
    public Oxbinder<T> Of<T>()
        where T : class
    {
        return new OxBinderImpl<T>(GetSharedMetadata);
    }

    /// <summary>
    /// Returns the shared <see cref="Metadata"/> object for the specified
    /// class.
    /// </summary>
    /// <param name="type">The class corresponding to the element.</param>
    /// <returns>
    /// The <see cref="Metadata"/> object for the specified class.
    /// </returns>
    private Metadata GetSharedMetadata(Type type)
    {
        return metadataCache.Intern(type, () =>
        {
            validationTraversal.Visit(type);
            try
            {
                dagChecker.Check(type);
            }
            catch (CircularDependencyException e)
            {
                throw new BindException(
                    $"{type.Name} has circular dependency.", e);
            }
            var fields
                = Classes.GetInstanceFields<ForTextAttribute>(type);
            var methods
                = Classes.GetInstanceMethods<FromTextAttribute>(type);
            if (fields.Any() && methods.Any())
            {
                Debug.Fail(fields.First() + " and " + methods.First());
            }
            return fields.Any() ? new TextMetadata(type, fields)
                : methods.Any() ? new TextMetadata(type, methods)
                : new SchemaMetadata(type) as Metadata;
        });
    }
}
