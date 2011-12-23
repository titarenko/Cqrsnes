namespace Cqrsnes.Infrastructure
{
    /// <summary>
    /// Defines interface for command receiver (handler).
    /// </summary>
    /// <typeparam name="T">Type of command.</typeparam>
    public interface ICommandHandler<in T> where T : Command
    {
        /// <summary>
        /// Handles (reacts to) command.
        /// </summary>
        /// <param name="command">Command instance.</param>
        void Handle(T command);
    }
}