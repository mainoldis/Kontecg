using System;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Identity.Contracts;
using Elsa.Identity.Entities;
using Elsa.Identity.Models;
using Kontecg.Dependency;

namespace Kontecg.Workflows.Identity
{
    internal class KontecgElsaUserProvider : IUserProvider
    {
        private readonly IIocResolver _iocResolver;

        public KontecgElsaUserProvider(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        public async Task<User> FindAsync(UserFilter filter, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }
    }
}
