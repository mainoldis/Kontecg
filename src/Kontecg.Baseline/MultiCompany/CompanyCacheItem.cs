using System;

namespace Kontecg.MultiCompany
{
    [Serializable]
    public class CompanyCacheItem
    {
        public const string CacheName = "KontecgBaselineCompanyCache";

        public const string ByNameCacheName = "KontecgBaselineCompanyByNameCache";

        public int Id { get; set; }

        public string Name { get; set; }

        public string CompanyName { get; set; }

        public string ConnectionString { get; set; }

        public bool IsActive { get; set; }

        public object CustomData { get; set; }
    }
}
