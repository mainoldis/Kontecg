namespace Kontecg.Application.Navigation
{
    /// <summary>
    /// Provides context information for navigation providers.
    /// </summary>
    internal class NavigationProviderContext : INavigationProviderContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationProviderContext"/> class.
        /// </summary>
        /// <param name="manager">The navigation manager instance.</param>
        public NavigationProviderContext(INavigationManager manager)
        {
            Manager = manager;
        }

        /// <summary>
        /// Gets the navigation manager associated with this context.
        /// </summary>
        public INavigationManager Manager { get; }
    }
}
