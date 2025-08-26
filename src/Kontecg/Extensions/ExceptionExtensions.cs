using System;
using System.Runtime.ExceptionServices;

namespace Kontecg.Extensions
{
    /// <summary>
    /// Provides comprehensive extension methods for the <see cref="Exception"/> class to enhance
    /// exception handling capabilities throughout the Kontecg framework.
    /// </summary>
    /// <remarks>
    /// <para>
    /// ExceptionExtensions provides utility methods that extend the functionality of the
    /// standard <see cref="Exception"/> class. These extensions cover common exception
    /// handling tasks and provide consistent, reusable functionality across the application.
    /// </para>
    /// <para>
    /// <strong>Key Features:</strong>
    /// <list type="bullet">
    /// <item><description><strong>Stack Trace Preservation:</strong> Re-throw exceptions while maintaining original stack traces</description></item>
    /// <item><description><strong>Exception Handling:</strong> Enhanced exception handling patterns</description></item>
    /// <item><description><strong>Debugging Support:</strong> Improved debugging experience with preserved context</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Stack Trace Preservation:</strong> The primary feature of this class is the
    /// ability to re-throw exceptions while preserving their original stack traces, which
    /// is crucial for effective debugging and error diagnosis.
    /// </para>
    /// <para>
    /// <strong>Exception Handling Patterns:</strong> These extensions support common
    /// exception handling patterns used in enterprise applications, particularly when
    /// working with async/await patterns and exception propagation.
    /// </para>
    /// </remarks>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Re-throws an exception while preserving its original stack trace.
        /// </summary>
        /// <param name="exception">
        /// The exception to be re-thrown.
        /// </param>
        /// <remarks>
        /// <para>
        /// This method uses <see cref="ExceptionDispatchInfo.Capture"/> to re-throw an
        /// exception while preserving its original stack trace. This is particularly useful
        /// in exception handling scenarios where you need to re-throw an exception but
        /// want to maintain the original call stack for debugging purposes.
        /// </para>
        /// <para>
        /// <strong>Stack Trace Preservation:</strong> Unlike using the standard throw
        /// statement, which creates a new stack trace starting from the current location,
        /// this method preserves the original stack trace of the exception. This makes
        /// debugging much easier as you can see the actual call path that led to the
        /// original exception.
        /// </para>
        /// <para>
        /// <strong>Use Cases:</strong>
        /// <list type="bullet">
        /// <item><description>Exception handling in async/await patterns</description></item>
        /// <item><description>Re-throwing exceptions in catch blocks</description></item>
        /// <item><description>Exception propagation in middleware or interceptors</description></item>
        /// <item><description>Logging and re-throwing exceptions</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Example:</strong>
        /// <code>
        /// try
        /// {
        ///     // Some operation that might throw
        ///     await SomeAsyncOperation();
        /// }
        /// catch (Exception ex)
        /// {
        ///     // Log the exception
        ///     _logger.LogError(ex, "Operation failed");
        ///     
        ///     // Re-throw while preserving stack trace
        ///     ex.ReThrow();
        /// }
        /// </code>
        /// </para>
        /// <para>
        /// <strong>Important Notes:</strong>
        /// <list type="bullet">
        /// <item><description>This method will terminate the current method execution</description></item>
        /// <item><description>The exception will be thrown immediately when this method is called</description></item>
        /// <item><description>This method does not return a value (void method)</description></item>
        /// <item><description>Use this method when you want to preserve the original exception context</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Performance:</strong> This method has minimal performance overhead
        /// compared to standard exception throwing, but provides significant benefits
        /// for debugging and error diagnosis.
        /// </para>
        /// </remarks>
        public static void ReThrow(this Exception exception)
        {
            ExceptionDispatchInfo.Capture(exception).Throw();
        }
    }
}
