namespace Cqrsnes.Infrastructure
{
    /// <summary>
    /// Defines interface for event handler (subscriber).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEventHandler<in T> where T : Event
    {
        /// <summary>
        /// Handles (reacts to) event.
        /// </summary>
        /// <param name="event">Event instance.</param>
        void Handle(T @event);
    }
}