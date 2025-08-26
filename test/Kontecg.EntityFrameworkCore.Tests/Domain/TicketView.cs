using Kontecg.Domain.Entities;

namespace Kontecg.EntityFrameworkCore.Tests.Domain
{
    public class TicketListItem : IPassivable, IMustHaveCompany, IEntity<int>
    {
        public int Id { get; set; }

        public virtual string EmailAddress { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual int CompanyId { get; set; }

        public bool IsTransient()
        {
            return Id <= 0;
        }
    }
}
