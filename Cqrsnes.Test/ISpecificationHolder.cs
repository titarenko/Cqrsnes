using System.Collections.Generic;

namespace Cqrsnes.Test
{
    /// <summary>
    /// Defines interface for entity containing set of specifications.
    /// </summary>
    public interface ISpecificationHolder
    {
        /// <summary>
        /// Executes all specifications.
        /// </summary>
        /// <returns>Results of execution.</returns>
        IEnumerable<ExecutionResult> ExecuteAll();
    }
}