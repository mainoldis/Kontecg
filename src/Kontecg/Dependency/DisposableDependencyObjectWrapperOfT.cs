namespace Kontecg.Dependency
{
    /// <summary>
    /// Provides a non-generic wrapper for disposable dependency objects that automatically
    /// releases resources when disposed.
    /// </summary>
    /// <remarks>
    /// This class is a convenience wrapper that simplifies the management of non-generic
    /// dependency objects. It inherits from the generic DisposableDependencyObjectWrapper
    /// with object as the type parameter, providing a consistent interface for resource
    /// management regardless of the object type. The wrapper ensures that the underlying
    /// object is properly released back to the IoC container when the wrapper is disposed.
    /// </remarks>
    internal class DisposableDependencyObjectWrapper : DisposableDependencyObjectWrapper<object>,
        IDisposableDependencyObjectWrapper
    {
        /// <summary>
        /// Initializes a new instance of the DisposableDependencyObjectWrapper class.
        /// </summary>
        /// <param name="iocResolver">The IoC resolver used to release the wrapped object.</param>
        /// <param name="obj">The object to be wrapped and managed.</param>
        /// <remarks>
        /// This constructor creates a wrapper around the specified object and associates
        /// it with the provided IoC resolver. The wrapper will use the resolver to properly
        /// release the object when disposed, ensuring that any resources associated with
        /// the object are cleaned up appropriately.
        /// </remarks>
        public DisposableDependencyObjectWrapper(IIocResolver iocResolver, object obj)
            : base(iocResolver, obj)
        {
        }
    }

    /// <summary>
    /// Provides a generic wrapper for disposable dependency objects that automatically
    /// releases resources when disposed.
    /// </summary>
    /// <typeparam name="T">The type of the object being wrapped and managed.</typeparam>
    /// <remarks>
    /// DisposableDependencyObjectWrapper is designed to provide automatic resource management
    /// for objects resolved from the IoC container. It implements the IDisposable pattern
    /// to ensure that wrapped objects are properly released back to the container when
    /// the wrapper is disposed. This is particularly useful in scenarios where objects
    /// need to be manually managed or when using the using statement for automatic cleanup.
    /// The wrapper maintains a reference to both the IoC resolver and the wrapped object,
    /// allowing for proper cleanup when the wrapper goes out of scope.
    /// </remarks>
    internal class DisposableDependencyObjectWrapper<T> : IDisposableDependencyObjectWrapper<T>
    {
        private readonly IIocResolver _iocResolver;

        /// <summary>
        /// Initializes a new instance of the DisposableDependencyObjectWrapper class.
        /// </summary>
        /// <param name="iocResolver">The IoC resolver used to release the wrapped object.</param>
        /// <param name="obj">The object to be wrapped and managed.</param>
        /// <remarks>
        /// This constructor creates a wrapper around the specified object and associates
        /// it with the provided IoC resolver. The wrapper will use the resolver to properly
        /// release the object when disposed, ensuring that any resources associated with
        /// the object are cleaned up appropriately. The wrapped object is stored and can
        /// be accessed through the Object property.
        /// </remarks>
        public DisposableDependencyObjectWrapper(IIocResolver iocResolver, T obj)
        {
            _iocResolver = iocResolver;
            Object = obj;
        }

        /// <summary>
        /// Gets the wrapped object that is being managed by this wrapper.
        /// </summary>
        /// <value>
        /// The object of type T that was passed to the constructor and is being managed
        /// by this wrapper instance.
        /// </value>
        /// <remarks>
        /// This property provides access to the wrapped object. The object remains available
        /// for use until the wrapper is disposed. Once disposed, the object will be released
        /// back to the IoC container, and any subsequent access should be avoided to prevent
        /// potential issues with object lifecycle management.
        /// </remarks>
        public T Object { get; }

        /// <summary>
        /// Releases the wrapped object back to the IoC container and performs cleanup.
        /// </summary>
        /// <remarks>
        /// This method is called when the wrapper is disposed, either explicitly or through
        /// the using statement. It uses the IoC resolver to properly release the wrapped
        /// object, allowing the container to clean up any resources associated with the
        /// object. This ensures that the object lifecycle is properly managed and prevents
        /// resource leaks in the application.
        /// </remarks>
        public void Dispose()
        {
            _iocResolver.Release(Object);
        }
    }
}
