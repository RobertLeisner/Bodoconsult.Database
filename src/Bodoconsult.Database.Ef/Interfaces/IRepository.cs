// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Linq.Expressions;

namespace Bodoconsult.Database.Ef.Interfaces;

/// <summary>
/// Interface for StSys data repositories
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public interface IRepository<TEntity> where TEntity : class, IEntityRequirements, new()
{
    /// <summary>
    /// Any items in set data set
    /// </summary>
    /// <returns>true if any items</returns>
    bool Any();

    /// <summary>
    /// Any items in set data set
    /// </summary>
    /// <param name="where">condition</param>
    /// <returns>true if any items</returns>
    bool Any(Expression<Func<TEntity, bool>> where);

    /// <summary>
    /// Any items in set data set (asyncron version)
    /// </summary>
    /// <returns>true if any items</returns>
    Task<bool> AnyAsync();

    /// <summary>
    /// Any items in set data set (asyncron version)
    /// </summary>
    /// <param name="where">condition</param>
    /// <returns>true if any items</returns>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> where);

    /// <summary>
    /// Create a new entity. Requires the ID to be 0. If the ID is not 0 the existing entity with this ID is updated
    /// </summary>
    /// <param name="entity">entity to add</param>
    /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
    /// <returns></returns>
    TEntity Add(TEntity entity);

    /// <summary>
    /// Add a list of entities to the store
    /// </summary>
    /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
    /// <param name="entities">list of entities</param>
    void Add(IEnumerable<TEntity> entities);

    /// <summary>
    /// Create or update an entity
    /// </summary>
    /// <param name="entity">entity to add</param>
    /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
    /// <returns></returns>
    TEntity AddOrUpdate(TEntity entity);

    /// <summary>
    /// Create a new entity (asyncron version)
    /// </summary>
    /// <param name="entity">entity to add</param>
    /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
    /// <returns></returns>
    Task<TEntity> AddAsync(TEntity entity);

    /// <summary>
    /// Add a list of entities to the store (asyncron version)
    /// </summary>
    /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
    /// <param name="entities">list of entities</param>
    Task AddAsync(IEnumerable<TEntity> entities);

    /// <summary>
    /// Add a list of entities to the store. Does not check nested entities. Use normally with plain new pocos to speed up mass imports
    /// </summary>
    /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
    /// <param name="entities">list of entities</param>
    void AddBatch(IEnumerable<TEntity> entities);

    /// <summary>
    /// Delete entities 
    /// </summary>
    /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
    /// <param name="where">selection expression</param>
    /// <returns></returns>
    bool Delete(Expression<Func<TEntity, bool>> where);

    /// <summary>
    /// Deletes a entity 
    /// </summary>
    /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
    /// <param name="entity">Entity to delete</param>
    /// <returns>true on success</returns>
    bool Delete(TEntity entity);

    /// <summary>
    /// Delete entities (asyncron version)
    /// </summary>
    /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
    /// <param name="where">selection expression</param>
    /// <returns></returns>
    Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> where);

    /// <summary>
    /// Deletes a entity  (asyncron version)
    /// </summary>
    /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
    /// <param name="entity">Entity to delete</param>
    /// <returns>true on success</returns>
    Task<bool> DeleteAsync(TEntity entity);

    /// <summary>
    /// Drop all entities
    /// </summary>
    /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
    void DropAll();

    /// <summary>
    /// Get an untracked entity using delegate
    /// </summary>
    /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
    /// <param name="where">selection expression</param>
    /// <returns>Requested entity or null if entity not found</returns>
    TEntity GetOne(Expression<Func<TEntity, bool>> where);

    /// <summary>
    /// Get an untracked entity using delegate
    /// </summary>
    /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
    /// <param name="where">selection expression</param>
    /// <param name="loadNestedEntities">Load all nested entities</param>
    /// <returns>Requested entity or null if entity not found</returns>
    TEntity GetOne(Expression<Func<TEntity, bool>> where, bool loadNestedEntities);

    /// <summary>
    /// Get an untracked entity using delegate with selected includes
    /// </summary>
    /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
    /// <param name="where">selection expression</param>
    /// <param name="includePaths">Load all entities from the selected apths. Paths separated by semicolon.</param>
    /// <returns>Requested entity or null if entity not found</returns>
    TEntity GetOneInclude(Expression<Func<TEntity, bool>> where, string includePaths);

    /// <summary>
    /// Get a tracked entity using delegate (
    /// </summary>
    /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
    /// <param name="where">selection expression</param>
    /// <returns>Requested entity or null if entity not found</returns>
    TEntity GetOneTracked(Expression<Func<TEntity, bool>> where);

    /// <summary>
    /// Get a tracked entity using delegate (
    /// </summary>
    /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
    /// <param name="where">selection expression</param>
    /// <param name="loadNestedEntities">Load all nested entities</param>
    /// <returns>Requested entity or null if entity not found</returns>
    TEntity GetOneTracked(Expression<Func<TEntity, bool>> where, bool loadNestedEntities);

    /// <summary>
    /// Get a tracked entity using delegate with selected includes
    /// </summary>
    /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
    /// <param name="where">selection expression</param>
    /// <param name="includePaths">Load all entities from the selected apths. Paths separated by semicolon.</param>
    /// <returns>Requested entity or null if entity not found</returns>
    TEntity GetOneTrackedInclude(Expression<Func<TEntity, bool>> where, string includePaths);

    /// <summary>
    /// Get an entity using delegate (asyncron version)
    /// </summary>
    /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
    /// <param name="where">selection expression</param>
    /// <returns>Requested entity or null if entity not found</returns>
    Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> where);

    /// <summary>
    /// Get an entity using delegate (asyncron version)
    /// </summary>
    /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
    /// <param name="where">selection expression</param>
    /// <param name="loadNestedEntities">Load all nested entities</param>
    /// <returns>Requested entity or null if entity not found</returns>
    Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> where, bool loadNestedEntities);

    /// <summary>
    /// Get all entities as IQueryable. Entities will be loaded untracked.
    /// </summary>
    /// <returns>Queryable list of all entities</returns>
    IQueryable<TEntity> GetAll();

    /// <summary>
    /// Get all entities as IQueryable. Entities will be loaded untracked.
    /// </summary>
    /// <returns>Queryable list of all entities</returns>
    IQueryable<TEntity> GetAll(params Expression<Func<TEntity, object>>[] includeProperties);

    /// <summary>
    /// Gets entities using selection delegate. Entities will be loaded tracked.
    /// </summary>
    /// <param name="where">selection delegate</param>
    /// <returns>List of entities</returns>
    IQueryable<TEntity> GetListTracked(Expression<Func<TEntity, bool>> where);

    /// <summary>
    /// Gets entities using selection delegate. Entities will be loaded tracked.
    /// </summary>
    /// <param name="where">selection delegate</param>
    /// <param name="includeProperties">Related properties to include</param>
    /// <returns>List of entities</returns>
    IQueryable<TEntity> GetListTracked(Expression<Func<TEntity, bool>> where, params Expression<Func<TEntity, object>>[] includeProperties);

    /// <summary>
    /// Get all entities as IQueryable. Entities will be loaded tracked. Entities will be loaded tracked.
    /// </summary>
    /// <returns>Queryable list of all entities</returns>
    IQueryable<TEntity> GetAllTracked();

    /// <summary>
    /// Get all entities as IQueryable. Entities will be loaded tracked. Entities will be loaded tracked.
    /// </summary>
    /// <returns>Queryable list of all entities</returns>
    IQueryable<TEntity> GetAllTracked(params Expression<Func<TEntity, object>>[] includeProperties);

    /// <summary>
    /// Gets entities using selection delegate
    /// </summary>
    /// <param name="where">selection delegate</param>
    /// <returns>List of entities</returns>
    IQueryable<TEntity> GetList(Expression<Func<TEntity, bool>> where);

    /// <summary>
    /// Gets entities using selection delegate
    /// </summary>
    /// <param name="where">selection delegate</param>
    /// <param name="includeProperties">Related properties to include</param>
    /// <returns>List of entities</returns>
    IQueryable<TEntity> GetList(Expression<Func<TEntity, bool>> where, params Expression<Func<TEntity, object>>[] includeProperties);

    /// <summary>
    /// Get number of entities
    /// </summary>
    /// <returns>Number of entities found</returns>
    int Count();

    /// <summary>
    /// Get number of entities
    /// </summary>
    /// <returns>Number of entities found</returns>
    int Count(Expression<Func<TEntity, bool>> where);

    /// <summary>
    /// Get number of entities (asyncron version)
    /// </summary>
    /// <returns>Number of entities found</returns>
    Task<int> CountAsync();

    /// <summary>
    /// Get number of entities (asyncron version)
    /// </summary>
    /// <returns>Number of entities found</returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>> where);

    /// <summary>
    /// Update a entity and all nested enties using the Microsoft style. Prefer usage of <see cref="Update"/> or
    /// <see cref="UpdateTopLevelEntityOnly"/>due to
    /// better performance and more stable behaviour in a disconnected scenario
    /// </summary>
    /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/>
    /// of the current UnitOfWork.</remarks>
    /// <param name="entity">Entity to update</param>
    /// <returns>true on success</returns>
    /// <remarks>Uses EFCore default implementation: if the entity is detached, it will be attached and set to Modified state and all
    ///  nested enties will be set to Modified state too. This behaviour may result in lots of (unncessary) database updates and
    /// as result of them index fragmentation in the database.</remarks>
    TEntity UpdateMs(TEntity entity);

    /// <summary>
    /// Update a entity and all nested enties
    /// </summary>
    /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
    /// <param name="entity">Entity to update</param>
    /// <returns>true on success</returns>
    ///  <remarks>Uses own update implementation: detached entities will be attached and all nested entities in entity graph will be checked for changes.
    /// This behaviour produces much less update statements in the database.</remarks>
    TEntity Update(TEntity entity);

    /// <summary>
    /// Update a entity but no nested entities
    /// </summary>
    /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
    /// <param name="entity">Entity to update</param>
    /// <returns>true on success</returns>
    /// <remarks>Uses own update implementation: updates only the etity itself and no nested enties.</remarks>
    TEntity UpdateTopLevelEntityOnly(TEntity entity);

    /// <summary>
    /// Update a entity (asyncron version)
    /// </summary>
    /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
    /// <param name="entity">Entity to update</param>
    /// <returns>true on success</returns>
    Task<TEntity> UpdateAsync(TEntity entity);

    /// <summary>
    /// Attach a detached entity. Normally not needed!!!
    /// </summary>
    /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
    /// <param name="entity">entity to attach</param>
    /// <returns>returns th attached entity</returns>
    TEntity Attach(TEntity entity);

    /// <summary>
    /// Attach a detached entity if it is not attached. Normally not needed!!!
    /// </summary>
    /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
    /// <param name="entity">entity to attach</param>
    /// <returns>returns the attached entity</returns>
    TEntity AttachIfNot(TEntity entity);

    /// <summary>
    /// Bulk insert data to the database
    /// </summary>
    /// <param name="entities">Insert data in a table by bulkcopying it</param>
    void BulkInsertAll(IEnumerable<TEntity> entities);

    /// <summary>
    /// Async bulk insert data to the database
    /// </summary>
    /// <param name="entities"></param>
    Task BulkInsertAllAsync(IEnumerable<TEntity> entities);

    /// <summary>
    /// Get all include paths for the type TEnity
    /// </summary>
    /// <returns></returns>
    IEnumerable<string> GetIncludePaths();

    /// <summary>
    /// Load all entites for a query result
    /// </summary>
    /// <param name="query">Query to prepare for loading all nested entities for the result</param>
    /// <returns>Query prepare for loading all nested entities for the result</returns>
    IQueryable<TEntity> LoadAllNestedEntities(IQueryable<TEntity> query);
}