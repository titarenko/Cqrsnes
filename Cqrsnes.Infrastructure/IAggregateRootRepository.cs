using System;

namespace Cqrsnes.Infrastructure
{
    /// <summary>
    /// Specialized repository for loading and storing of aggregate roots.
    /// </summary>
    public interface IAggregateRootRepository
    {
        /// <summary>
        /// Saves aggregate root.
        /// </summary>
        /// <param name="instance">Aggregate root instance.</param>
        /// <typeparam name="T">Type of aggregate root.</typeparam>
        void Save<T>(T instance) where T : AggregateRoot, new();


        /// <summary>
        /// Retrieves aggregate root by its identifier.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <typeparam name="T">Type of aggregate root.</typeparam>
        /// <returns>Aggregate root instance.</returns>
        T GetById<T>(Guid id) where T : AggregateRoot, new();
    }
}