using System.Linq;
using System.Reflection;
using Kontecg.Domain.Entities;
using Kontecg.Extensions;

namespace Kontecg.MultiCompany
{
    internal class MultiCompanyHelper
    {
        public static bool IsMultiCompanyEntity(object entity)
        {
            return entity is IMayHaveCompany || entity is IMustHaveCompany;
        }

        /// <param name="entity">The entity to check</param>
        /// <param name="expectedCompanyId">CompanyId or null for host</param>
        public static bool IsCompanyEntity(object entity, int? expectedCompanyId)
        {
            return (entity is IMayHaveCompany && entity.As<IMayHaveCompany>().CompanyId == expectedCompanyId) ||
                   (entity is IMustHaveCompany && entity.As<IMustHaveCompany>().CompanyId == expectedCompanyId);
        }

        public static bool IsHostEntity(object entity)
        {
            MultiCompanySideAttribute attribute = entity.GetType().GetTypeInfo()
                .GetCustomAttributes(typeof(MultiCompanySideAttribute), true)
                .Cast<MultiCompanySideAttribute>()
                .FirstOrDefault();

            return attribute != null && attribute.Side.HasFlag(MultiCompanySides.Host);
        }
    }
}
