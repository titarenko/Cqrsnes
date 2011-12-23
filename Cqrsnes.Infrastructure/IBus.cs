namespace Cqrsnes.Infrastructure
{
    /// <summary>
    /// Defines methods for organizing of communication between parts of the system.
    /// </summary>
    public interface IBus
    {
        /// <summary>
        /// Publishes event to multiple or none subscribers (handlers).
        /// </summary>
        /// <param name="event">Event to publish.</param>
        void Publish(Event @event);

        /// <summary>
        /// Sends command to exactly one receiver (handler).
        /// </summary>
        /// <param name="command">Command to send.</param>
        void Send(Command command);
    }
}