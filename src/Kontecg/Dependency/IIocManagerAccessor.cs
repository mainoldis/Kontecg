namespace Kontecg.Dependency
{
    /// <summary>
    /// Defines the contract for accessing the IoC (Inversion of Control) manager instance.
    /// This interface provides a standardized way to access the dependency injection container
    /// throughout the application.
    /// </summary>
    /// <remarks>
    /// The IIocManagerAccessor interface is used to provide access to the IoC manager
    /// in scenarios where direct dependency injection is not available or practical.
    /// This is particularly useful in static contexts, legacy code integration, or
    /// when working with frameworks that don't support dependency injection natively.
    /// Implementations of this interface typically hold a reference to the IoC manager
    /// and provide controlled access to it, ensuring that the dependency injection
    /// container is available throughout the application lifecycle.
    /// </remarks>
    public interface IIocManagerAccessor
    {
        /// <summary>
        /// Gets the IoC manager instance that provides access to the dependency injection container.
        /// </summary>
        /// <value>
        /// The IIocManager instance that can be used to resolve dependencies, register services,
        /// and manage the dependency injection container lifecycle.
        /// </value>
        /// <remarks>
        /// This property provides access to the IoC manager, which is the central point
        /// for dependency injection operations. The IoC manager can be used to resolve
        /// services, check if services are registered, and perform other dependency
        /// injection related operations. This accessor pattern is particularly useful
        /// in scenarios where the IoC manager needs to be accessed from contexts that
        /// don't support dependency injection directly.
        /// </remarks>
        IIocManager IocManager { get; }
    }
}
