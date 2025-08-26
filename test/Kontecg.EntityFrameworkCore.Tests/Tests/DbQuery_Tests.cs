using System.Threading.Tasks;
using Kontecg.Domain.Repositories;
using Kontecg.EntityFrameworkCore.Tests.Domain;
using Shouldly;
using Xunit;

namespace Kontecg.EntityFrameworkCore.Tests.Tests
{
    public class DbQuery_Tests : EntityFrameworkCoreModuleTestBase
    {
        [Fact]
        public async Task DbQuery_Test()
        {
            var blogViewRepository = Resolve<IRepository<BlogView>>();

            var blogViews = await blogViewRepository.GetAllListAsync();

            blogViews.ShouldNotBeNull();
            blogViews.ShouldContain(x => x.Name == "test-blog-1" && x.Url == "http://testblog1.myblogs.com");
        }
    }
}