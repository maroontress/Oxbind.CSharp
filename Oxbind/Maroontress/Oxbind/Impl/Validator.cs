#pragma warning disable SA1122

namespace Maroontress.Oxbind.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using Maroontress.Util;

    /// <summary>
    /// Validates the semantics of the Oxbind annotations.
    /// </summary>
    public sealed class Validator : AbstractValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Validator"/> class.
        /// </summary>
        /// <param name="clazz">
        /// The class annotated with <see cref="ForElementAttribute"/>.
        /// </param>
        public Validator(Type clazz)
            : base(clazz.Name)
        {
            Class = clazz;
            if (!IsElementClass(clazz))
            {
                Error("must_be_annotated_with_ForElement");
            }
            if (clazz.GetTypeInfo().IsInterface)
            {
                Error("must_not_be_interface");
            }
            CheckAttributes();
            SchemaClasses = CheckChildren();
        }

        /// <summary>
        /// Gets the collection of the class that the <see cref="Schema"/>
        /// object of the validated class contains.
        /// </summary>
        /// <returns>
        /// The collection of the class that the <see cref="Schema"/> object of
        /// the validated class contains if the class is valid.
        /// </returns>
        public IEnumerable<Type> SchemaClasses { get; }

        /// <summary>
        /// Gets the class annotated with <see cref="ForElementAttribute"/>.
        /// </summary>
        private Type Class { get; }

        /// <summary>
        /// Returns the new set of types that the specified class depends on.
        /// </summary>
        /// <param name="clazz">
        /// The type of the class.
        /// </param>
        /// <returns>
        /// The new set of types that <paramref name="clazz"/> depends on.
        /// </returns>
        public static ISet<Type> GetDependencies(Type clazz)
        {
            var all = Classes.GetStaticFields<ElementSchemaAttribute>(clazz);
            return !all.Any()
                ? new HashSet<Type>()
                : new HashSet<Type>(GetSchema(all.First()).Types()
                    .Where(t => t.IsMandatory)
                    .Select(t => t.ElementType));
        }

        /// <summary>
        /// Returns whether the specified class is marked with the annotation
        /// <see cref="ForElementAttribute"/>.
        /// </summary>
        /// <param name="clazz">
        /// The class to test.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified class is marked with the annotation
        /// <see cref="ForElementAttribute"/>.
        /// </returns>
        private static bool IsElementClass(Type clazz)
            => clazz.GetTypeInfo()
                .GetCustomAttributes<ForElementAttribute>()
                .Any();

        /// <summary>
        /// If the specified key is not already associated with a value (or is
        /// mapped to <c>null</c>), attempts to compute its value using the
        /// given mapping function and enters it into the specified map unless
        /// <c>null</c>.
        /// </summary>
        /// <typeparam name="K">
        ///  The type of key.
        /// </typeparam>
        /// <typeparam name="V">
        /// The type of value.
        /// </typeparam>
        /// <param name="d">
        /// The map.
        /// </param>
        /// <param name="key">
        /// The key with which the specified value is to be associated.
        /// </param>
        /// <param name="toValue">
        /// The function to compute a value.
        /// </param>
        /// <returns>
        /// The current (existing or computed) value associated with the
        /// specified key, or <c>null</c> if the computed value is <c>null</c>.
        /// </returns>
        private static V ComputeIfAbsent<K, V>(
            Dictionary<K, V> d, K key, Func<K, V> toValue)
        {
            if (d.TryGetValue(key, out var value))
            {
                return value;
            }
            var newValue = toValue(key);
            d[key] = newValue;
            return newValue;
        }

        /// <summary>
        /// Add the key and value that each of the specified elements provides
        /// to the specified multi-value map.
        /// </summary>
        /// <typeparam name="T">
        /// The type of each of <paramref name="all"/> that can provide the key
        /// and value.
        /// </typeparam>
        /// <typeparam name="K">
        /// The type of key.
        /// </typeparam>
        /// <typeparam name="V">
        /// The type of value.
        /// </typeparam>
        /// <param name="map">
        /// The map of the key to the associated <see cref="List{T}"/>
        /// containing the value.
        /// </param>
        /// <param name="all">
        /// The collection containing the elements. Each element can provides
        /// the key and value with the functions <paramref name="getKey"/> and
        /// <paramref name="getValue"/>.
        /// </param>
        /// <param name="getKey">
        /// The function that returns the key of the parameter.
        /// </param>
        /// <param name="getValue">
        /// The function that returns the value of the parameter.
        /// </param>
        private static void Add<T, K, V>(
            Dictionary<K, List<V>> map,
            IEnumerable<T> all,
            Func<T, K> getKey,
            Func<T, V> getValue)
        {
            foreach (var m in all)
            {
                var key = getKey(m);
                ComputeIfAbsent(map, key, k => new List<V>())
                    .Add(getValue(m));
            }
        }

        /// <summary>
        /// Returns the schema object.
        /// </summary>
        /// <param name="field">
        /// The field annotated with [ElementSchema].
        /// </param>
        /// <returns>
        /// The schema object.
        /// </returns>
        private static Schema GetSchema(FieldInfo field)
            => field.GetValue(null) as Schema ?? Schema.Empty;

        /// <summary>
        /// Returns all the placeholder classes that the schema object
        /// contains.
        /// </summary>
        /// <param name="all">
        /// The list of the field annotated with
        /// <see cref="ElementSchemaAttribute"/>.
        /// The size of the list must be either 0 or 1.
        /// </param>
        /// <returns>
        /// All the placeholder classes that the schema object contains.
        /// </returns>
        private static IEnumerable<SchemaType> GetSchemaTypes(
            IEnumerable<FieldInfo> all)
        {
            return !all.Any()
                ? Enumerable.Empty<SchemaType>()
                : GetSchema(all.First()).Types();
        }

        private static bool RetutnsVoidAndHasSingleParameter(
                MethodInfo m, Func<Type, bool> isValidType)
        {
            var parameters = m.GetParameters();
            return m.ReturnType.Equals(Types.Void)
                && parameters.Length == 1
                && isValidType(parameters[0].ParameterType);
        }

        /// <summary>
        /// Returns the static fields of the <c>elementClass</c>
        /// marked with the specified annotation.
        /// </summary>
        /// <typeparam name="T">
        /// The annotation class.
        /// </typeparam>
        /// <returns>
        /// The static fields marked with the specified annotation.
        /// </returns>
        private IEnumerable<FieldInfo> GetStaticFields<T>()
            where T : Attribute
            => Classes.GetStaticFields<T>(Class);

        /// <summary>
        /// Returns the instance fields of the <c>elementClass</c>
        /// marked with the specified annotation.
        /// </summary>
        /// <typeparam name="T">
        /// The annotation class.
        /// </typeparam>
        /// <returns>
        /// The instance fields marked with the specified annotation.
        /// </returns>
        private IEnumerable<FieldInfo> GetInstanceFields<T>()
            where T : Attribute
            => Classes.GetInstanceFields<T>(Class);

        /// <summary>
        /// Returns the static methods of the <c>elementClass</c> marked with
        /// the specified annotation.
        /// </summary>
        /// <typeparam name="T">
        /// The annotation class.
        /// </typeparam>
        /// <returns>
        /// The static methods marked with the specified annotation.
        /// </returns>
        private IEnumerable<MethodInfo> GetStaticMethods<T>()
            where T : Attribute
            => Classes.GetStaticMethods<T>(Class);

        /// <summary>
        /// Returns the instance methods of the <c>elementClass</c> marked with
        /// the specified annotation.
        /// </summary>
        /// <typeparam name="T">
        /// The annotation class.
        /// </typeparam>
        /// <returns>
        /// The instance methods marked with the specified annotation.
        /// </returns>
        private IEnumerable<MethodInfo> GetInstanceMethods<T>()
            where T : Attribute
            => Classes.GetInstanceMethods<T>(Class);

        /// <summary>
        /// Validates the fields/methods marked with the annotation <see
        /// cref="ForAttributeAttribute"/>/<see
        /// cref="FromAttributeAttribute"/>.
        /// </summary>
        private void CheckAttributes()
        {
            // Checks [For] for static fields.
            Elements.IfNotEmpty(
                GetStaticFields<ForAttributeAttribute>(),
                t => Warn("ForAttribute_is_ignored", Names.OfFields(t)));

            // Checks [From] for static methods.
            Elements.IfNotEmpty(
                GetStaticMethods<FromAttributeAttribute>(),
                t => Warn("FromAttribute_is_ignored", Names.OfMethods(t)));

            // Checks the duplication with the attribute name of the
            // [For] and [From].
            var fields = GetInstanceFields<ForAttributeAttribute>();
            var fromMethods = GetInstanceMethods<FromAttributeAttribute>();

            static XmlQualifiedName GetForAttributeValue(FieldInfo f)
                => f.GetCustomAttribute<ForAttributeAttribute>().QName;
            static XmlQualifiedName GetFromAttributeValue(MethodInfo m)
                => m.GetCustomAttribute<FromAttributeAttribute>().QName;

            var map = new Dictionary<XmlQualifiedName, List<string>>();
            Add(map, fields, GetForAttributeValue, f => f.Name);
            Add(map, fromMethods, GetFromAttributeValue, Names.GetMethodName);
            foreach (var pair in map)
            {
                var list = pair.Value;
                if (list.Count == 1)
                {
                    continue;
                }
                var names = Names.SortAndJoin(list);
                Error("duplicated_attribute_name", pair.Key, names);
            }

            // Checks that the type of the field annotated with [ForAttribute]
            // is not string.
            static bool IsValidType(Type t)
                => StringSugarcoaters.IsValid(t);

            static bool IsInvalidForAttributeField(FieldInfo f)
                => !IsValidType(f.FieldType);

            Elements.IfNotEmpty(
                fields.Where(IsInvalidForAttributeField),
                t => Error("type_mismatch_ForAttribute", Names.OfFields(t)));

            // Checks that the signature of the method annotated with
            // [FromAttribute] is not "void(string)".
            bool IsInvalidFromAttributeMethod(MethodInfo m)
                => !RetutnsVoidAndHasSingleParameter(m, IsValidType);

            Elements.IfNotEmpty(
                fromMethods.Where(IsInvalidFromAttributeMethod),
                t => Error("type_mismatch_FromAttribute", Names.OfMethods(t)));
        }

        /// <summary>
        /// Validates the components of the <c>elementClass</c>.
        /// </summary>
        private IEnumerable<Type> CheckChildren()
        {
            // Checks [ElementSchema] for instance fields.
            Elements.IfNotEmpty(
                GetInstanceFields<ElementSchemaAttribute>(),
                t => Warn("ElementSchema_is_ignored", Names.OfFields(t)));

            // Checks [ForText] for static fields.
            Elements.IfNotEmpty(
                GetStaticFields<ForTextAttribute>(),
                t => Warn("ForText_is_ignored", Names.OfFields(t)));

            // Checks [FromText] for static methods.
            Elements.IfNotEmpty(
                GetStaticMethods<FromTextAttribute>(),
                t => Warn("FromText_is_ignored", Names.OfMethods(t)));

            // Checks [ForChild] for static fields.
            Elements.IfNotEmpty(
                GetStaticFields<ForChildAttribute>(),
                t => Warn("ForChild_is_ignored", Names.OfFields(t)));

            // Checks [FromChild] for static methods.
            Elements.IfNotEmpty(
                GetStaticMethods<FromChildAttribute>(),
                t => Warn("FromChild_is_ignored", Names.OfMethods(t)));

            var elementSchemas = GetStaticFields<ElementSchemaAttribute>();
            var forTexts = GetInstanceFields<ForTextAttribute>();
            var fromTexts = GetInstanceMethods<FromTextAttribute>();

            // Checks [ElementSchema] is combined with [ForText], [FromText].
            if (elementSchemas.Any() && forTexts.Any())
            {
                Error(
                    "both_ForText_and_ElementSchema",
                    Names.OfFields(forTexts),
                    Names.OfFields(elementSchemas));
            }
            if (elementSchemas.Any() && fromTexts.Any())
            {
                Error(
                    "both_FromText_and_ElementSchema",
                    Names.OfMethods(fromTexts),
                    Names.OfFields(elementSchemas));
            }
            if (forTexts.Any() && fromTexts.Any())
            {
                Error(
                    "both_ForText_and_FromText",
                    Names.OfFields(forTexts),
                    Names.OfMethods(fromTexts));
            }
            CheckForText(forTexts);
            CheckFromText(fromTexts);
            return CheckForElementSchema(elementSchemas);
        }

        /// <summary>
        /// Validates the <see cref="Schema"/> object marked with the
        /// annotation <see cref="ElementSchemaAttribute"/>.
        /// </summary>
        /// <param name="fields">
        /// The static fields marked with the annotation
        /// <see cref="ElementSchemaAttribute"/>.
        /// </param>
        private IEnumerable<Type> CheckForElementSchema(
            IEnumerable<FieldInfo> fields)
        {
            // Checks two or more [ElementSchema]s.
            if (fields.Count() > 1)
            {
                Error("duplicated_ElementSchema", Names.OfFields(fields));
            }

            var schemaTypes = GetSchemaTypes(fields);
            var placeholders = schemaTypes.Select(t => t.PlaceholderType);
            var schemaClasses = schemaTypes.Select(t => t.ElementType);
            var forChildren = GetInstanceFields<ForChildAttribute>();
            var fromChildren = GetInstanceMethods<FromChildAttribute>();

            // Checks no [ElementSchema]s and any [ForChild]/[FromChild],
            // which probably means missing [ElementSchema].
            if (!fields.Any()
                && (forChildren.Any() || fromChildren.Any()))
            {
                var all = new[]
                {
                    Names.OfFields(forChildren),
                    Names.OfMethods(fromChildren),
                }.Where(s => !(s is ""));

                Warn("missing_ElementSchema", string.Join(", ", all));
            }

            static bool IsType<T>(FieldInfo f)
                => typeof(T).GetTypeInfo()
                    .IsAssignableFrom(f.FieldType.GetTypeInfo());

            static bool IsValidFromChildMethod(MethodInfo m)
                => m.ReturnType.Equals(Types.Void)
                    && m.GetParameters().Length == 1;

            static Type GetFirstParameterType(MethodInfo m)
                => m.GetParameters()[0].ParameterType;

            // Checks the type of the field annotated with [ElementSchema] is
            // not Schema class.
            Elements.IfNotEmpty(
                fields.Where(s => !IsType<Schema>(s)),
                t => Error("type_mismatch_ElementSchema", Names.OfFields(t)));

            // Checks each type that the Schema object contains is not
            // the class annotated with [ForElement].
            Elements.IfNotEmpty(
                schemaClasses.Where(t => !IsElementClass(t)),
                t => Error(
                    "not_annotated_with_ForElement", Names.OfClasses(t)));

            // Checks the duplication with element names.
            static XmlQualifiedName ToElementName(Type type)
                => type.GetTypeInfo()
                    .GetCustomAttribute<ForElementAttribute>()
                    .QName;
            var nameMap = new Dictionary<XmlQualifiedName, List<string>>();
            var validSchemaClasses
                = schemaClasses.Where(IsElementClass);
            Add(nameMap, validSchemaClasses, ToElementName, t => t.Name);
            foreach (var p in nameMap)
            {
                var key = p.Key;
                var list = p.Value;
                if (list.Count == 1)
                {
                    continue;
                }
                list.Sort();
                Error(
                    "duplicated_element_name",
                    key,
                    Names.SortAndJoin(list));
            }

            // Checks the method annotated with [FromChild] is invalid.
            Elements.IfNotEmpty(
                fromChildren.Where(m => !IsValidFromChildMethod(m)),
                t => Error("type_mismatch_FromChild", Names.OfMethods(t)));

            // Checks the duplication with the child elements.
            static Type FieldType(FieldInfo f)
                => Types.PlaceholderType(f.FieldType);
            static Type MethodType(MethodInfo m)
                => Types.PlaceholderType(GetFirstParameterType(m));

            var map = new Dictionary<Type, List<string>>();
            Add(map, forChildren, FieldType, f => f.Name);
            Add(map, fromChildren, MethodType, Names.GetMethodName);
            foreach (var p in map)
            {
                var key = p.Key;
                var list = p.Value;
                if (list.Count == 1)
                {
                    continue;
                }
                list.Sort();
                Error(
                    "duplicated_child_class",
                    key.Name,
                    Names.SortAndJoin(list));
            }

            // Checks the classes in the Schema are not handled.
            var handledClasses = map.Keys;
            Elements.IfNotEmpty(
                Elements.DifferenceOf(placeholders, handledClasses),
                p => Error("not_handled_class", Names.OfClasses(p)));

            // Checks the field/method(s) that are never used.
            if (fields.Any())
            {
                foreach (var c
                    in Elements.DifferenceOf(handledClasses, placeholders))
                {
                    Warn(
                        "ForChild_FromChild_is_unused",
                        Names.Of(c),
                        Names.SortAndJoin(map[c]));
                }
            }

            return schemaClasses;
        }

        /// <summary>
        /// Validates the fields marked with the annotation
        /// <see cref="ForTextAttribute"/>s.
        /// </summary>
        /// <param name="fields">
        /// The instance fields marked with the annotation
        /// <see cref="ForTextAttribute"/>.
        /// </param>
        private void CheckForText(IEnumerable<FieldInfo> fields)
        {
            // Checks two or more [ForText]s.
            if (fields.Count() > 1)
            {
                Error("duplicated_ForText", Names.OfFields(fields));
            }

            // Checks that the type of the field annotated with [ForText] is
            // not string.
            Elements.IfNotEmpty(
                fields.Where(f => !StringSugarcoaters.IsValid(f.FieldType)),
                t => Error("type_mismatch_ForText", Names.OfFields(t)));
        }

        /// <summary>
        /// Validates the methods marked with the annotation
        /// <see cref="FromTextAttribute"/>s.
        /// </summary>
        /// <param name="methods">
        /// The instance methods marked with the annotation
        /// <see cref="FromTextAttribute"/>.
        /// </param>
        private void CheckFromText(IEnumerable<MethodInfo> methods)
        {
            // Checks two or more [FromText]s.
            if (methods.Count() > 1)
            {
                Error("duplicated_FromText", Names.OfMethods(methods));
            }

            // Checks that the method annotated with [FromText] does not
            // return void and does not take a single parameter whose type
            // is string.
            static bool IsInvalid(MethodInfo m)
                => !RetutnsVoidAndHasSingleParameter(
                    m, StringSugarcoaters.IsValid);
            Elements.IfNotEmpty(
                methods.Where(IsInvalid),
                t => Error("type_mismatch_FromText", Names.OfMethods(t)));
        }
    }
}
