using System.Threading.Tasks;
using Kontecg.Application.Services.Dto;

namespace Kontecg.Application.Services
{
    /// <summary>
    /// Defines the contract for asynchronous CRUD operations with default integer primary key.
    /// This interface provides a simplified version of the async CRUD service for entities
    /// that use integer as their primary key type.
    /// </summary>
    /// <typeparam name="TEntityDto">The type of the entity DTO that represents the domain entity.</typeparam>
    /// <remarks>
    /// This interface is a convenience overload that simplifies the generic type parameters
    /// for common scenarios where entities use integer primary keys and standard DTOs.
    /// It inherits from the more generic IAsyncCrudAppService with explicit type parameters.
    /// </remarks>
    public interface IAsyncCrudAppService<TEntityDto>
        : IAsyncCrudAppService<TEntityDto, int>
        where TEntityDto : IEntityDto<int>
    {
    }

    /// <summary>
    /// Defines the contract for asynchronous CRUD operations with custom primary key type.
    /// This interface provides the basic async CRUD service contract for entities with
    /// configurable primary key types.
    /// </summary>
    /// <typeparam name="TEntityDto">The type of the entity DTO that represents the domain entity.</typeparam>
    /// <typeparam name="TPrimaryKey">The type of the primary key for the entity.</typeparam>
    /// <remarks>
    /// This interface extends the basic CRUD contract to support custom primary key types
    /// while maintaining the standard DTO structure for create, update, and delete operations.
    /// </remarks>
    public interface IAsyncCrudAppService<TEntityDto, TPrimaryKey>
        : IAsyncCrudAppService<TEntityDto, TPrimaryKey, PagedAndSortedResultRequestDto>
        where TEntityDto : IEntityDto<TPrimaryKey>
    {
    }

    /// <summary>
    /// Defines the contract for asynchronous CRUD operations with custom input types for retrieval.
    /// This interface allows customization of the input type used for retrieving entities.
    /// </summary>
    /// <typeparam name="TEntityDto">The type of the entity DTO that represents the domain entity.</typeparam>
    /// <typeparam name="TPrimaryKey">The type of the primary key for the entity.</typeparam>
    /// <typeparam name="TGetAllInput">The type of the input DTO used for retrieving multiple entities.</typeparam>
    /// <remarks>
    /// This interface provides flexibility in defining custom input types for the GetAll operation,
    /// allowing for advanced filtering, sorting, and paging capabilities while maintaining
    /// standard DTOs for other operations.
    /// </remarks>
    public interface IAsyncCrudAppService<TEntityDto, TPrimaryKey, in TGetAllInput>
        : IAsyncCrudAppService<TEntityDto, TPrimaryKey, TGetAllInput, TEntityDto, TEntityDto>
        where TEntityDto : IEntityDto<TPrimaryKey>
    {
    }

    /// <summary>
    /// Defines the contract for asynchronous CRUD operations with custom input types for creation.
    /// This interface allows customization of the input type used for creating entities.
    /// </summary>
    /// <typeparam name="TEntityDto">The type of the entity DTO that represents the domain entity.</typeparam>
    /// <typeparam name="TPrimaryKey">The type of the primary key for the entity.</typeparam>
    /// <typeparam name="TGetAllInput">The type of the input DTO used for retrieving multiple entities.</typeparam>
    /// <typeparam name="TCreateInput">The type of the input DTO used for creating entities.</typeparam>
    /// <remarks>
    /// This interface provides flexibility in defining custom input types for the Create operation,
    /// allowing for specialized creation DTOs that may contain additional validation or
    /// business logic specific to entity creation.
    /// </remarks>
    public interface IAsyncCrudAppService<TEntityDto, TPrimaryKey, in TGetAllInput, in TCreateInput>
        : IAsyncCrudAppService<TEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TCreateInput>
        where TEntityDto : IEntityDto<TPrimaryKey>
        where TCreateInput : IEntityDto<TPrimaryKey>
    {
    }

    /// <summary>
    /// Defines the contract for asynchronous CRUD operations with custom input types for updates.
    /// This interface allows customization of the input type used for updating entities.
    /// </summary>
    /// <typeparam name="TEntityDto">The type of the entity DTO that represents the domain entity.</typeparam>
    /// <typeparam name="TPrimaryKey">The type of the primary key for the entity.</typeparam>
    /// <typeparam name="TGetAllInput">The type of the input DTO used for retrieving multiple entities.</typeparam>
    /// <typeparam name="TCreateInput">The type of the input DTO used for creating entities.</typeparam>
    /// <typeparam name="TUpdateInput">The type of the input DTO used for updating entities.</typeparam>
    /// <remarks>
    /// This interface provides flexibility in defining custom input types for the Update operation,
    /// allowing for specialized update DTOs that may contain different validation rules or
    /// business logic specific to entity updates.
    /// </remarks>
    public interface IAsyncCrudAppService<TEntityDto, TPrimaryKey, in TGetAllInput, in TCreateInput, in TUpdateInput>
        : IAsyncCrudAppService<TEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput, EntityDto<TPrimaryKey>
        >
        where TEntityDto : IEntityDto<TPrimaryKey>
        where TUpdateInput : IEntityDto<TPrimaryKey>
    {
    }

    /// <summary>
    /// Defines the contract for asynchronous CRUD operations with custom input types for retrieval by ID.
    /// This interface allows customization of the input type used for retrieving individual entities.
    /// </summary>
    /// <typeparam name="TEntityDto">The type of the entity DTO that represents the domain entity.</typeparam>
    /// <typeparam name="TPrimaryKey">The type of the primary key for the entity.</typeparam>
    /// <typeparam name="TGetAllInput">The type of the input DTO used for retrieving multiple entities.</typeparam>
    /// <typeparam name="TCreateInput">The type of the input DTO used for creating entities.</typeparam>
    /// <typeparam name="TUpdateInput">The type of the input DTO used for updating entities.</typeparam>
    /// <typeparam name="TGetInput">The type of the input DTO used for retrieving individual entities.</typeparam>
    /// <remarks>
    /// This interface provides flexibility in defining custom input types for the Get operation,
    /// allowing for specialized retrieval DTOs that may contain additional parameters or
    /// context information for entity retrieval.
    /// </remarks>
    public interface IAsyncCrudAppService<TEntityDto, TPrimaryKey, in TGetAllInput, in TCreateInput, in TUpdateInput,
            in TGetInput>
        : IAsyncCrudAppService<TEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput, TGetInput,
            EntityDto<TPrimaryKey>>
        where TEntityDto : IEntityDto<TPrimaryKey>
        where TUpdateInput : IEntityDto<TPrimaryKey>
        where TGetInput : IEntityDto<TPrimaryKey>
    {
    }

    /// <summary>
    /// Defines the complete contract for asynchronous CRUD operations with full customization of input types.
    /// This interface provides the most flexible CRUD service contract allowing customization
    /// of input types for all operations.
    /// </summary>
    /// <typeparam name="TEntityDto">The type of the entity DTO that represents the domain entity.</typeparam>
    /// <typeparam name="TPrimaryKey">The type of the primary key for the entity.</typeparam>
    /// <typeparam name="TGetAllInput">The type of the input DTO used for retrieving multiple entities.</typeparam>
    /// <typeparam name="TCreateInput">The type of the input DTO used for creating entities.</typeparam>
    /// <typeparam name="TUpdateInput">The type of the input DTO used for updating entities.</typeparam>
    /// <typeparam name="TGetInput">The type of the input DTO used for retrieving individual entities.</typeparam>
    /// <typeparam name="TDeleteInput">The type of the input DTO used for deleting entities.</typeparam>
    /// <remarks>
    /// This interface represents the most comprehensive CRUD service contract, providing
    /// complete flexibility in defining custom input types for all operations. It allows
    /// for specialized DTOs that can contain operation-specific validation rules, business
    /// logic, or additional context information. All methods are asynchronous to support
    /// non-blocking operations and better scalability.
    /// </remarks>
    public interface IAsyncCrudAppService<TEntityDto, TPrimaryKey, in TGetAllInput, in TCreateInput, in TUpdateInput,
            in TGetInput, in TDeleteInput>
        : IApplicationService
        where TEntityDto : IEntityDto<TPrimaryKey>
        where TUpdateInput : IEntityDto<TPrimaryKey>
        where TGetInput : IEntityDto<TPrimaryKey>
        where TDeleteInput : IEntityDto<TPrimaryKey>
    {
        /// <summary>
        /// Retrieves a single entity by its primary key asynchronously.
        /// </summary>
        /// <param name="input">The input DTO containing the primary key and any additional parameters for retrieval.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains
        /// the entity DTO if found, or null if the entity does not exist.
        /// </returns>
        /// <remarks>
        /// This method retrieves a single entity based on the provided input parameters.
        /// The input DTO typically contains the primary key but may also include additional
        /// context information or parameters that affect the retrieval process.
        /// </remarks>
        Task<TEntityDto> GetAsync(TGetInput input);

        /// <summary>
        /// Retrieves a paged list of entities based on the specified criteria asynchronously.
        /// </summary>
        /// <param name="input">The input DTO containing filtering, sorting, and paging parameters.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains
        /// a paged result DTO with the list of entities and total count information.
        /// </returns>
        /// <remarks>
        /// This method supports advanced querying capabilities including filtering, sorting,
        /// and paging. The input DTO can contain complex criteria for filtering entities
        /// and parameters for controlling the result set size and ordering.
        /// </remarks>
        Task<PagedResultDto<TEntityDto>> GetAllAsync(TGetAllInput input);

        /// <summary>
        /// Creates a new entity asynchronously.
        /// </summary>
        /// <param name="input">The input DTO containing the data for creating the new entity.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains
        /// the created entity DTO with the generated primary key and any computed values.
        /// </returns>
        /// <remarks>
        /// This method creates a new entity based on the provided input data. The input DTO
        /// may contain validation rules and business logic specific to entity creation.
        /// The returned DTO includes the generated primary key and any computed or default values.
        /// </remarks>
        Task<TEntityDto> CreateAsync(TCreateInput input);

        /// <summary>
        /// Updates an existing entity asynchronously.
        /// </summary>
        /// <param name="input">The input DTO containing the primary key and updated data for the entity.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains
        /// the updated entity DTO with the current values after the update operation.
        /// </returns>
        /// <remarks>
        /// This method updates an existing entity based on the provided input data. The input DTO
        /// must contain the primary key to identify the entity to update and may contain
        /// validation rules and business logic specific to entity updates.
        /// </remarks>
        Task<TEntityDto> UpdateAsync(TUpdateInput input);

        /// <summary>
        /// Deletes an entity asynchronously.
        /// </summary>
        /// <param name="input">The input DTO containing the primary key and any additional parameters for deletion.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task completes when
        /// the entity has been successfully deleted.
        /// </returns>
        /// <remarks>
        /// This method deletes an entity based on the provided input parameters. The input DTO
        /// typically contains the primary key but may also include additional context information
        /// or parameters that affect the deletion process, such as soft delete options.
        /// </remarks>
        Task DeleteAsync(TDeleteInput input);
    }
}
