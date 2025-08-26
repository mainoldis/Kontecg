using System;
using Kontecg.Domain.Repositories;
using Kontecg.EntityFrameworkCore.Tests.Domain;

namespace Kontecg.EntityFrameworkCore.Tests.Ef
{
    public interface IPostRepository : IRepository<Post, Guid>
    {
    }
}