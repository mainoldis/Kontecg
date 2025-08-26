using System;

namespace Kontecg.MultiCompany
{
    /// <summary>
    ///     Used to declare multi tenancy side of an object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method |
                    AttributeTargets.Interface)]
    public class MultiCompanySideAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MultiCompanySideAttribute" /> class.
        /// </summary>
        /// <param name="side">Multitenancy side.</param>
        public MultiCompanySideAttribute(MultiCompanySides side)
        {
            Side = side;
        }

        /// <summary>
        ///     Multitenancy side.
        /// </summary>
        public MultiCompanySides Side { get; set; }
    }
}
