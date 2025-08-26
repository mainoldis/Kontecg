using System;
using AutoMapper;
using Kontecg.Collections.Extensions;

namespace Kontecg.AutoMapper
{
    public class AutoMapFromAttribute : AutoMapAttributeBase
    {
        public AutoMapFromAttribute(params Type[] targetTypes)
            : base(targetTypes)
        {
        }

        public AutoMapFromAttribute(MemberList memberList, params Type[] targetTypes)
            : this(targetTypes)
        {
            MemberList = memberList;
        }

        public MemberList MemberList { get; set; } = MemberList.Destination;

        public override void CreateMap(IMapperConfigurationExpression configuration, Type type)
        {
            if (TargetTypes.IsNullOrEmpty())
            {
                return;
            }

            foreach (Type targetType in TargetTypes)
            {
                configuration.CreateAutoAttributeMaps(targetType, new[] {type}, MemberList);
            }
        }
    }
}
