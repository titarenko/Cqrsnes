namespace Cqrsnes.Test
{
    /// <summary>
    /// Specification execution result.
    /// </summary>
    public class ExecutionResult
    {
        public ExecutionResult()
        {
            IsPassed = false;
            Details = "Was not executed yet.";
        }

        /// <summary>
        /// Is specification (test) passed?
        /// </summary>
        public bool IsPassed { get; set; }

        /// <summary>
        /// Details as human-readable text.
        /// </summary>
        public string Details { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that 
        /// represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents 
        /// the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return Details;
        }
    }
}