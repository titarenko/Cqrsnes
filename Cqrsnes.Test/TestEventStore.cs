using System;
using System.Collections.Generic;
using Cqrsnes.Infrastructure;

namespace Cqrsnes.Test
{
    /// <summary>
    /// Event store for testing purposes (doesn't distinguish 
    /// different aggregate roots, leaves events transient).
    /// </summary>
    public class TestEventStore : IEventStore
    {
        private readonly IEnumerable<Event> events;
        private readonly List<Event> produced = new List<Event>();

        /// <summary>
        /// Constructs new instance.
        /// </summary>
        /// <param name="events">Sequence of events representing history.</param>
        public TestEventStore(IEnumerable<Event> events)
        {
            this.events = events;
        }

        /// <summary>
        /// Stores sequence of events for given aggregate root.
        /// </summary>
        /// <param name="aggregateId">Aggregate root identifier.</param>
        /// <param name="events">Sequence of events.</param>
        public void SaveEvents(Guid aggregateId, IEnumerable<Event> events)
        {
            produced.AddRange(events);
        }

        /// <summary>
        /// Retrieves all events for given aggregate root.
        /// </summary>
        /// <param name="id">Identifier of aggregate root.</param>
        /// <returns>Sequence of events.</returns>
        public IEnumerable<Event> GetEventsForAggregate(Guid id)
        {
            return events;
        }

        /// <summary>
        /// Returns events that were requested to store.
        /// </summary>
        /// <returns>Sequence of produced events.</returns>
        public IEnumerable<Event> GetProducedEvents()
        {
            return produced;
        }
    }
}