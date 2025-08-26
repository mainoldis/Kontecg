using Kontecg.Runtime.Remoting;

namespace Kontecg.EntityHistory
{
    /// <summary>
    ///     Implements null object pattern for <see cref="IEntityChangeSetReasonProvider" />.
    /// </summary>
    public class NullEntityChangeSetReasonProvider : EntityChangeSetReasonProviderBase
    {
        private NullEntityChangeSetReasonProvider()
            : base(
                new DataContextAmbientScopeProvider<ReasonOverride>(new AsyncLocalAmbientDataContext())
            )
        {
        }

        /// <summary>
        ///     Singleton instance.
        /// </summary>
        public static NullEntityChangeSetReasonProvider Instance { get; } = new();

        /// <inheritdoc />
        public override string Reason => null;
    }
}
