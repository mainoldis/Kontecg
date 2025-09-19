using MassTransit;
using System;
using Kontecg.Reflection.Extensions;
using MassTransit.Internals;

namespace Kontecg.MassTransit.NamingConventions
{
    public class KontecgMessageEntityNameFormatter<T> : IMessageEntityNameFormatter<T> where T : class
    {
        /// <inheritdoc />
        public string FormatEntityName()
        {
            var entityNameAttribute = typeof(T).GetSingleAttributeOrNull<EntityNameAttribute>();
            if (entityNameAttribute != null)
                return entityNameAttribute.EntityName;

            if (typeof(T).ClosesType(typeof(Fault<>), out Type[] messageTypes))
            {
                var faultEntityNameAttribute = messageTypes[0].GetSingleAttributeOrNull<FaultEntityNameAttribute>();
                if (faultEntityNameAttribute != null)
                    return faultEntityNameAttribute.EntityName;
            }

            return typeof(T).Name;
        }
    }
}
