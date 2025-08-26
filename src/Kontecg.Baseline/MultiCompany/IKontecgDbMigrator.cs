namespace Kontecg.MultiCompany
{
    public interface IKontecgDbMigrator
    {
        void CreateOrMigrateForHost();

        void CreateOrMigrateForCompany(KontecgCompanyBase company);
    }
}
