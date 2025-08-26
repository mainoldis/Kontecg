using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Identity.Contracts;
using Elsa.Identity.Entities;
using Elsa.Identity.Models;
using Kontecg.Dependency;

namespace Kontecg.Workflows.Identity
{
    internal class KontecgElsaRoleProvider : IRoleProvider
    {
        private readonly IIocResolver _iocResolver;

        public KontecgElsaRoleProvider(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        public async ValueTask<IEnumerable<Role>> FindManyAsync(RoleFilter filter, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }
    }
}
