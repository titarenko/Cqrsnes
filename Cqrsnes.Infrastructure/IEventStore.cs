using System;
using System.Collections.Generic;

namespace Cqrsnes.Infrastructure
{
    /// <summary>
    /// Specialized repository for storing of events.
    /// </summary>
    public interface IEventStore
    {
        /// <summary>
        /// Stores sequence of events for given aggregate root.
        /// </summary>
        /// <param name="aggregateId">Aggregate root identifier.</param>
        /// <param name="events">Sequence of events.</param>
        void SaveEvents(Guid aggregateId, IEnumerable<Event> events);

        /// <summary>
        /// Retrieves all events for given aggregate root.
        /// </summary>
        /// <param name="id">Identifier of aggregate root.</param>
        /// <returns>Sequence of events.</returns>
        IEnumerable<Event> GetEventsForAggregate(Guid id);
    }
}