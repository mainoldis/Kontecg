using System;
using Kontecg.MultiCompany;
using Kontecg.Reflection.Extensions;
using Shouldly;
using Xunit;

namespace Kontecg.Tests.Reflection.Extensions
{
    public class MemberInfoExtensions_Tests
    {
        [Theory]
        [InlineData(typeof(MyClass))]
        [InlineData(typeof(MyBaseClass))]
        public void GetSingleAttributeOfTypeOrBaseTypesOrNull_Test(Type type)
        {
            var attr = type.GetSingleAttributeOfTypeOrBaseTypesOrNull<MultiCompanySideAttribute>();
            attr.ShouldNotBeNull();
            attr.Side.ShouldBe(MultiCompanySides.Host);
        }

        private class MyClass : MyBaseClass
        {
            
        }

        [MultiCompanySide(MultiCompanySides.Host)]
        private abstract class MyBaseClass
        {

        }
    }
}
