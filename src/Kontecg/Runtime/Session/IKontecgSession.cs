using System;
using Kontecg.MultiCompany;

namespace Kontecg.Runtime.Session
{
    /// <summary>
    ///     Defines some session information that can be useful for applications.
    /// </summary>
    public interface IKontecgSession
    {
        /// <summary>
        ///     Gets current UserId or null.
        ///     It can be null if no user logged in.
        /// </summary>
        long? UserId { get; }

        /// <summary>
        ///     Gets current CompanyId or null.
        ///     This CompanyId should be the CompanyId of the <see cref="UserId" />.
        ///     It can be null if given <see cref="UserId" /> is a host user or no user logged in.
        /// </summary>
        int? CompanyId { get; }

        /// <summary>
        ///     Gets current multi-company side.
        /// </summary>
        MultiCompanySides MultiCompanySide { get; }

        /// <summary>
        ///     UserId of the impersonator.
        ///     This is filled if a user is performing actions behalf of the
        ///     <see cref="P:Kontecg.Runtime.Session.IKontecgSession.UserId" />.
        /// </summary>
        long? ImpersonatorUserId { get; }

        /// <summary>
        ///     CompanyId of the impersonator.
        ///     This is filled if a user with <see cref="P:Kontecg.Runtime.Session.IKontecgSession.ImpersonatorUserId" />
        ///     performing actions behalf of the <see cref="P:Kontecg.Runtime.Session.IKontecgSession.UserId" />.
        /// </summary>
        int? ImpersonatorCompanyId { get; }

        /// <summary>
        ///     Used to change <see cref="CompanyId" /> and <see cref="UserId" /> for a limited scope.
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        IDisposable Use(int? companyId, long? userId);
    }
}
