using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Kontecg.Application.Services.Dto
{
    /// <summary>
    ///     A shortcut of <see cref="EntityDto{TPrimaryKey}" /> for most used primary key type (<see cref="int" />).
    /// </summary>
    [Serializable]
    public class EntityDto : EntityDto<int>, IEntityDto
    {
        /// <summary>
        ///     Creates a new <see cref="EntityDto" /> object.
        /// </summary>
        public EntityDto()
        {
        }

        /// <summary>
        ///     Creates a new <see cref="EntityDto" /> object.
        /// </summary>
        /// <param name="id">Id of the entity</param>
        public EntityDto(int id)
            : base(id)
        {
        }
    }

    /// <summary>
    ///     Implements common properties for entity based DTOs.
    /// </summary>
    /// <typeparam name="TPrimaryKey">Type of the primary key</typeparam>
    [Serializable]
    public class EntityDto<TPrimaryKey> : IEntityDto<TPrimaryKey>
    {
        /// <summary>
        ///     Creates a new <see cref="EntityDto{TPrimaryKey}" /> object.
        /// </summary>
        public EntityDto()
        {
        }

        /// <summary>
        ///     Creates a new <see cref="EntityDto{TPrimaryKey}" /> object.
        /// </summary>
        /// <param name="id">Id of the entity</param>
        public EntityDto(TPrimaryKey id)
        {
            Id = id;
        }

        /// <summary>
        ///     Id of the entity.
        /// </summary>
        public TPrimaryKey Id { get; set; }

        public virtual event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
