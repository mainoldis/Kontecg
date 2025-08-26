using Kontecg.Dependency;

namespace Kontecg.Authorization
{
    public class PermissionDependencyContext : IPermissionDependencyContext, ITransientDependency
    {
        public PermissionDependencyContext(IIocResolver iocResolver)
        {
            IocResolver = iocResolver;
            PermissionChecker = NullPermissionChecker.Instance;
        }

        public UserIdentifier User { get; set; }

        public IIocResolver IocResolver { get; }

        public IPermissionChecker PermissionChecker { get; set; }
    }
}
