using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using Castle.MicroKernel.Lifestyle;

namespace Kontecg.Castle.MsAdapter
{
    /// <summary>
    ///     Extends Windsor's <see cref="ScopedLifestyleManager" /> to work as
    ///     MS style scope using <see cref="MsScopedAccesor" />.
    /// </summary>
    public class MsScopedLifestyleManager : ScopedLifestyleManager
    {
        public MsScopedLifestyleManager()
            : base(new MsScopedAccesor())
        {
        }

        public override object Resolve(CreationContext context, IReleasePolicy releasePolicy)
        {
            IMsLifetimeScope currentScope = MsLifetimeScope.Current;

            if (currentScope == null)
            {
                //Act as transient!
                Burden burden = CreateInstance(context, false);
                if (!releasePolicy.HasTrack(burden.Instance))
                {
                    Track(burden, releasePolicy);
                }

                return burden.Instance;
            }

            lock (currentScope)
            {
                return base.Resolve(context, releasePolicy);
            }
        }
    }
}
