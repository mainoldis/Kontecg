using Kontecg.Domain.Entities;

namespace Kontecg.EntityFrameworkCore.Tests.Domain
{
    public class Ticket : Entity, IPassivable, IMustHaveCompany
    {
        public virtual string EmailAddress { get; set; }

        public virtual string Message { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual int CompanyId { get; set; }

        public Ticket()
        {
            IsActive = true;
        }
    }
}
