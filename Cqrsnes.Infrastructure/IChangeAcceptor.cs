namespace Cqrsnes.Infrastructure
{
    /// <summary>
    /// Should be implemented by any aggregate root changing its state as a
    /// reaction to event of certain type.
    /// </summary>
    /// <typeparam name="T">Event type.</typeparam>
    /// <remarks>
    /// While doing refactoring be sure to keep ApplyChanges method of
    /// <see cref="AggregateRoot"/> up to date.
    /// </remarks>
    public interface IChangeAcceptor<in T> where T : Event
    {
        /// <summary>
        /// Performs changes caused by event.
        /// </summary>
        /// <param name="event"></param>
        void Accept(T @event);
    }
}