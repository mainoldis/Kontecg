using System;
using System.Collections.Generic;
using System.Linq;
using Kontecg.BackgroundJobs;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.Extensions;
using Kontecg.MultiCompany;
using Kontecg.Threading.BackgroundWorkers;
using Kontecg.Threading.Timers;
using Kontecg.Timing;

namespace Kontecg.Authorization.Users
{
    public class UserTokenExpirationWorker<TCompany, TUser> : PeriodicBackgroundWorkerBase
        where TCompany : KontecgCompany<TUser>
        where TUser : KontecgUserBase
    {
        private readonly IRepository<TCompany> _companyRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<UserToken, long> _userTokenRepository;

        public UserTokenExpirationWorker(
            KontecgTimer timer,
            IRepository<UserToken, long> userTokenRepository,
            IBackgroundJobConfiguration backgroundJobConfiguration,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<TCompany> companyRepository)
            : base(timer)
        {
            _userTokenRepository = userTokenRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _companyRepository = companyRepository;

            Timer.Period = backgroundJobConfiguration.UserTokenExpirationPeriod?.TotalMilliseconds.To<int>() ??
                           TimeSpan.FromHours(1).TotalMilliseconds.To<int>();
        }

        protected override void DoWork()
        {
            List<int> companyIds;
            DateTime utcNow = Clock.Now.ToUniversalTime();

            using (IUnitOfWorkCompleteHandle uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetCompanyId(null))
                {
                    _userTokenRepository.Delete(t => t.ExpireDate <= utcNow);
                    companyIds = _companyRepository.GetAll().Select(t => t.Id).ToList();
                    uow.Complete();
                }
            }

            foreach (int companyId in companyIds)
            {
                using IUnitOfWorkCompleteHandle uow = _unitOfWorkManager.Begin();
                using (_unitOfWorkManager.Current.SetCompanyId(companyId))
                {
                    _userTokenRepository.Delete(t => t.ExpireDate <= utcNow);
                    uow.Complete();
                }
            }
        }
    }
}
