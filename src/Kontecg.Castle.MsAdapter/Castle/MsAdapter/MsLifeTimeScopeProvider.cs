namespace Kontecg.Castle.MsAdapter
{
    /// <summary>
    ///     Used to obtain true lifetime scope for dependent service.
    /// </summary>
    public class MsLifetimeScopeProvider
    {
        public MsLifetimeScopeProvider()
        {
            LifetimeScope = MsLifetimeScope.Current;
        }

        public IMsLifetimeScope LifetimeScope { get; }
    }
}
