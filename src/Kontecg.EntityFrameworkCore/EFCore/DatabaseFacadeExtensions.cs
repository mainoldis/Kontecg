using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Kontecg.EFCore
{
    public static class DatabaseFacadeExtensions
    {
        public static bool IsRelational(this DatabaseFacade database)
        {
            return database.GetInfrastructure().GetService<IRelationalConnection>() != null;
        }
    }
}
