using Kontecg.Domain.Entities;

namespace Kontecg.EntityFrameworkCore.Tests.Domain
{
    public class BlogView : Entity
    {
        public string Name { get; set; }

        public string Url { get; set; }
    }
}