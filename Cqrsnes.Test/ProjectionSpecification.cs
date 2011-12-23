using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Cqrsnes.Infrastructure;

namespace Cqrsnes.Test
{
    /// <summary>
    /// Projection specification (projector test).
    /// </summary>
    /// <typeparam name="TProjector">Projector type.</typeparam>
    public class ProjectionSpecification<TProjector>
    {
        private IEnumerable<Event> given;

        private Event when;

        private readonly IList<Expression<Func<TProjector, bool>>> expect;

        private bool isExceptionExpected;

        private string unwantedPostfix;

        /// <summary>
        /// Constructs new instance.
        /// </summary>
        public ProjectionSpecification()
        {
            Name = "Projection logic of " + Utilities.Prettify(
                typeof(TProjector).Name) + " (SUT)";
            given = new Event[0];
            expect = new List<Expression<Func<TProjector, bool>>>();
        }

        /// <summary>
        /// Specification name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Accepts sequence of preceding events.
        /// </summary>
        /// <param name="events">Sequence of preceding events.</param>
        /// <returns>Reference to specification instance to enable chaining.</returns>
        public ProjectionSpecification<TProjector> Given(IEnumerable<Event> events)
        {
            given = events;
            return this;
        }

        /// <summary>
        /// Accepts incoming event (reaction to which should be tested).
        /// </summary>
        /// <param name="event">Incoming event.</param>
        /// <returns>Reference to specification instance to enable chaining.</returns>
        public ProjectionSpecification<TProjector> When(Event @event)
        {
            when = @event;
            return this;
        }

        /// <summary>
        /// Accepts statement that expresses expectation (tests result of projection).
        /// </summary>
        /// <param name="expression">Statement that expresses expectation (tests result of projection).</param>
        /// <returns>Reference to specification instance to enable chaining.</returns>
        public ProjectionSpecification<TProjector> Expect(Expression<Func<TProjector, bool>> expression)
        {
            expect.Add(expression);
            return this;
        }

        public ProjectionSpecification<TProjector> UnwantedPostfix<TProperty>(
            Expression<Func<TProjector, TProperty>> expression)
        {
            unwantedPostfix = GetActualValueDescription(expression.Body);
            return this;
        }

        /// <summary>
        /// Accepts expectation of exception as a reaction to incoming event.
        /// </summary>
        /// <returns>Reference to specification instance to enable chaining.</returns>
        public ProjectionSpecification<TProjector> ExpectException()
        {
            isExceptionExpected = true;
            return this;
        }

        /// <summary>
        /// Runs specification.
        /// </summary>
        /// <returns>Execution result.</returns>
        public ExecutionResult Run()
        {
            var result = new ExecutionResult {IsPassed = true};

            var eventHandlerType = typeof (IEventHandler<>);
            var projectorType = typeof (TProjector);

            var projector = (TProjector) Activator.CreateInstance(projectorType, new TestRepository());

            foreach (var @event in given)
            {
                var eventType = @event.GetType();

                var type = eventHandlerType.MakeGenericType(eventType);
                if (!type.IsAssignableFrom(projectorType))
                {
                    throw new InvalidOperationException(
                        "Projector can't handle at least one of the given events.");
                }

                projectorType.GetMethod("Handle", new[] {eventType})
                    .Invoke(projector, new[] {@event});
            }

            var exceptionMessage = string.Empty;
            try
            {
                projectorType.GetMethod("Handle",
                                        new[] {when.GetType()}).Invoke(projector, new[] {when});
            }
            catch (Exception exception)
            {
                exceptionMessage = exception is TargetInvocationException
                                       ? exception.InnerException.Message
                                       : exception.Message;
            }

            PrintSpecification(
                expect.Select(x => ProcessExpectation(projector, x)),
                result, exceptionMessage);

            return result;
        }

        private void PrintSpecification(
            IEnumerable<ExecutionResult> results, 
            ExecutionResult result, 
            string exceptionMessage)
        {
            var s = new StringBuilder();

            s.AppendFormat("Specification: \"{0}\"\n", Name);
            s.AppendLine();

            var hasGiven = given.Count() > 0;
            if (hasGiven)
            {
                s.AppendLine("Given:");
            }
            foreach (var @event in given)
            {
                s.AppendFormat("\t{0}\n", Utilities.Describe(@event));
            }
            if (hasGiven)
            {
                s.AppendLine();
            }

            s.AppendLine("When:");
            s.AppendFormat("\t{0}\n\n", Utilities.Describe(when));

            s.AppendLine("Expect:");
            foreach (var expectationResult in results)
            {
                s.AppendFormat("\t{0}\n", expectationResult);
            }
            var gotException = !string.IsNullOrEmpty(exceptionMessage);
            s.AppendFormat(
                "\t{0}: {1}\n",
                isExceptionExpected ? "exception" : "no exception",
                isExceptionExpected
                    ? (gotException ? string.Format("passed (\"{0}\")", exceptionMessage) : "failed")
                    : (gotException ? string.Format("failed (\"{0}\")", exceptionMessage) : "passed"));
            s.AppendLine();

            result.IsPassed = results.All(x => x.IsPassed) &&
                              (isExceptionExpected && gotException ||
                               !isExceptionExpected && !gotException);

            s.AppendFormat("Done ({0}).", result.IsPassed ? "passed" : "failed");

            result.Details = s.ToString();
        }

        private ExecutionResult ProcessExpectation<T>(T instance, Expression<Func<T, bool>> expression)
        {
            var comparison = expression.Body as BinaryExpression;

            if (comparison == null)
            {
                throw new InvalidOperationException(
                    "Provided expression is not a binary one.");
            }

            if (comparison.NodeType != ExpressionType.Equal &&
                comparison.NodeType != ExpressionType.NotEqual)
            {
                throw new InvalidOperationException(
                    "Only strict equality comparison expressions are supported.");
            }

            var actualValueDescription = GetActualValueDescription(comparison.Left);
            var expectedValue = GetExpectedValue(comparison.Right);
            var result = expression.Compile()(instance);

            if (!string.IsNullOrEmpty(unwantedPostfix))
            {
                actualValueDescription = actualValueDescription.Replace(
                    unwantedPostfix, "SUT");
            }

            return new ExecutionResult
                       {
                           IsPassed = result,
                           Details = string.Format(
                               "{0} {1} {2}: {3}",
                               actualValueDescription,
                               comparison.NodeType == ExpressionType.Equal
                                   ? "must be equal to"
                                   : "must not be equal to",
                               expectedValue.GetType() == typeof (Guid)
                                   ? Utilities.Prettify((Guid) expectedValue)
                                   : expectedValue.ToString(),
                               result
                                   ? "passed"
                                   : "failed")
                       };
        }

        private object GetExpectedValue(Expression expected)
        {
            object expectedValue;
            if (expected is ConstantExpression)
            {
                expectedValue = (expected as ConstantExpression).Value;
            }
            else if (expected is MemberExpression)
            {
                var objectMember = Expression.Convert(expected, typeof(object));
                var getterLambda = Expression.Lambda<Func<object>>(objectMember);
                var getter = getterLambda.Compile();
                expectedValue = getter();
            }
            else
            {
                throw new InvalidOperationException(
                    "Expected value must be a right hand operand of comparison.");
            }
            return expectedValue;
        }

        private string GetActualValueDescription(Expression expression)
        {
            if (expression is MemberExpression)
            {
                var memberExpression = expression as MemberExpression;
                return Utilities.Prettify(memberExpression.Member.Name) +
                       (memberExpression.Expression == null
                           ? ""
                           : " of " + GetActualValueDescription(memberExpression.Expression));
            }

            if (expression is MethodCallExpression)
            {
                var methodCallExpression = expression as MethodCallExpression;
                return Utilities.Prettify(methodCallExpression.Method.Name) +
                       (methodCallExpression.Object == null
                            ? (methodCallExpression.Method.IsDefined(typeof (ExtensionAttribute), true)
                                   ? " of " + GetActualValueDescription(methodCallExpression.Arguments[0])
                                   : "")
                            : " of " + GetActualValueDescription(methodCallExpression.Object));
            }

            if (expression is ParameterExpression)
            {
                var parameterExpression = expression as ParameterExpression;
                return parameterExpression.Name.Length < 3 ? "SUT" : Utilities.Prettify(parameterExpression.Name);
            }

            throw new InvalidOperationException("Unexpected expression.");
        }
    }
}