using Kontecg.Dependency;

namespace Kontecg.Configuration
{
    public class CustomConfigProviderContext
    {
        public CustomConfigProviderContext(IScopedIocResolver iocResolver)
        {
            IocResolver = iocResolver;
        }

        public IScopedIocResolver IocResolver { get; }
    }
}
