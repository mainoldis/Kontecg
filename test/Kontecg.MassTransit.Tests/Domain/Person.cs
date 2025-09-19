using Kontecg.Application.Services.Dto;

namespace Kontecg.MassTransit.Tests.Domain
{
    public class PersonDto : EntityDto
    {
        /// <inheritdoc />
        public PersonDto(int id, string name)
            : base(id)
        {
            Name = name;
        }

        public string Name {get; set; }
    }
}
