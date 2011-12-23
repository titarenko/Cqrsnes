using System;
using System.Collections.Generic;

namespace Cqrsnes.Infrastructure
{
    /// <summary>
    /// Base class for any entity (building block) of the domain.
    /// </summary>
    public abstract class AggregateRoot
    {
        /// <summary>
        /// Unique identifier of aggregate root.
        /// </summary>
        protected Guid id;

        private readonly List<Event> uncommittedChanges = new List<Event>();

        /// <summary>
        /// Constructs new instance (leaves identifier empty).
        /// </summary>
        protected AggregateRoot()
        {
        }

        /// <summary>
        /// Constructs new instance.
        /// </summary>
        /// <param name="id">Unique identifier.</param>
        protected AggregateRoot(Guid id)
        {
            if (id == default(Guid))
            {
                throw new InvalidOperationException("Can't assign null id.");
            }

            this.id = id;
        }

        /// <summary>
        /// Unique identifier of aggregate root.
        /// </summary>
        /// <remarks>
        /// Important guideline is "do not expose the state of aggregate root".
        /// This property is an exception of above rule. You should keep 
        /// state encapsulated, only behavior should be exposed.
        /// </remarks>
        public Guid Id
        {
            get
            {
                return id;
            }
        }

        /// <summary>
        /// Returns sequence of uncommitted changes.
        /// </summary>
        /// <returns>Sequence of uncommitted changes.</returns>
        public IEnumerable<Event> GetUncommittedChanges()
        {
            return uncommittedChanges;
        }

        /// <summary>
        /// Marks all uncommitted changes as committed.
        /// </summary>
        public void MarkChangesAsCommitted()
        {
            uncommittedChanges.Clear();
        }

        /// <summary>
        /// Loads aggregate root from history (doesn't affect 
        /// uncommitted changes collection).
        /// </summary>
        /// <param name="events">History itself as sequence of events.</param>
        public void LoadFromHistory(IEnumerable<Event> events)
        {
            foreach (var @event in events)
            {
                ApplyChange(@event, true);
            }
        }

        /// <summary>
        /// Applies event to aggregate root (adds this event to 
        /// sequence of uncommitted changes in case if it is not from history).
        /// </summary>
        /// <param name="event">Event describing the change.</param>
        /// <param name="isFromHistory">Is event from history?</param>
        protected void ApplyChange(Event @event, bool isFromHistory = false)
        {
            var type = GetType();
            var eventType = @event.GetType();

            var handlerType = typeof (IChangeAcceptor<>)
                .MakeGenericType(eventType);

            if (handlerType.IsAssignableFrom(type))
            {
                var method = type.GetMethod("Accept", new[] {eventType});
                if (method == null)
                {
                    throw new ApplicationException(
                        "Can't find Accept method of IChangeAcceptor. Make sure it was not renamed.");
                }
                method.Invoke(this, new object[] {@event});
            }

            if (!isFromHistory)
            {
                uncommittedChanges.Add(@event);
            }
        }
    }
}