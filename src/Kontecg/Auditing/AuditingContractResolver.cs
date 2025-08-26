using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Kontecg.Auditing
{
    /// <summary>
    ///     Decides which properties of auditing class to be serialized
    /// </summary>
    public class AuditingContractResolver : CamelCasePropertyNamesContractResolver
    {
        private readonly List<Type> _ignoredTypes;

        public AuditingContractResolver(List<Type> ignoredTypes)
        {
            _ignoredTypes = ignoredTypes;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (member.IsDefined(typeof(DisableAuditingAttribute)) || member.IsDefined(typeof(JsonIgnoreAttribute)))
            {
                property.ShouldSerialize = instance => false;
            }

            foreach (Type ignoredType in _ignoredTypes.Where(ignoredType =>
                         ignoredType.GetTypeInfo().IsAssignableFrom(property.PropertyType)))
            {
                property.ShouldSerialize = instance => false;
                break;
            }

            return property;
        }
    }
}
