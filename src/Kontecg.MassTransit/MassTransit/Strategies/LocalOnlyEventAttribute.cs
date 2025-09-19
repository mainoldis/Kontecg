using System;

namespace Kontecg.MassTransit.Strategies
{
    /// <summary>
    /// Attribute that indicates an event should remain as local event only.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This attribute is useful when you have a default strategy that publishes events
    /// but want to specifically exclude certain event types.
    /// </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class LocalOnlyEventAttribute : Attribute
    {
    }
}
