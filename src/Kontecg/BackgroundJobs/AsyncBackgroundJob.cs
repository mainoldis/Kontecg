using System.Threading.Tasks;

namespace Kontecg.BackgroundJobs
{
    /// <summary>
    /// Provides a base implementation for asynchronous background job processing with strongly-typed arguments.
    /// This abstract class serves as the foundation for implementing background jobs that
    /// can be executed asynchronously and support non-blocking operations.
    /// </summary>
    /// <typeparam name="TArgs">The type of the arguments that will be passed to the background job
    /// for execution. This type should contain all the data necessary for the job to perform
    /// its work.</typeparam>
    /// <remarks>
    /// AsyncBackgroundJob is designed for background job implementations that need to perform
    /// I/O-bound operations, network calls, or other asynchronous work. Unlike the synchronous
    /// BackgroundJob class, this class allows jobs to perform non-blocking operations, making
    /// better use of system resources and improving overall application performance. The class
    /// inherits from BackgroundJobBase to provide common functionality while enforcing the
    /// implementation of the ExecuteAsync method. Jobs can be scheduled, queued, and executed
    /// by the background job infrastructure with proper error handling and retry mechanisms.
    /// </remarks>
    public abstract class AsyncBackgroundJob<TArgs> : BackgroundJobBase<TArgs>, IAsyncBackgroundJob<TArgs>
    {
        /// <summary>
        /// Asynchronously executes the background job with the specified arguments.
        /// </summary>
        /// <param name="args">The strongly-typed arguments containing the data and parameters
        /// required for the job execution. This object should contain all necessary
        /// information for the job to perform its intended work.</param>
        /// <returns>
        /// A task that represents the asynchronous execution operation. The task completes
        /// when the background job has finished processing, whether successfully or with an error.
        /// </returns>
        /// <remarks>
        /// This method must be implemented by derived classes to define the actual asynchronous
        /// work that the background job should perform. The method is called by the background
        /// job infrastructure when the job is ready to be executed. Implementations should
        /// handle their own exception management and provide appropriate logging for debugging
        /// and monitoring purposes. The method should be designed to be idempotent when possible,
        /// as background jobs may be retried in case of failures. The asynchronous nature of
        /// this method allows for better resource utilization and improved scalability compared
        /// to synchronous job execution.
        /// </remarks>
        public abstract Task ExecuteAsync(TArgs args);
    }
}
