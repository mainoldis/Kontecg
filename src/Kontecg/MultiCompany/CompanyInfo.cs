namespace Kontecg.MultiCompany
{
    public class CompanyInfo
    {
        public CompanyInfo(int id, string companyName)
        {
            Id = id;
            CompanyName = companyName;
        }

        public int Id { get; set; }

        public string CompanyName { get; set; }
    }
}
