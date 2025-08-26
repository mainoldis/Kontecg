using System;

namespace Kontecg.ExceptionHandling
{
    /// <summary>
    ///     Used to disable exception handling for a single method or
    ///     all methods of a class or interface.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class DisableExceptionHandlingAttribute : Attribute
    {
    }
}
