using System;
using Kontecg.Application.Services;

namespace Kontecg.Authorization
{
    /// <summary>
    ///     This attribute is used on a method of an Application Service (A class that implements
    ///     <see cref="IApplicationService" />)
    ///     to make that method usable only by authorized users.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class KontecgAuthorizeAttribute : Attribute, IKontecgAuthorizeAttribute
    {
        /// <summary>
        ///     Creates a new instance of <see cref="KontecgAuthorizeAttribute" /> class.
        /// </summary>
        /// <param name="permissions">A list of permissions to authorize</param>
        public KontecgAuthorizeAttribute(params string[] permissions)
        {
            Permissions = permissions;
        }

        /// <summary>
        ///     A list of permissions to authorize.
        /// </summary>
        public string[] Permissions { get; }

        /// <summary>
        ///     If this property is set to true, all of the <see cref="Permissions" /> must be granted.
        ///     If it's false, at least one of the <see cref="Permissions" /> must be granted.
        ///     Default: false.
        /// </summary>
        public bool RequireAllPermissions { get; set; }
    }
}
