using System;
using System.Collections.Generic;
using System.Linq;
using Cqrsnes.Infrastructure;

namespace Cqrsnes.Test
{
    /// <summary>
    /// Repository for testing purposes (leaves entities transient).
    /// </summary>
    public class TestRepository : IRepository
    {
        private readonly IDictionary<Guid, object> objects = new Dictionary<Guid, object>();

        /// <summary>
        /// Saves instance.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <typeparam name="T">Type of instance.</typeparam>
        public void Save<T>(T instance)
        {
            var id = (Guid) typeof (T).GetProperty("Id").GetValue(instance, null);
            if (objects.ContainsKey(id))
            {
                objects[id] = instance;
            }
            else
            {
                objects.Add(id, instance);
            }
        }

        /// <summary>
        /// Retrieves instance by its identifier.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <typeparam name="T">Instance type.</typeparam>
        /// <returns>Instance.</returns>
        public T GetById<T>(Guid id)
        {
            if (objects.ContainsKey(id))
            {
                return (T) objects[id];
            }
            return default(T);
        }

        /// <summary>
        /// Retrieves collection of instances.
        /// </summary>
        /// <typeparam name="T">Instance type.</typeparam>
        /// <returns>Collection of instances.</returns>
        public IEnumerable<T> GetAll<T>()
        {
            return objects.Values.Cast<T>();
        }
    }
}