using Kontecg.Domain.Entities;

namespace Kontecg.EntityFrameworkCore.Tests.Domain
{
    public class Comment : Entity
    {
        public Post Post { get; set; }

        public string Content { get; set; }
    }
}