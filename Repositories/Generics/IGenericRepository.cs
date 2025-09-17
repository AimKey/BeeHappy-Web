using System.Linq.Expressions;
using BusinessObjects.Base;

namespace Repositories.Generics
{
    /// <summary>
    /// Defines a generic repository for MongoDB entities that provides common CRUD operations.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of entity stored in the collection. Must inherit from <see cref="MongoEntity"/>.
    /// </typeparam>
    public interface IGenericRepository<TEntity> where TEntity : MongoEntity
    {
        /// <summary>
        /// Retrieves all documents from the collection.
        /// </summary>
        /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A list containing all entities in the collection.</returns>
        Task<List<TEntity>> GetAllAsync(CancellationToken ct = default);

        /// <summary>
        /// Retrieves documents that match the specified filter expression.
        /// </summary>
        /// <param name="filter">
        /// An optional LINQ expression used to filter documents. 
        /// If <c>null</c>, all documents are returned.
        /// </param>
        /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A list of entities matching the filter.</returns>
        Task<List<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>>? filter,
            CancellationToken ct = default);

        /// <summary>
        /// Retrieves a single entity by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity (as stored in <see cref="MongoEntity.Id"/>).</param>
        /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// The entity with the matching identifier, or <c>null</c> if no document is found.
        /// </returns>
        Task<TEntity> GetByIdAsync(string id, CancellationToken ct = default);

        /// <summary>
        /// Inserts a new entity into the collection.
        /// </summary>
        /// <param name="entity">The entity to insert. Cannot be <c>null</c>.</param>
        /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
        Task InsertAsync(TEntity entity, CancellationToken ct = default);

        /// <summary>
        /// Replaces an existing entity or, if <paramref name="upsert"/> is <c>true</c>, 
        /// inserts it if no matching entity is found.
        /// </summary>
        /// <param name="entity">The entity to replace or insert. The <see cref="MongoEntity.Id"/> must be set.</param>
        /// <param name="upsert">
        /// If <c>true</c>, inserts the entity when a document with the same identifier does not exist.
        /// If <c>false</c>, the operation only updates existing documents.
        /// </param>
        /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// <c>true</c> if the replace or upsert operation was acknowledged and modified or inserted a document; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> ReplaceAsync(TEntity entity, bool upsert = false, CancellationToken ct = default);

        /// <summary>
        /// Deletes an entity from the collection by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity to delete.</param>
        /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// <c>true</c> if a document was found and deleted; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> DeleteByIdAsync(string id, CancellationToken ct = default);

        /// <summary>
        /// Deletes the specified entity from the collection.
        /// </summary>
        /// <param name="entity">The entity to delete. Must not be <c>null</c> and must have an <see cref="MongoEntity.Id"/> value.</param>
        /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// <c>true</c> if a matching document was found and deleted; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> DeleteAsync(TEntity entity, CancellationToken ct = default);

        /// <summary>
        /// Counts the number of documents in the collection, optionally filtered by the given expression.
        /// </summary>
        /// <param name="filter">
        /// An optional LINQ expression used to filter documents. 
        /// If <c>null</c>, all documents are counted.
        /// </param>
        /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>The number of documents that match the filter.</returns>
        Task<long> CountAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            CancellationToken ct = default);
    }
}
