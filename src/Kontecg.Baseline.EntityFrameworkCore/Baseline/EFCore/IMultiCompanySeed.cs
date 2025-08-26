using Kontecg.MultiCompany;

namespace Kontecg.Baseline.EFCore
{
    public interface IMultiCompanySeed
    {
        KontecgCompanyBase Company { get; set; }
    }
}
