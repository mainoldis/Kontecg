using System;
using Kontecg.EFCore;
using Kontecg.EFCore.Repositories;
using Kontecg.EntityFrameworkCore.Tests.Domain;

namespace Kontecg.EntityFrameworkCore.Tests.Ef
{
    public class PostRepository : EfCoreRepositoryBase<BloggingDbContext, Post, Guid>, IPostRepository
    {
        public PostRepository(IDbContextProvider<BloggingDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public override int Count()
        {
            throw new Exception("can not get count of posts");
        }
    }
}
