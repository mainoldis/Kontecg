using System;
using AutoMapper;
using Kontecg.Collections.Extensions;

namespace Kontecg.AutoMapper
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Field |
                    AttributeTargets.Property)]
    public class AutoMapToAttribute : AutoMapAttributeBase
    {
        public AutoMapToAttribute(params Type[] targetTypes)
            : base(targetTypes)
        {
        }

        public AutoMapToAttribute(MemberList memberList, params Type[] targetTypes)
            : this(targetTypes)
        {
            MemberList = memberList;
        }

        public MemberList MemberList { get; set; } = MemberList.Source;

        public override void CreateMap(IMapperConfigurationExpression configuration, Type type)
        {
            if (TargetTypes.IsNullOrEmpty())
            {
                return;
            }

            configuration.CreateAutoAttributeMaps(type, TargetTypes, MemberList);
        }
    }
}
