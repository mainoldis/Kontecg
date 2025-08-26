using System;
using AutoMapper;
using Kontecg.Collections.Extensions;

namespace Kontecg.AutoMapper
{
    public class AutoMapAttribute : AutoMapAttributeBase
    {
        public AutoMapAttribute(params Type[] targetTypes)
            : base(targetTypes)
        {
        }

        public override void CreateMap(IMapperConfigurationExpression configuration, Type type)
        {
            if (TargetTypes.IsNullOrEmpty())
            {
                return;
            }

            configuration.CreateAutoAttributeMaps(type, TargetTypes, MemberList.Source);

            foreach (Type targetType in TargetTypes)
            {
                configuration.CreateAutoAttributeMaps(targetType, new[] {type}, MemberList.Destination);
            }
        }
    }
}
