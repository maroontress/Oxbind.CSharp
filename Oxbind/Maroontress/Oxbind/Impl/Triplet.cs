namespace Maroontress.Oxbind.Impl
{
    using System;

    /// <summary>
    /// Capsulize the placeholder type, the unit type, and the sugarcoater.
    /// </summary>
    public struct Triplet
    {
        private static readonly Sugarcoater<object> Eventizer
            = Readers.NewEventObject;

        private static readonly Sugarcoater<object> PassThrough
            = (r, v) => v;

        private Triplet(
            Type placeholderType,
            Type unitType,
            Sugarcoater<object> sugarcoater)
        {
            PlaceHolderType = placeholderType;
            UnitType = unitType;
            Sugarcoater = sugarcoater;
        }

        /// <summary>
        /// Gets the placeholder type.
        /// </summary>
        public Type PlaceHolderType { get; }

        /// <summary>
        /// Gets the unit type.
        /// </summary>
        public Type UnitType { get; }

        /// <summary>
        /// Gets the sugarcoater.
        /// </summary>
        public Sugarcoater<object> Sugarcoater { get; }

        /// <summary>
        /// Gets a new triplet associated with the specified type.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The new triplet.
        /// </returns>
        public static Triplet Of(Type type)
        {
            Triplet Dispatch(Type u, Func<Type, Type> p)
                => !Types.IsRawType(u, Types.BindEventT)
                    ? new Triplet(type, u, PassThrough)
                    : new Triplet(p(Types.FirstInnerType(u)), u, Eventizer);

            Type ToGenericType(Type u)
                => Types.IEnumerableT.MakeGenericType(u);

            return !Types.IsRawType(type, Types.IEnumerableT)
                ? Dispatch(type, u => u)
                : Dispatch(Types.FirstInnerType(type), ToGenericType);
        }
    }
}
/*
    | Type of value               | PlaceholderType  | UnitType       | Sugarcoater  |
    | :---                        | :---             | :---           | :---         |
    | `T`                         | `T`              | `T`            | Pass through |
    | `BindEvent<T>`              | `T`              | `BindEevnt<T>` | Eventizer    |
    | `IEnumerable<T>`            | `IEnumerable<T>` | `T`            | Pass through |
    | `IEnumerable<BindEvent<T>>` | `IEnumerable<T>` | `BindEvent<T>` | Eventizer    |
*/
