using System.Linq.Expressions;

namespace UserService.Api.Repositories.Interfaces
{
    /// <summary>
    /// Represents a generic repository for working with entities of a specific type.
    /// </summary>
    /// <typeparam name="TEntity">The type of entities that the repository works with.</typeparam>
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Retrieves a list of entities based on the provided filter, order, and inclusion criteria.
        /// </summary>
        /// <param name="filter">A predicate to filter entities, or null for no filtering.</param>
        /// <param name="orderBy">A function to specify the order of entities, or null for no ordering.</param>
        /// <param name="includeProperties">A comma-separated list of navigation properties to include, or an empty string for no inclusion.</param>
        /// <returns>A list of entities that meet the criteria.</returns>
        Task<List<TEntity>> Get(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = "");

        /// <summary>
        /// Retrieves an entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to retrieve, usually integer.</param>
        /// <returns>The entity with the specified unique identifier, or null if not found.</returns>
        Task<TEntity?> GetByID(object id);

        /// <summary>
        /// Inserts a new entity into the repository.
        /// </summary>
        /// <param name="entity">The entity to be inserted.</param>
        /// <returns>The inserted entity.</returns>
        Task<TEntity> Insert(TEntity entity);

        /// <summary>
        /// Deletes an entity from the repository based on its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to be deleted.</param>
        /// <returns>A task representing the asynchronous delete operation.</returns>
        /// <remarks>
        /// This method performs a hard delete operation, removing the entity from the database permanently.
        /// It is recommended to consider using softDelete method instead.
        /// </remarks>
        Task Delete(object id);

        /// <summary>
        /// Deletes the specified entity from the repository.
        /// </summary>
        /// <param name="entityToDelete">The entity to be deleted.</param>
        /// <returns>A task representing the asynchronous delete operation.</returns>
        /// <remarks>
        /// This method performs a hard delete operation, removing the entity from the database permanently.
        /// It is recommended to consider using softDelete method instead.
        /// </remarks>
        Task Delete(TEntity entityToDelete);

        /// <summary>
        /// Soft Deletes an entity from the repository based on its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to be soft-deleted.</param>
        /// <returns>A task representing the asynchronous soft-delete operation.</returns>
        Task SoftDelete(object id);

        /// <summary>
        /// Soft Deletes the specified entity from the repository.
        /// </summary>
        /// <param name="entityToDelete">The entity to be soft-deleted.</param>
        /// <returns>A task representing the asynchronous soft-delete operation.</returns>
        Task SoftDelete(TEntity entityToDelete);

        /// <summary>
        /// Updates an existing entity in the repository.
        /// </summary>
        /// <param name="entityToUpdate">The entity to be updated.</param>
        /// <returns>The updated entity.</returns>
        Task<TEntity> Update(TEntity entityToUpdate);
    }

}