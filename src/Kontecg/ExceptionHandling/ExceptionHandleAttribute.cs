using System;

namespace Kontecg.ExceptionHandling
{
    /// <summary>
    ///     This attribute is used to apply exception handling for a single method or
    ///     all methods of a class or interface.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class ExceptionHandleAttribute : Attribute
    {
    }
}
