using System;
using System.Linq;
using System.Linq.Expressions;
using Kontecg.EFCore;
using Kontecg.EntityFrameworkCore.Tests.Domain;

namespace Kontecg.EntityFrameworkCore.Tests.Ef
{
    public class TicketListItemRepository : SupportRepositoryBase<TicketListItem>
    {
        private IQueryable<TicketListItem> View => GetContext().TicketListItems;

        public TicketListItemRepository(IDbContextProvider<SupportDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public override IQueryable<TicketListItem> GetAllIncluding(params Expression<Func<TicketListItem, object>>[] propertySelectors) => View;
    }
}
