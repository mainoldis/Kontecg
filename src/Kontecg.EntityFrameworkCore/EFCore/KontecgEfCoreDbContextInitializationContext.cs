using Kontecg.Domain.Uow;

namespace Kontecg.EFCore
{
    public class KontecgEfDbContextInitializationContext
    {
        public KontecgEfDbContextInitializationContext(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public IUnitOfWork UnitOfWork { get; }
    }
}
