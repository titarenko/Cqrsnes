using System;
using System.Collections;
using System.Collections.Generic;

namespace Cqrsnes.Infrastructure
{
    /// <summary>
    /// Defines interface for dependency resolver.
    /// </summary>
    public interface IDependencyResolver
    {
        /// <summary>
        /// Returns all instances of requested type.
        /// </summary>
        /// <param name="type">Type of instances to return.</param>
        /// <returns>Collection of instances of given type.</returns>
        IEnumerable ResolveMultiple(Type type);

        /// <summary>
        /// Returns all instances of requested type.
        /// </summary>
        /// <typeparam name="T">Type of instances to return.</typeparam>
        /// <returns>Collection of instances of given type.</returns>
        IEnumerable<T> ResolveMultiple<T>();

        /// <summary>
        /// Returns instance of requested type.
        /// </summary>
        /// <param name="type">Type of instance to return.</param>
        /// <returns>Instance of requested type.</returns>
        object Resolve(Type type);

        /// <summary>
        /// Returns instance of requested type.
        /// </summary>
        /// <typeparam name="T">Type of instance to return.</typeparam>
        /// <returns>Instance of requested type.</returns>
        T Resolve<T>();
    }
}