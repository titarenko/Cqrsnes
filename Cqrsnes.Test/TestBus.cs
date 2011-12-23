using Cqrsnes.Infrastructure;

namespace Cqrsnes.Test
{
    /// <summary>
    /// Bus for testing purposes (doesn't send messages).
    /// </summary>
    public class TestBus : IBus
    {
        /// <summary>
        /// Publishes event to multiple or none subscribers (handlers).
        /// </summary>
        /// <param name="event">Event to publish.</param>
        public void Publish(Event @event)
        {
        }

        /// <summary>
        /// Sends command to exactly one receiver (handler).
        /// </summary>
        /// <param name="command">Command to send.</param>
        public void Send(Command command)
        {
        }
    }
}