using Kontecg.Configuration.Startup;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.EntityFrameworkCore.Tests.Domain;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Kontecg.EntityFrameworkCore.Tests.Tests
{
    public class Repository_Filtering_Tests : EntityFrameworkCoreModuleTestBase
    {
        private readonly IRepository<Post, Guid> _postRepository;
        private readonly IRepository<Blog> _blogRepository;
        private readonly IRepository<Ticket> _ticketRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<TicketListItem> _ticketListItemRepository;

        public Repository_Filtering_Tests()
        {
            _unitOfWorkManager = Resolve<IUnitOfWorkManager>();

            _postRepository = Resolve<IRepository<Post, Guid>>();
            _blogRepository = Resolve<IRepository<Blog>>();
            _ticketRepository = Resolve<IRepository<Ticket>>();
            _ticketListItemRepository = Resolve<IRepository<TicketListItem>>();
        }

        override protected void PostInitialize()
        {
            Resolve<IMultiCompanyConfig>().IsEnabled = true;
        }

        [Fact]
        public async Task Should_Filter_SoftDelete()
        {
            var posts = await _postRepository.GetAllListAsync();
            posts.All(p => !p.IsDeleted).ShouldBeTrue();
        }

        [Fact]
        public async Task Should_Get_SoftDeleted_Entities_If_Filter_Is_Disabled()
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.DisableFilter(KontecgDataFilters.SoftDelete))
                {
                    var posts = await _postRepository.GetAllListAsync();
                    posts.Any(p => p.IsDeleted).ShouldBeTrue();
                }
            }
        }

        [Fact]
        public async Task Should_Filter_MayHaveCompanyId()
        {
            var postsDefault = await _postRepository.GetAllListAsync();
            postsDefault.Any(p => p.CompanyId == null).ShouldBeTrue();

            //Switch to company 42
            KontecgSession.CompanyId = 42;

            var posts1 = await _postRepository.GetAllListAsync();
            posts1.All(p => p.CompanyId == 42).ShouldBeTrue();

            //Switch to host
            KontecgSession.CompanyId = null;
            
            var posts2 = await _postRepository.GetAllListAsync();
            posts2.Any(p => p.CompanyId == 42).ShouldBeFalse();

            using (var uow = _unitOfWorkManager.Begin())
            {
                //Switch to company 42
                using (_unitOfWorkManager.Current.SetCompanyId(42))
                {
                    var posts3 = await _postRepository.GetAllListAsync(p => p.Title != null);
                    posts3.All(p => p.CompanyId == 42).ShouldBeTrue();
                }

                var posts4 = await _postRepository.GetAllListAsync();
                posts4.Any(p => p.CompanyId == 42).ShouldBeFalse();
                posts4.Any(p => p.CompanyId == null).ShouldBeTrue();

                using (_unitOfWorkManager.Current.DisableFilter(KontecgDataFilters.MayHaveCompany))
                {
                    var posts5 = await _postRepository.GetAllListAsync();
                    posts5.Any(p => p.CompanyId == 42).ShouldBeTrue();
                    posts5.Any(p => p.CompanyId == null).ShouldBeTrue();
                }
            }
        }

        [Fact]
        public async Task Should_Filter_MustHaveCompanyId()
        {
            //Should get all entities for the host
            var ticketsDefault = await _ticketRepository.GetAllListAsync();
            ticketsDefault.Any(t => t.CompanyId == 1).ShouldBeTrue();
            ticketsDefault.Any(t => t.CompanyId == 42).ShouldBeTrue();

            //Switch to company 42
            KontecgSession.CompanyId = 42;
            ticketsDefault = await _ticketRepository.GetAllListAsync();
            ticketsDefault.Any(t => t.CompanyId == 42).ShouldBeTrue();
            ticketsDefault.Any(t => t.CompanyId != 42).ShouldBeFalse();

            //TODO: Create unit test
            //TODO: Change filter
        }

        [Fact]
        public async Task Should_Filter_View_With_MustHaveCompanyId()
        {
            //Should get all entities for the host
            var ticketsDefault = await _ticketListItemRepository.GetAllListAsync();
            ticketsDefault.Any(t => t.CompanyId == 1).ShouldBeTrue();
            ticketsDefault.Any(t => t.CompanyId == 42).ShouldBeTrue();

            //Switch to company 42
            KontecgSession.CompanyId = 42;
            ticketsDefault = await _ticketListItemRepository.GetAllListAsync();
            ticketsDefault.Any(t => t.CompanyId == 42).ShouldBeTrue();
            ticketsDefault.Any(t => t.CompanyId != 42).ShouldBeFalse();
        }
        
        [Fact]
        public async Task Navigation_Properties_Cascade_Delete_Test()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var blog = await _blogRepository.GetAll().Include(x => x.Posts).FirstOrDefaultAsync(b => b.Name == "test-blog-1");
                blog.Posts.ShouldNotBeEmpty();

                blog.Posts.Clear();
                await _blogRepository.UpdateAsync(blog);
            });
            
            await WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.DisableFilter(KontecgDataFilters.SoftDelete))
                {
                    var blog = await _blogRepository.GetAll().Include(x => x.Posts).FirstOrDefaultAsync(b => b.Name == "test-blog-1");
                    blog.Posts.ShouldNotBeEmpty();
                    blog.Posts.ShouldAllBe(x => x.IsDeleted);
                }
            });
        }
    }
}
