using System;
using System.Reflection;

namespace Kontecg.Aspects
{
    //THIS NAMESPACE IS WORK-IN-PROGRESS

    internal abstract class AspectAttribute : Attribute
    {
        protected AspectAttribute(Type interceptorType)
        {
            InterceptorType = interceptorType;
        }

        public Type InterceptorType { get; set; }
    }

    internal interface IKontecgInterceptionContext
    {
        object Target { get; }

        MethodInfo Method { get; }

        object[] Arguments { get; }

        object ReturnValue { get; }

        bool Handled { get; set; }
    }

    internal interface IKontecgBeforeExecutionInterceptionContext : IKontecgInterceptionContext
    {
    }


    internal interface IKontecgAfterExecutionInterceptionContext : IKontecgInterceptionContext
    {
        Exception Exception { get; }
    }

    internal interface IKontecgInterceptor<TAspect>
    {
        TAspect Aspect { get; set; }

        void BeforeExecution(IKontecgBeforeExecutionInterceptionContext context);

        void AfterExecution(IKontecgAfterExecutionInterceptionContext context);
    }

    internal abstract class KontecgInterceptorBase<TAspect> : IKontecgInterceptor<TAspect>
    {
        public TAspect Aspect { get; set; }

        public virtual void BeforeExecution(IKontecgBeforeExecutionInterceptionContext context)
        {
        }

        public virtual void AfterExecution(IKontecgAfterExecutionInterceptionContext context)
        {
        }
    }

    internal class TestAspects
    {
        internal class MyAspectAttribute : AspectAttribute
        {
            public MyAspectAttribute()
                : base(typeof(MyInterceptor))
            {
            }

            public int TestValue { get; set; }
        }

        internal class MyInterceptor : KontecgInterceptorBase<MyAspectAttribute>
        {
            public override void BeforeExecution(IKontecgBeforeExecutionInterceptionContext context)
            {
                Aspect.TestValue++;
            }

            public override void AfterExecution(IKontecgAfterExecutionInterceptionContext context)
            {
                Aspect.TestValue++;
            }
        }

        public class MyService
        {
            [MyAspect(TestValue = 41)] //Usage!
            public void DoIt()
            {
            }
        }

        public class MyClient
        {
            private readonly MyService _service;

            public MyClient(MyService service)
            {
                _service = service;
            }

            public void Test()
            {
                _service.DoIt();
            }
        }
    }
}
