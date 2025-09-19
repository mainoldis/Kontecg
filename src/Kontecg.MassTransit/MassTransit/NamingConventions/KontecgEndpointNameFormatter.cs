using System;
using MassTransit;

namespace Kontecg.MassTransit.NamingConventions
{
    public class KontecgEndpointNameFormatter : KebabCaseEndpointNameFormatter
    {
        /// <inheritdoc />
        public KontecgEndpointNameFormatter(bool includeNamespace)
            : base(includeNamespace)
        {
        }

        /// <inheritdoc />
        public KontecgEndpointNameFormatter(string prefix)
            : base(prefix)
        {
        }

        /// <inheritdoc />
        public KontecgEndpointNameFormatter(string prefix, bool includeNamespace)
            : base(prefix, includeNamespace)
        {
        }

        /// <inheritdoc />
        private KontecgEndpointNameFormatter()
        {
        }

        public new static IEndpointNameFormatter Instance { get; } = new KontecgEndpointNameFormatter();

        /// <inheritdoc />
        protected override string GetActivityName(Type activityType, Type argumentType)
        {
            return base.GetActivityName(activityType, argumentType);
        }

        /// <inheritdoc />
        protected override string GetConsumerName(Type type)
        {
            return base.GetConsumerName(type);
        }

        /// <inheritdoc />
        protected override string GetMessageName(Type type)
        {
            return base.GetMessageName(type);
        }

        /// <inheritdoc />
        protected override string GetSagaName(Type type)
        {
            return base.GetSagaName(type);
        }
    }
}
