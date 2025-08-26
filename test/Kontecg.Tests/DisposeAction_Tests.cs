using Shouldly;
using Xunit;

namespace Kontecg.Tests
{
    public class DisposeAction_Tests
    {
        [Fact]
        public void Should_Call_Action_When_Disposed()
        {
            var actionIsCalled = false;
            
            using (new DisposeAction(() => actionIsCalled = true))
            {
                
            }

            actionIsCalled.ShouldBe(true);
        }
    }
}
