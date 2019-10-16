namespace Maroontress.Oxbind.Impl
{
    using System;

    /// <summary>
    /// Provides factory methods of <see cref="Reflector{T}"/>.
    /// </summary>
    public static class Reflectors
    {
        /// <summary>
        /// Gets a new instance of the <see cref="Reflector{T}"/> class.
        /// </summary>
        /// <param name="inject">
        /// The delegate that injects the sugarcoated value to the field or
        /// with the method.
        /// </param>
        /// <param name="unitType">
        /// Get the type of the unit.
        /// </param>
        /// <param name="sugarcoater">
        /// The delegate that sugarcoats the specified value.
        /// </param>
        /// <typeparam name="T">
        /// The type of the value to be injected.
        /// </typeparam>
        /// <returns>
        /// The new instance of the <see cref="Reflector{T}"/> class.
        /// </returns>
        public static Reflector<T> Of<T>(
            Injector inject, Type unitType, Sugarcoater<T> sugarcoater)
        {
            return new Reflector<T>(inject, unitType, sugarcoater);
        }
    }
}
