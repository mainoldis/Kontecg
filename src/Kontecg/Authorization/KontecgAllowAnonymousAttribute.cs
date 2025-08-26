using System;

namespace Kontecg.Authorization
{
    /// <summary>
    ///     Used to allow a method to be accessed by any user.
    ///     Suppress <see cref="KontecgAuthorizeAttribute" /> defined in the class containing that method.
    /// </summary>
    public class KontecgAllowAnonymousAttribute : Attribute, IKontecgAllowAnonymousAttribute
    {
    }
}
