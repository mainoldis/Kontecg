using Kontecg.Application.Services.Dto;

namespace Kontecg.Application.Services
{
    /// <summary>
    /// Defines a contract for a CRUD application service for a specific DTO type with integer primary keys.
    /// </summary>
    /// <typeparam name="TEntityDto">The DTO type for the entity.</typeparam>
    public interface ICrudAppService<TEntityDto>
        : ICrudAppService<TEntityDto, int>
        where TEntityDto : IEntityDto<int>
    {
    }

    /// <summary>
    /// Defines a contract for a CRUD application service for a specific DTO type with a custom primary key.
    /// </summary>
    /// <typeparam name="TEntityDto">The DTO type for the entity.</typeparam>
    /// <typeparam name="TPrimaryKey">The type of the primary key.</typeparam>
    public interface ICrudAppService<TEntityDto, TPrimaryKey>
        : ICrudAppService<TEntityDto, TPrimaryKey, PagedAndSortedResultRequestDto>
        where TEntityDto : IEntityDto<TPrimaryKey>
    {
    }

    /// <summary>
    /// Defines a contract for a CRUD application service with custom input for getting all entities.
    /// </summary>
    /// <typeparam name="TEntityDto">The DTO type for the entity.</typeparam>
    /// <typeparam name="TPrimaryKey">The type of the primary key.</typeparam>
    /// <typeparam name="TGetAllInput">The type of the input for getting all entities.</typeparam>
    public interface ICrudAppService<TEntityDto, TPrimaryKey, in TGetAllInput>
        : ICrudAppService<TEntityDto, TPrimaryKey, TGetAllInput, TEntityDto, TEntityDto>
        where TEntityDto : IEntityDto<TPrimaryKey>
    {
    }

    /// <summary>
    /// Defines a contract for a CRUD application service with custom create input type.
    /// </summary>
    /// <typeparam name="TEntityDto">The DTO type for the entity.</typeparam>
    /// <typeparam name="TPrimaryKey">The type of the primary key.</typeparam>
    /// <typeparam name="TGetAllInput">The type of the input for getting all entities.</typeparam>
    /// <typeparam name="TCreateInput">The type of the input for creating an entity.</typeparam>
    public interface ICrudAppService<TEntityDto, TPrimaryKey, in TGetAllInput, in TCreateInput>
        : ICrudAppService<TEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TCreateInput>
        where TEntityDto : IEntityDto<TPrimaryKey>
        where TCreateInput : IEntityDto<TPrimaryKey>
    {
    }

    /// <summary>
    /// Defines a contract for a CRUD application service with custom update input type.
    /// </summary>
    /// <typeparam name="TEntityDto">The DTO type for the entity.</typeparam>
    /// <typeparam name="TPrimaryKey">The type of the primary key.</typeparam>
    /// <typeparam name="TGetAllInput">The type of the input for getting all entities.</typeparam>
    /// <typeparam name="TCreateInput">The type of the input for creating an entity.</typeparam>
    /// <typeparam name="TUpdateInput">The type of the input for updating an entity.</typeparam>
    public interface ICrudAppService<TEntityDto, TPrimaryKey, in TGetAllInput, in TCreateInput, in TUpdateInput>
        : ICrudAppService<TEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput, EntityDto<TPrimaryKey>>
        where TEntityDto : IEntityDto<TPrimaryKey>
        where TUpdateInput : IEntityDto<TPrimaryKey>
    {
    }

    /// <summary>
    /// Defines a contract for a CRUD application service with custom get input type.
    /// </summary>
    /// <typeparam name="TEntityDto">The DTO type for the entity.</typeparam>
    /// <typeparam name="TPrimaryKey">The type of the primary key.</typeparam>
    /// <typeparam name="TGetAllInput">The type of the input for getting all entities.</typeparam>
    /// <typeparam name="TCreateInput">The type of the input for creating an entity.</typeparam>
    /// <typeparam name="TUpdateInput">The type of the input for updating an entity.</typeparam>
    /// <typeparam name="TGetInput">The type of the input for getting a single entity.</typeparam>
    public interface ICrudAppService<TEntityDto, TPrimaryKey, in TGetAllInput, in TCreateInput, in TUpdateInput,
            in TGetInput>
        : ICrudAppService<TEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput, TGetInput,
            EntityDto<TPrimaryKey>>
        where TEntityDto : IEntityDto<TPrimaryKey>
        where TUpdateInput : IEntityDto<TPrimaryKey>
        where TGetInput : IEntityDto<TPrimaryKey>
    {
    }

    /// <summary>
    /// Defines a contract for a CRUD application service with custom delete input type.
    /// </summary>
    /// <typeparam name="TEntityDto">The DTO type for the entity.</typeparam>
    /// <typeparam name="TPrimaryKey">The type of the primary key.</typeparam>
    /// <typeparam name="TGetAllInput">The type of the input for getting all entities.</typeparam>
    /// <typeparam name="TCreateInput">The type of the input for creating an entity.</typeparam>
    /// <typeparam name="TUpdateInput">The type of the input for updating an entity.</typeparam>
    /// <typeparam name="TGetInput">The type of the input for getting a single entity.</typeparam>
    /// <typeparam name="TDeleteInput">The type of the input for deleting an entity.</typeparam>
    public interface ICrudAppService<TEntityDto, TPrimaryKey, in TGetAllInput, in TCreateInput, in TUpdateInput,
            in TGetInput, in TDeleteInput>
        : IApplicationService
        where TEntityDto : IEntityDto<TPrimaryKey>
        where TUpdateInput : IEntityDto<TPrimaryKey>
        where TGetInput : IEntityDto<TPrimaryKey>
        where TDeleteInput : IEntityDto<TPrimaryKey>
    {
        /// <summary>
        /// Gets an entity by input.
        /// </summary>
        /// <param name="input">The input containing the entity ID.</param>
        /// <returns>The entity DTO.</returns>
        TEntityDto Get(TGetInput input);

        /// <summary>
        /// Gets all entities matching the input criteria.
        /// </summary>
        /// <param name="input">The input for filtering, sorting, and paging.</param>
        /// <returns>Paged result of entity DTOs.</returns>
        PagedResultDto<TEntityDto> GetAll(TGetAllInput input);

        /// <summary>
        /// Creates a new entity.
        /// </summary>
        /// <param name="input">The input DTO for creation.</param>
        /// <returns>The created entity DTO.</returns>
        TEntityDto Create(TCreateInput input);

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="input">The input DTO for update.</param>
        /// <returns>The updated entity DTO.</returns>
        TEntityDto Update(TUpdateInput input);

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="input">The input DTO for deletion.</param>
        void Delete(TDeleteInput input);
    }
}
