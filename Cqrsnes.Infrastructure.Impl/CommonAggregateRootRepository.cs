using System;

namespace Cqrsnes.Infrastructure.Impl
{
    /// <summary>
    /// Common implementation of aggregate root repository.
    /// </summary>
    public class CommonAggregateRootRepository : IAggregateRootRepository
    {
        private readonly IEventStore store;
        private readonly IBus bus;

        /// <summary>
        /// Constructs new instance.
        /// </summary>
        /// <param name="store">
        /// Event store implementation.
        /// </param>
        /// <param name="bus">
        /// Bus implementation.
        /// </param>
        public CommonAggregateRootRepository(IEventStore store, IBus bus)
        {
            this.store = store;
            this.bus = bus;
        }

        /// <summary>
        /// Saves aggregate root.
        /// </summary>
        /// <param name="instance">Aggregate root instance.</param>
        /// <typeparam name="T">Type of aggregate root.</typeparam>
        public void Save<T>(T instance) where T : AggregateRoot, new()
        {
            var events = instance.GetUncommittedChanges();
            store.SaveEvents(instance.Id, events);

            foreach (var @event in events)
            {
                bus.Publish(@event);
            }

            instance.MarkChangesAsCommitted();
        }

        /// <summary>
        /// Retrieves aggregate root by its identifier.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <typeparam name="T">Type of aggregate root.</typeparam>
        /// <returns>Aggregate root instance.</returns>
        public T GetById<T>(Guid id) where T : AggregateRoot, new()
        {
            var instance = new T();

            var events = store.GetEventsForAggregate(id);
            instance.LoadFromHistory(events);

            return instance;
        }
    }
}