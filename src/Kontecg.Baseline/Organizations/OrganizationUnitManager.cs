using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.Baseline;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Services;
using Kontecg.Domain.Uow;
using Kontecg.Linq;
using Kontecg.UI;

namespace Kontecg.Organizations
{
    /// <summary>
    ///     Performs domain logic for Organization Units.
    /// </summary>
    public class OrganizationUnitManager : DomainService
    {
        public OrganizationUnitManager(IRepository<OrganizationUnit, long> organizationUnitRepository)
        {
            OrganizationUnitRepository = organizationUnitRepository;

            LocalizationSourceName = KontecgBaselineConsts.LocalizationSourceName;
            AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
        }

        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }

        protected IRepository<OrganizationUnit, long> OrganizationUnitRepository { get; }

        public virtual async Task<long> CreateAsync(OrganizationUnit organizationUnit)
        {
            using IUnitOfWorkCompleteHandle uow = UnitOfWorkManager.Begin();
            organizationUnit.Code = await GetNextChildCodeAsync(organizationUnit.ParentId);
            organizationUnit.Order = await CalculateOrderAsync(organizationUnit.ParentId);
            await ValidateOrganizationUnitAsync(organizationUnit);
            long ouId = await OrganizationUnitRepository.InsertAndGetIdAsync(organizationUnit);

            await uow.CompleteAsync();
            return await Task.FromResult(ouId);
        }

        public virtual long Create(OrganizationUnit organizationUnit)
        {
            using IUnitOfWorkCompleteHandle uow = UnitOfWorkManager.Begin();
            organizationUnit.Code = GetNextChildCode(organizationUnit.ParentId);
            organizationUnit.Order = CalculateOrder(organizationUnit.ParentId);
            ValidateOrganizationUnit(organizationUnit);
            long ouId = OrganizationUnitRepository.InsertAndGetId(organizationUnit);

            uow.Complete();
            return ouId;
        }

        public virtual async Task UpdateAsync(OrganizationUnit organizationUnit)
        {
            await ValidateOrganizationUnitAsync(organizationUnit);
            await OrganizationUnitRepository.UpdateAsync(organizationUnit);
        }

        public virtual void Update(OrganizationUnit organizationUnit)
        {
            ValidateOrganizationUnit(organizationUnit);
            OrganizationUnitRepository.Update(organizationUnit);
        }

        public virtual async Task<string> GetNextChildCodeAsync(long? parentId)
        {
            OrganizationUnit lastChild = await GetLastChildOrNullAsync(parentId);
            if (lastChild == null)
            {
                string parentCode = parentId != null ? await GetCodeAsync(parentId.Value) : null;
                return OrganizationUnit.AppendCode(parentCode, OrganizationUnit.CreateCode(1));
            }

            return OrganizationUnit.CalculateNextCode(lastChild.Code);
        }

        public virtual string GetNextChildCode(long? parentId)
        {
            OrganizationUnit lastChild = GetLastChildOrNull(parentId);
            if (lastChild == null)
            {
                string parentCode = parentId != null ? GetCode(parentId.Value) : null;
                return OrganizationUnit.AppendCode(parentCode, OrganizationUnit.CreateCode(1));
            }

            return OrganizationUnit.CalculateNextCode(lastChild.Code);
        }

        public virtual async Task<OrganizationUnit> GetLastChildOrNullAsync(long? parentId)
        {
            IOrderedQueryable<OrganizationUnit> query = OrganizationUnitRepository.GetAll()
                .Where(ou => ou.ParentId == parentId)
                .OrderByDescending(ou => ou.Code);
            return await AsyncQueryableExecuter.FirstOrDefaultAsync(query);
        }

        public virtual OrganizationUnit GetLastChildOrNull(long? parentId)
        {
            IOrderedQueryable<OrganizationUnit> query = OrganizationUnitRepository.GetAll()
                .Where(ou => ou.ParentId == parentId)
                .OrderByDescending(ou => ou.Code);
            return query.FirstOrDefault();
        }

        protected virtual async Task<int> CalculateOrderAsync(long? parentId)
        {
            IQueryable<OrganizationUnit> query = OrganizationUnitRepository.GetAll()
                .Where(ou => ou.ParentId == parentId);
            return await AsyncQueryableExecuter.CountAsync(query);
        }

        protected virtual int CalculateOrder(long? parentId)
        {
            IQueryable<OrganizationUnit> query = OrganizationUnitRepository.GetAll()
                .Where(ou => ou.ParentId == parentId);
            return query.Count();
        }

        public virtual async Task<string> GetCodeAsync(long id)
        {
            return (await OrganizationUnitRepository.GetAsync(id)).Code;
        }

        public virtual string GetCode(long id)
        {
            return OrganizationUnitRepository.Get(id).Code;
        }

        public virtual async Task DeleteAsync(long id)
        {
            using IUnitOfWorkCompleteHandle uow = UnitOfWorkManager.Begin();
            List<OrganizationUnit> children = await FindChildrenAsync(id, true);

            foreach (OrganizationUnit child in children)
            {
                await OrganizationUnitRepository.DeleteAsync(child);
            }

            await OrganizationUnitRepository.DeleteAsync(id);

            await uow.CompleteAsync();
        }

        public virtual void Delete(long id)
        {
            using IUnitOfWorkCompleteHandle uow = UnitOfWorkManager.Begin();
            List<OrganizationUnit> children = FindChildren(id, true);

            foreach (OrganizationUnit child in children)
            {
                OrganizationUnitRepository.Delete(child);
            }

            OrganizationUnitRepository.Delete(id);

            uow.Complete();
        }

        public virtual async Task MoveAsync(long id, long? parentId)
        {
            using IUnitOfWorkCompleteHandle uow = UnitOfWorkManager.Begin();
            OrganizationUnit organizationUnit = await OrganizationUnitRepository.GetAsync(id);
            if (organizationUnit.ParentId == parentId)
            {
                await uow.CompleteAsync();
                return;
            }

            //Should find children before Code change
            List<OrganizationUnit> children = await FindChildrenAsync(id, true);

            //Store old code of OU
            string oldCode = organizationUnit.Code;

            //Move OU
            organizationUnit.Code = await GetNextChildCodeAsync(parentId);
            organizationUnit.Order = await CalculateOrderAsync(parentId);
            organizationUnit.ParentId = parentId;

            await ValidateOrganizationUnitAsync(organizationUnit);

            //Update Children Codes
            foreach (OrganizationUnit child in children)
            {
                child.Code = OrganizationUnit.AppendCode(organizationUnit.Code,
                    OrganizationUnit.GetRelativeCode(child.Code, oldCode));
            }

            await uow.CompleteAsync();
        }

        public virtual void Move(long id, long? parentId)
        {
            UnitOfWorkManager.WithUnitOfWork(() =>
            {
                OrganizationUnit organizationUnit = OrganizationUnitRepository.Get(id);
                if (organizationUnit.ParentId == parentId)
                {
                    return;
                }

                //Should find children before Code change
                List<OrganizationUnit> children = FindChildren(id, true);

                //Store old code of OU
                string oldCode = organizationUnit.Code;

                //Move OU
                organizationUnit.Code = GetNextChildCode(parentId);
                organizationUnit.Order = CalculateOrder(parentId);
                organizationUnit.ParentId = parentId;

                ValidateOrganizationUnit(organizationUnit);

                //Update Children Codes
                foreach (OrganizationUnit child in children)
                {
                    child.Code = OrganizationUnit.AppendCode(organizationUnit.Code,
                        OrganizationUnit.GetRelativeCode(child.Code, oldCode));
                }
            });
        }

        public async Task<List<OrganizationUnit>> FindChildrenAsync(long? parentId, bool recursive = false)
        {
            if (!recursive)
            {
                return await OrganizationUnitRepository.GetAllListAsync(ou => ou.ParentId == parentId);
            }

            if (!parentId.HasValue)
            {
                return await OrganizationUnitRepository.GetAllListAsync();
            }

            string code = await GetCodeAsync(parentId.Value);

            return await OrganizationUnitRepository.GetAllListAsync(
                ou => ou.Code.StartsWith(code) && ou.Id != parentId.Value
            );
        }

        public List<OrganizationUnit> FindChildren(long? parentId, bool recursive = false)
        {
            if (!recursive)
            {
                return OrganizationUnitRepository.GetAllList(ou => ou.ParentId == parentId);
            }

            if (!parentId.HasValue)
            {
                return OrganizationUnitRepository.GetAllList();
            }

            string code = GetCode(parentId.Value);

            return OrganizationUnitRepository.GetAllList(
                ou => ou.Code.StartsWith(code) && ou.Id != parentId.Value
            );
        }

        protected virtual async Task ValidateOrganizationUnitAsync(OrganizationUnit organizationUnit)
        {
            List<OrganizationUnit> siblings = (await FindChildrenAsync(organizationUnit.ParentId))
                .Where(ou => ou.Id != organizationUnit.Id)
                .ToList();

            if (siblings.Any(ou => ou.DisplayName == organizationUnit.DisplayName))
            {
                throw new UserFriendlyException(L("OrganizationUnitDuplicateDisplayNameWarning",
                    organizationUnit.DisplayName));
            }
        }

        protected virtual void ValidateOrganizationUnit(OrganizationUnit organizationUnit)
        {
            List<OrganizationUnit> siblings = FindChildren(organizationUnit.ParentId)
                .Where(ou => ou.Id != organizationUnit.Id)
                .ToList();

            if (siblings.Any(ou => ou.DisplayName == organizationUnit.DisplayName))
            {
                throw new UserFriendlyException(L("OrganizationUnitDuplicateDisplayNameWarning",
                    organizationUnit.DisplayName));
            }
        }
    }
}
