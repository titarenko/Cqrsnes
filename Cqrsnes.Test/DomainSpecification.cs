using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cqrsnes.Infrastructure;
using Cqrsnes.Infrastructure.Impl;

namespace Cqrsnes.Test
{
    /// <summary>
    /// Domain behavior specification (domain test).
    /// </summary>
    /// <typeparam name="TCommand">Command type.</typeparam>
    /// <typeparam name="THandlerType">Command handler type.</typeparam>
    public class DomainSpecification<TCommand, THandlerType> 
        where TCommand : Command
        where THandlerType : ICommandHandler<TCommand>
    {
        /// <summary>
        /// Specification name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Sequence of preceding events.
        /// </summary>
        public IEnumerable<Event> Given { get; set; }

        /// <summary>
        /// Command (reaction to which must be verified).
        /// </summary>
        public TCommand When { get; set; }

        /// <summary>
        /// Expected events (as a reaction to given command).
        /// </summary>
        public IEnumerable<Event> Expect { get; set; }

        /// <summary>
        /// Is exception expected as reaction to command?
        /// </summary>
        public bool IsExceptionExpected { get; set; }

        /// <summary>
        /// Constructs new instance.
        /// </summary>
        public DomainSpecification()
        {
            Name = "Unnamed Specification";
            Given = new Event[0];
            Expect = new Event[0];
            IsExceptionExpected = false;
        }

        /// <summary>
        /// Runs specification.
        /// </summary>
        /// <returns></returns>
        public ExecutionResult Run()
        {
            var result = new ExecutionResult {IsPassed = true};
            var s = new StringBuilder();

            PrintSpecification(s);

            try
            {
                var store = new TestEventStore(Given);
                var handler = (ICommandHandler<TCommand>) Activator.CreateInstance(
                    typeof (THandlerType), new object[] {new CommonAggregateRootRepository(store, new TestBus())});

                try
                {
                    handler.Handle(When);
                    result.IsPassed = result.IsPassed && !IsExceptionExpected;
                    if (IsExceptionExpected)
                    {
                        s.AppendLine("Failure: exception was not thrown.");
                    }
                }
                catch (Exception e)
                {
                    result.IsPassed = result.IsPassed && IsExceptionExpected;
                    s.AppendFormat(
                        !IsExceptionExpected
                            ? "Failure: unexpected exception (\"{0}\").\n"
                            : "Success: got expected exception (\"{0}\").\n", e.Message);
                }

                var produced = store.GetProducedEvents();
                var correct = Utilities.SequenceEqual(Expect, produced);
                result.IsPassed = result.IsPassed && correct;
                s.AppendLine(!correct
                                 ? "Failure: produced events didn't match expected."
                                 : "Success: produced events matched expected.");
            }
            catch (Exception e)
            {
                s.AppendFormat("Failure: really unexpected exception (\"{0}\").\n", e.Message);
                result.IsPassed = false;
            }

            s.AppendFormat("Done ({0}).\n", result.IsPassed ? "passed" : "failed");
            result.Details = s.ToString();
            return result;
        }

        private void PrintSpecification(StringBuilder s)
        {
            s.AppendFormat("Specification: \"{0}\"\n", Name);
            s.AppendLine();

            if (Given.Count() > 0)
            {
                s.AppendLine("Given:");
            }
            foreach (var @event in Given)
            {
                s.AppendFormat("\t{0}\n", Utilities.Describe(@event));
            }
            if (Given.Count() > 0)
            {
                s.AppendLine();
            }

            s.AppendLine("When:");
            s.AppendFormat("\t{0}\n", Utilities.Describe(When));
            s.AppendLine();

            s.AppendLine("Expect:");
            foreach (var @event in Expect)
            {
                s.AppendFormat("\t{0}\n", Utilities.Describe(@event));
            }
            if (IsExceptionExpected)
            {
                s.AppendLine("\texception is thrown");
            }
            s.AppendLine();
        }
    }
}