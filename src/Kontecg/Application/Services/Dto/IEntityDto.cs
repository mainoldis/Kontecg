using System.ComponentModel;

namespace Kontecg.Application.Services.Dto
{
    /// <summary>
    ///     A shortcut of <see cref="IEntityDto{TPrimaryKey}" /> for most used primary key type (<see cref="int" />).
    /// </summary>
    public interface IEntityDto : IEntityDto<int>
    {
    }

    /// <summary>
    ///     Defines common properties for entity based DTOs.
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    public interface IEntityDto<TPrimaryKey> : INotifyPropertyChanged
    {
        /// <summary>
        ///     Id of the entity.
        /// </summary>
        TPrimaryKey Id { get; set; }
    }
}
