using System;
using Kontecg.Events.Bus.Handlers;
using Kontecg.Reflection;

namespace Kontecg.Events.Bus.Factories.Internals
{
    /// <summary>
    ///     This <see cref="IEventHandlerFactory" /> implementation is used to handle events
    ///     by a single instance object.
    /// </summary>
    /// <remarks>
    ///     This class always gets the same single instance of handler.
    /// </remarks>
    internal class SingleInstanceHandlerFactory : IEventHandlerFactory
    {
        /// <summary>
        /// </summary>
        /// <param name="handler"></param>
        public SingleInstanceHandlerFactory(IEventHandler handler)
        {
            HandlerInstance = handler;
        }

        /// <summary>
        ///     The event handler instance.
        /// </summary>
        public IEventHandler HandlerInstance { get; }

        public IEventHandler GetHandler()
        {
            return HandlerInstance;
        }

        public Type GetHandlerType()
        {
            return ProxyHelper.UnProxy(HandlerInstance).GetType();
        }

        public void ReleaseHandler(IEventHandler handler)
        {
        }
    }
}
