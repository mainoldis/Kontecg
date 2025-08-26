namespace Kontecg.BackgroundJobs
{
    /// <summary>
    /// Provides a base implementation for background job processing with strongly-typed arguments.
    /// This abstract class serves as the foundation for implementing background jobs that
    /// can be executed asynchronously by the background job system.
    /// </summary>
    /// <typeparam name="TArgs">The type of the arguments that will be passed to the background job
    /// for execution. This type should contain all the data necessary for the job to perform
    /// its work.</typeparam>
    /// <remarks>
    /// BackgroundJob is designed to be inherited by concrete job implementations that need
    /// to perform long-running or resource-intensive operations without blocking the main
    /// application thread. The class provides a structured approach to background job
    /// development by enforcing the implementation of the Execute method and providing
    /// access to the base functionality through BackgroundJobBase. Jobs can be scheduled,
    /// queued, and executed by the background job infrastructure with proper error handling
    /// and retry mechanisms.
    /// </remarks>
    public abstract class BackgroundJob<TArgs> : BackgroundJobBase<TArgs>, IBackgroundJob<TArgs>
    {
        /// <summary>
        /// Executes the background job with the specified arguments.
        /// </summary>
        /// <param name="args">The strongly-typed arguments containing the data and parameters
        /// required for the job execution. This object should contain all necessary
        /// information for the job to perform its intended work.</param>
        /// <remarks>
        /// This method must be implemented by derived classes to define the actual work
        /// that the background job should perform. The method is called by the background
        /// job infrastructure when the job is ready to be executed. Implementations should
        /// handle their own exception management and provide appropriate logging for
        /// debugging and monitoring purposes. The method should be designed to be idempotent
        /// when possible, as background jobs may be retried in case of failures.
        /// </remarks>
        public abstract void Execute(TArgs args);
    }
}
