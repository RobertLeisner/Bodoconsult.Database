// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq.Expressions;
using Bodoconsult.App.Helpers;
using Bodoconsult.App.Interfaces;
using Bodoconsult.Database.Ef.Helpers;
using Bodoconsult.Database.Ef.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Bodoconsult.Database.Ef.Infrastructure
{
    /// <summary>
    /// Generic repository for all POCO types based on EntityRequirementsGuid
    /// </summary>
    /// <typeparam name="TEntity">POCO type of the repository</typeparam>
    /// <typeparam name="TContext"></typeparam>
    public class BaseRepositoryGuid<TEntity, TContext> : IRepositoryGuid<TEntity> where TEntity : class, IEntityRequirementsGuid, new() where TContext : DbContext
    {

        private IEnumerable<string> _includePaths;

        protected readonly IAmbientDbContextLocator _contextLocator;

        private readonly IAppLoggerProxy _log;

        /// <summary>
        /// default ctor
        /// </summary>
        /// <param name="contextLocator">Current database context locator</param>
        /// <param name="logger">Current logger</param>
        public BaseRepositoryGuid(IAmbientDbContextLocator contextLocator, IAppLoggerProxy logger)
        {
            _contextLocator = contextLocator ?? throw new ArgumentNullException(nameof(contextLocator));
            _log = logger;

        }

        #region Helpers

        /// <summary>
        /// Get table name related to a Poco object
        /// </summary>
        /// <returns></returns>
        protected string GetTableName()
        {
            var db = _contextLocator.GetContext<TContext>();

            var mapping = db.Model.FindEntityType(typeof(TEntity));
            //var schema = mapping.Schema;
            var tableName = mapping?.GetTableName();

            // Return the table name from the storage entity set
            return tableName;
        }

        #endregion

        /// <summary>
        /// Check if there are any entities fulfilling the selection criteria
        /// </summary>
        /// <returns>true if there are any entities</returns>
        public virtual bool Any()
        {
            var db = _contextLocator.GetContext<TContext>();
            return db.Set<TEntity>().Any();

        }

        /// <summary>
        /// Check if there are any entities fulfilling the selection criteria
        /// </summary>
        /// <param name="where">selection criteria or null for all records</param>
        /// <returns>true if there are any entities</returns>
        public virtual bool Any(Expression<Func<TEntity, bool>> where)
        {
            var db = _contextLocator.GetContext<TContext>();
            return where == null ? db.Set<TEntity>().Any() : db.Set<TEntity>().Any(where);

        }

        /// <summary>
        /// Any items in set data set (asyncron version)
        /// </summary>
        /// <returns>true if any items</returns>
        public async Task<bool> AnyAsync()
        {
            var db = _contextLocator.GetContext<TContext>();
            return await db.Set<TEntity>().AnyAsync().ConfigureAwait(false);
        }


        /// <summary>
        /// Any items in set data set (asyncron version)
        /// </summary>
        /// <param name="where">condition</param>
        /// <returns>true if any items</returns>
        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> where)
        {
            var db = _contextLocator.GetContext<TContext>();
            return where == null
                ? await db.Set<TEntity>().AnyAsync().ConfigureAwait(false)
                : await db.Set<TEntity>().AnyAsync(where).ConfigureAwait(false);
        }


        /// <summary>
        /// Create a new entity. Requires the ID to be Guid.Empty. If the ID is not Guid.Empty the existing entity with this ID is updated
        /// </summary>
        /// <param name="entity">entity to add</param>
        /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        /// <returns></returns>
        public virtual TEntity Add(TEntity entity)
        {
            return AddInternal(entity);
        }

        private TEntity AddInternal(TEntity entity)
        {

            // validation
            ValidateEntity(entity);

            var db = _contextLocator.GetContext<TContext>();

            try
            {
                db.ChangeTracker.TrackGraph(entity, e => CheckEntity(entity, e, true));
                //return db.Set<TEntity>().Add(entity).Entity;
                return db.Entry(entity).Entity;
            }
            catch (Exception e)
            {

                Debug.Print(e.Message);

                foreach (var entry in db.ChangeTracker.Entries().ToList())
                {
                    if (entry.Entity.Equals(entity))
                    {
                        entry.State = EntityState.Detached;
                    }
                }

                throw new InvalidOperationException("Repository:Add()");

            }
        }

        /// <summary>
        /// Create a new entity if the entity is not existing. Otherwise update the entity.
        /// Identifying the entity is done by the ID property
        /// </summary>
        /// <param name="entity">entity to add or update</param>
        /// <remarks>. Does not commit the added or updated entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        /// <returns></returns>
        public TEntity AddOrUpdate(TEntity entity)
        {

            if (entity == null)
            {
                return null;
            }

            return entity.Uid == Guid.Empty ? AddInternal(entity) : Update(entity);
        }

        /// <summary>
        /// Create a new entity (asyncron version)
        /// </summary>
        /// <param name="entity">entity to add</param>
        /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        /// <returns></returns>
        public async Task<TEntity> AddAsync(TEntity entity)
        {
            return await Task.Run(() => Add(entity)).ConfigureAwait(false);
        }

        /// <summary>
        /// Add a list of entities to the store
        /// </summary>
        /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        /// <param name="entities">list of entities</param>
        public void Add(IEnumerable<TEntity> entities)
        {
            if (entities == null)
            {
                return;
            }

            foreach (var entity in entities)
            {
                AddInternal(entity);
            }
        }


        /// <summary>
        /// Add a list of entities to the store. Does not check nested entities
        /// </summary>
        /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        /// <param name="entities">list of entities</param>
        public void AddBatch(IEnumerable<TEntity> entities)
        {

            if (entities == null)
            {
                return;
            }

            var db = _contextLocator.GetContext<TContext>();
            db.Set<TEntity>().AddRange(entities);

        }

        /// <summary>
        /// Add a list of entities to the store (asyncron version)
        /// </summary>
        /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        /// <param name="entities">list of entities</param>
        public async Task AddAsync(IEnumerable<TEntity> entities)
        {
            var db = _contextLocator.GetContext<TContext>();
            await Task.Run(() => db.Set<TEntity>().AddRange(entities)).ConfigureAwait(false);
        }

        /// <summary>
        /// Get number of entities
        /// </summary>
        /// <returns>Number of entities found</returns>
        public int Count()
        {
            var db = _contextLocator.GetContext<TContext>();
            return db.Set<TEntity>().Count();
        }

        /// <summary>
        /// Get number of entities
        /// </summary>
        /// <returns>Number of entities found</returns>
        public int Count(Expression<Func<TEntity, bool>> where)
        {
            var db = _contextLocator.GetContext<TContext>();
            return where == null ? db.Set<TEntity>().Count() : db.Set<TEntity>().Count(where);
        }


        /// <summary>
        /// Get number of entities (asyncron version)
        /// </summary>
        /// <returns>Number of entities found</returns>
        public async Task<int> CountAsync()
        {
            var db = _contextLocator.GetContext<TContext>();
            return await db.Set<TEntity>().CountAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Get number of entities (asyncron version)
        /// </summary>
        /// <returns>Number of entities found</returns>
        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> where)
        {
            var db = _contextLocator.GetContext<TContext>();
            return where == null
                ? await db.Set<TEntity>().CountAsync().ConfigureAwait(false)
                : await db.Set<TEntity>().CountAsync(where).ConfigureAwait(false);
        }


        /// <summary>
        /// Update a entity and all nested enties using the Microsoft style. Prefer usage of <see cref="DbLoggerCategory.Update"/> or
        /// <see cref="IRepository{TEntity}.UpdateTopLevelEntityOnly"/>due to
        /// better performance and more stable behaviour in a disconnected scenario
        /// </summary>
        /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/>
        /// of the current UnitOfWork.</remarks>
        /// <param name="entity">Entity to update</param>
        /// <returns>true on success</returns>
        /// <remarks>Uses EFCore default implementation: if the entity is detached, it will be attached and set to Modified state and all
        ///  nested enties will be set to Modified state too. This behaviour may result in lots of (unncessary) database updates and
        /// as result of them index fragmentation in the database.</remarks>
        public virtual TEntity UpdateMs(TEntity entity)
        {

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var db = _contextLocator.GetContext<TContext>();

            // validation
            ValidateEntity(entity);

            try
            {
                return db.Update(entity).Entity;
            }
            catch (DbUpdateConcurrencyException e)
            {
                //Debug.WriteLine(e.GetType().Name);

                foreach (var entry in db.ChangeTracker.Entries().ToList())
                {
                    if (entry.Entity.Equals(entity))
                    {
                        entry.State = EntityState.Unchanged;
                    }
                }


                _log.LogError("ScopedRepository:Update()" + entity.GetType().Name + " " + e.Message);
                throw new InvalidOperationException("ScopedRepository:Update()", e);

            }
        }

        /// <summary>
        /// Update a entity and all nested enties
        /// </summary>
        /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        /// <param name="entity">Entity to update</param>
        /// <returns>true on success</returns>
        ///  <remarks>Uses own update implementation: detached entities will be attached and all nested entities in entity graph will be checked for changes.
        /// This behaviour produces much less update statements in the database.</remarks>
        public TEntity Update(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var db = _contextLocator.GetContext<TContext>();

            // validation
            ValidateEntity(entity);

            try
            {
                db.ChangeTracker.TrackGraph(entity, e => CheckEntity(entity, e, true));

                return db.Entry(entity).Entity;
                //return db.Update(entity).Entity;
            }
            catch (DbUpdateConcurrencyException e)
            {
                //Debug.WriteLine(e.GetType().Name);

                foreach (var entry in db.ChangeTracker.Entries().ToList())
                {
                    if (entry.Entity.Equals(entity))
                    {
                        entry.State = EntityState.Unchanged;
                    }
                }


                _log.LogError($"ScopedRepository:Update(){entity.GetType().Name} {e.Message}");
                throw new InvalidOperationException("ScopedRepository:Update()", e);

            }

        }

        private static void CheckEntity(TEntity entity, EntityEntryGraphNode e, bool recursive)
        {

            var entry = e.Entry;

            //Debug.WriteLine(entry.Entity.GetType().Name);

            if (!recursive && entry.Entity != entity)
            {
                foreach (var property in entry.Properties)
                {
                    property.IsModified = false;
                }

                entry.State = EntityState.Unchanged;
                return;
            }

            if (entry.Entity is IEntityRequirements idEntity)
            {
                var id = idEntity.ID;
                if (id > 0)
                {
                    if (entry.State == EntityState.Detached)
                    {
                        entry.State = EntityState.Unchanged;
                    }
                }
                else
                {
                    entry.State = EntityState.Added;
                    return;
                }
            }
            else
            {
                var uidEntity = ((IEntityRequirementsGuid)entry.Entity).Uid;
                if (uidEntity != Guid.Empty)
                {
                    if (entry.State == EntityState.Detached)
                    {
                        entry.State = EntityState.Unchanged;
                    }
                }
                else
                {
                    entry.State = EntityState.Added;
                    return;
                }
            }

            var values = entry.GetDatabaseValues();

            if (values == null)
            {
                entry.State = EntityState.Added;
                return;
            }

            entry.OriginalValues.SetValues(values);

            //var props = entry.Properties.ToList();

            foreach (var property in entry.Properties)
            {

                var original = property.OriginalValue;
                var current = property.CurrentValue;



                if (property.Metadata.IsConcurrencyToken)
                {
                    // Reset the original value of a rowversion column to raise a concurrency exception if necessary
                    // Otherwise it uses always the latest timestamp and therefore it will always update the
                    // data row in the database. See:
                    // https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/concurrency?view=aspnetcore-3.0#update-the-edit-methods-in-the-departments-controller
                    // https://stackoverflow.com/questions/35242461/ef-core-set-timestamp-before-save-still-uses-the-old-value

                    property.IsModified = true;
                    property.OriginalValue = current;
                    continue;
                }

                //Debug.Print(property.Metadata.Name);

                if (property.Metadata.IsShadowProperty())
                {
                    property.IsModified = false;
                    continue;
                }


                var equal = ObjectHelper.CheckIfValuesAreEqual(original, current);

                if (equal)
                {
                    //Debug.WriteLine($"==> {property.Metadata.Name} :: {original} :: {current} :: Equal");
                    property.IsModified = false;
                    continue;
                }

                //Debug.WriteLine($"==> {property.Metadata.Name} :: {original} :: {current} :: Not equal");
                property.IsModified = true;

            }
        }

        /// <summary>
        /// Update a entity but no nested entities
        /// </summary>
        /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        /// <param name="entity">Entity to update</param>
        /// <returns>true on success</returns>
        /// <remarks>Uses own update implementation: updates only the etity itself and no nested enties.</remarks>
        public TEntity UpdateTopLevelEntityOnly(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var db = _contextLocator.GetContext<TContext>();

            // validation
            ValidateEntity(entity);

            try
            {
                db.ChangeTracker.TrackGraph(entity, e => CheckEntity(entity, e, false));

                return db.Entry(entity).Entity;

            }
            catch (Exception e)
            {
                //Debug.WriteLine(e.GetType().Name);


                foreach (var entry in db.ChangeTracker.Entries().ToList())
                {
                    if (entry.Entity.Equals(entity))
                    {
                        entry.State = EntityState.Unchanged;
                    }
                }


                _log.LogError($"ScopedRepository:Update(){entity.GetType().Name} {e.Message}");
                throw new InvalidOperationException("ScopedRepository:Update()", e);

            }

        }

        /// <summary>
        /// Validate an entity. Throws exception when validation fails.
        /// </summary>
        /// <param name="entity">Entity to validate</param>
        private void ValidateEntity(TEntity entity)
        {
            var validationContext = new ValidationContext(entity);

            var results = new List<ValidationResult>();

            var valid = Validator.TryValidateObject(entity, validationContext, results, true);

            if (valid)
            {
                return;
            }

            var msg = "";

            foreach (var result in results)
            {
                msg += $"{result.ErrorMessage}: {result.MemberNames}||";
            }

            msg = $"ScopedRepository:Add()<{typeof(TEntity).Name}>:ValidationError:{msg}";
            Debug.WriteLine(msg);
            _log.LogError(msg);

            throw new ValidationException(msg);
        }


        /// <summary>
        /// Update a entity (asyncron version)
        /// </summary>
        /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        /// <param name="entity">Entity to update</param>
        /// <returns>true on success</returns>
        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            return await Task.Run(() => Update(entity)).ConfigureAwait(false);
        }


        /// <summary>
        /// Attach an detached entity. Normally not needed!!!
        /// </summary>
        /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        /// <param name="entity">entity to attach</param>
        /// <returns>returns th attached entity</returns>
        public TEntity Attach(TEntity entity)
        {
            try
            {
                var db = _contextLocator.GetContext<TContext>();
                return db.Set<TEntity>().Attach(entity).Entity;
            }
            catch (Exception e)
            {
                var msg = "ScopedRepository:Attach()" + typeof(TEntity).Name + " ";
                _log.LogError(msg + e.Message);
                throw new InvalidOperationException(msg, e);
            }
        }


        /// <summary>
        /// Attach an detached entity if it is not attached. Normally not needed!!!
        /// </summary>
        /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        /// <param name="entity">entity to attach</param>
        /// <returns>returns the attached entity</returns>
        public TEntity AttachIfNot(TEntity entity)
        {
            try
            {
                var db = _contextLocator.GetContext<TContext>();
                return db.Entry(entity).State == EntityState.Detached ? Attach(entity) : entity;
            }
            catch (Exception e)
            {

                _log.LogError($"ScopedRepository:Attach(){typeof(TEntity).Name} {e.Message}");
                throw new InvalidOperationException("ScopedRepository:Attach()", e);

            }
        }



        /// <summary>
        /// Delete entities 
        /// </summary>
        /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        /// <param name="where">selection expression</param>
        /// <returns></returns>
        public bool Delete(Expression<Func<TEntity, bool>> where)
        {
            try
            {
                var db = _contextLocator.GetContext<TContext>();

                foreach (var entity in db.Set<TEntity>().Where(where))
                {
                    db.Set<TEntity>().Remove(entity);
                }

                return true;
            }
            catch (Exception e)
            {
                var msg = $"ScopedRepository:Delete(where){typeof(TEntity).Name} {e.Message}";
                _log.LogError(msg);
                throw new InvalidOperationException(msg, e);

            }
        }

        /// <summary>
        /// Delete entities (asyncron version)
        /// </summary>
        /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        /// <param name="where">selection expression</param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> where)
        {
            return await Task.Run(() => Delete(where)).ConfigureAwait(false);
        }

        public bool Delete(TEntity entity)
        {
            try
            {
                var db = _contextLocator.GetContext<TContext>();
                db.Set<TEntity>().Remove(entity);
                return true;
            }
            catch (Exception e)
            {
                var msg = "ScopedRepository:Delete(T)" + typeof(TEntity).Name + " " + e.Message;
                _log.LogError(msg);
                throw new InvalidOperationException(msg, e);
            }
        }

        /// <summary>
        /// Deletes a entity  (asyncron version)
        /// </summary>
        /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        /// <param name="entity">Entity to delete</param>
        /// <returns>true on success</returns>
        public async Task<bool> DeleteAsync(TEntity entity)
        {
            return await Task.Run(() => Delete(entity)).ConfigureAwait(false);
        }

        /// <summary>
        /// Drop all entities. !!!! Use only for testing purposes !!!!
        /// </summary>
        /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        public void DropAll()
        {

            var db = _contextLocator.GetContext<TContext>();
            db.Set<TEntity>().RemoveRange(db.Set<TEntity>());
        }

        /// <summary>
        /// Get all entities as IQueryable
        /// </summary>
        /// <returns>Queryable list of all entities</returns>
        public IQueryable<TEntity> GetAll(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var db = _contextLocator.GetContext<TContext>();
            var query = db.Set<TEntity>().AsNoTracking().AsQueryable();

            if (includeProperties == null)
            {
                return query;
            }

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query;
        }

        /// <summary>
        /// Gets entities using selection delegate. Entities will be loaded tracked.
        /// </summary>
        /// <param name="where">selection delegate</param>
        /// <returns>List of entities</returns>
        public IQueryable<TEntity> GetListTracked(Expression<Func<TEntity, bool>> @where)
        {
            var db = _contextLocator.GetContext<TContext>();
            var query = db.Set<TEntity>().Where(where);
            return query;
        }

        /// <summary>
        /// Get all entities as IQueryable. Entities will be loaded tracked. Entities will be loaded tracked.
        /// </summary>
        /// <returns>Queryable list of all entities</returns>
        public IQueryable<TEntity> GetAllTracked()
        {
            var db = _contextLocator.GetContext<TContext>();
            var query = db.Set<TEntity>().AsQueryable();
            return query;
        }

        /// <summary>
        /// Get all entities as IQueryable
        /// </summary>
        /// <returns>Queryable list of all entities</returns>
        public IQueryable<TEntity> GetAllTracked(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var db = _contextLocator.GetContext<TContext>();
            var query = db.Set<TEntity>().AsQueryable();

            if (includeProperties == null)
            {
                return query;
            }

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query;
        }

        public IQueryable<TEntity> GetList(Expression<Func<TEntity, bool>> @where)
        {
            var db = _contextLocator.GetContext<TContext>();
            var query = db.Set<TEntity>().AsNoTracking().Where(where);
            return query;
        }


        /// <summary>
        /// Gets entities using selection criteria
        /// </summary>
        /// <param name="where">selection criteria</param>
        /// <param name="includeProperties">Related properties to include</param>
        /// <returns>List of entities</returns>
        public IQueryable<TEntity> GetList(Expression<Func<TEntity, bool>> where, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var db = _contextLocator.GetContext<TContext>();
            var query = db.Set<TEntity>().AsNoTracking().Where(where);

            if (includeProperties == null)
            {
                return query;
            }

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query;
        }


        /// <summary>
        /// Load all entites for a query result
        /// </summary>
        /// <param name="query">Query to prepare for loading all nested entities for the result</param>
        /// <returns>Query prepare for loading all nested entities for the result</returns>
        public IQueryable<TEntity> LoadAllNestedEntities(IQueryable<TEntity> query)
        {
            return query.MultipleInclude(GetIncludePaths());
        }


        /// <summary>
        /// Gets entities using selection criteria
        /// </summary>
        /// <param name="where">selection criteria</param>
        /// <param name="includeProperties">Related properties to include</param>
        /// <returns>List of entities</returns>
        public IQueryable<TEntity> GetListTracked(Expression<Func<TEntity, bool>> where, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var db = _contextLocator.GetContext<TContext>();
            var query = db.Set<TEntity>().Where(where);

            if (includeProperties == null)
            {
                return query;
            }

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }



        /// <summary>
        /// Get all entities as IQueryable
        /// </summary>
        /// <returns>Queryable list of all entities</returns>
        public IQueryable<TEntity> GetAll()
        {
            var db = _contextLocator.GetContext<TContext>();
            return db.Set<TEntity>().AsNoTracking();
        }

        /// <summary>
        /// Get an untracked entity using delegate. 
        /// </summary>
        /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        /// <param name="where">selection expression</param>
        /// <returns>Requested entity or null if entity not found</returns>
        public TEntity GetOne(Expression<Func<TEntity, bool>> where)
        {

            var db = _contextLocator.GetContext<TContext>();

            //if (loadNestedEntities)
            //{
            //    var timeout = db.Database.GetCommandTimeout() ?? 30;
            //    db.Database.SetCommandTimeout(2 * timeout);
            //}

            //Debug.Print(typeof(TEntity).Name);

            return db.Set<TEntity>().AsNoTracking().Where(where).FirstOrDefault();

        }

        /// <summary>
        /// Get an untracked entity using delegate. 
        /// </summary>
        /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        /// <param name="where">selection expression</param>
        /// <param name="loadNestedEntities">Load all nested entities</param>
        /// <returns>Requested entity or null if entity not found</returns>
        public TEntity GetOne(Expression<Func<TEntity, bool>> where, bool loadNestedEntities)
        {

            var db = _contextLocator.GetContext<TContext>();

            //if (loadNestedEntities)
            //{
            //    var timeout = db.Database.GetCommandTimeout() ?? 30;
            //    db.Database.SetCommandTimeout(2 * timeout);
            //}

            //Debug.Print(typeof(TEntity).Name);

            return loadNestedEntities ?
                db.Set<TEntity>().AsNoTracking().Where(where).MultipleInclude(GetIncludePaths()).FirstOrDefault() :
                db.Set<TEntity>().AsNoTracking().Where(where).FirstOrDefault();

        }

        /// <summary>
        /// Get an untracked entity using delegate with selected includes
        /// </summary>
        /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        /// <param name="where">selection expression</param>
        /// <param name="includePaths">Load all entities from the selected apths. Paths separated by semicolon.</param>
        /// <returns>Requested entity or null if entity not found</returns>
        public TEntity GetOneInclude(Expression<Func<TEntity, bool>> @where, string includePaths)
        {
            var db = _contextLocator.GetContext<TContext>();

            if (string.IsNullOrEmpty(includePaths))
            {
                return db.Set<TEntity>().AsNoTracking().Where(where).FirstOrDefault();
            }

            var includes = includePaths.Split(";", StringSplitOptions.RemoveEmptyEntries);

            var q = db.Set<TEntity>().AsNoTracking().Where(where);

            return q.MultipleInclude(includes).FirstOrDefault();
        }

        /// <summary>
        /// Get an tracked entity using delegate.
        /// </summary>
        /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        /// <param name="where">selection expression</param>
        /// <returns>Requested entity or null if entity not found</returns>
        public TEntity GetOneTracked(Expression<Func<TEntity, bool>> where)
        {
            var db = _contextLocator.GetContext<TContext>();
            return db.Set<TEntity>().Where(where).FirstOrDefault();
        }


        /// <summary>
        /// Get an tracked entity using delegate.
        /// </summary>
        /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        /// <param name="where">selection expression</param>
        /// <param name="loadNestedEntities">Load all nested entities</param>
        /// <returns>Requested entity or null if entity not found</returns>
        public TEntity GetOneTracked(Expression<Func<TEntity, bool>> where, bool loadNestedEntities)
        {
            var db = _contextLocator.GetContext<TContext>();
            return loadNestedEntities ?
                db.Set<TEntity>().Where(where).MultipleInclude(GetIncludePaths()).FirstOrDefault() :
                db.Set<TEntity>().Where(where).FirstOrDefault();

        }

        /// <summary>
        /// Get an tracked entity using delegate with selected includes
        /// </summary>
        /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        /// <param name="where">selection expression</param>
        /// <param name="includePaths">Load all entities from the selected apths. Paths separated by semicolon.</param>
        /// <returns>Requested entity or null if entity not found</returns>
        public TEntity GetOneTrackedInclude(Expression<Func<TEntity, bool>> @where, string includePaths)
        {
            var db = _contextLocator.GetContext<TContext>();

            if (string.IsNullOrEmpty(includePaths))
            {
                return db.Set<TEntity>().Where(where).FirstOrDefault();
            }

            var includes = includePaths.Split(";", StringSplitOptions.RemoveEmptyEntries);

            return db.Set<TEntity>().Where(where).MultipleInclude(includes).FirstOrDefault();
        }


        /// <summary>
        /// Get an entity using delegate (asyncron version)
        /// </summary>
        /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        /// <param name="where">selection expression</param>
        /// <returns>Requested entity or null if entity not found</returns>
        public async Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> where)
        {
            var db = _contextLocator.GetContext<TContext>();
            return await Task
                .Run(() => db.Set<TEntity>().Where(where).MultipleInclude(GetIncludePaths()).FirstOrDefault())
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get an entity using delegate (asyncron version)
        /// </summary>
        /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        /// <param name="where">selection expression</param>
        /// <param name="loadNestedEntities">Load all nested entities</param>
        /// <returns>Requested entity or null if entity not found</returns>
        public async Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> where, bool loadNestedEntities)
        {
            var db = _contextLocator.GetContext<TContext>();
            return loadNestedEntities
                ? await Task
                    .Run(() => db.Set<TEntity>().Where(where).MultipleInclude(GetIncludePaths()).FirstOrDefault())
                    .ConfigureAwait(false)
                : await Task.Run(() => db.Set<TEntity>().Where(where).FirstOrDefault()).ConfigureAwait(false);
        }

        /// <summary>
        /// Async bulk insert data to the database
        /// </summary>
        /// <param name="entities"></param>
        public virtual Task BulkInsertAllAsync(IEnumerable<TEntity> entities)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Bulk insert data to the database
        /// </summary>
        /// <param name="entities"></param>
        public virtual void BulkInsertAll(IEnumerable<TEntity> entities)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Get all include paths for the type TEnity
        /// </summary>
        /// <returns>all include paths</returns>
        public IEnumerable<string> GetIncludePaths()
        {
            if (_includePaths != null)
            {
                return _includePaths;
            }
            _includePaths = GetIncludePathsIntern();
            return _includePaths;
        }

        /// <summary>
        /// Get all include paths for the type TEnity
        /// </summary>
        /// <returns>all include paths</returns>
        public IEnumerable<string> GetIncludePathsIntern()
        {
            var db = _contextLocator.GetContext<TContext>();

            var entityType = db.Model.FindEntityType(typeof(TEntity));

            //var baseType = entityType;

            var includedNavigations = new HashSet<INavigation>();
            var stack = new Stack<IEnumerator<INavigation>>();
            while (true)
            {
                var entityNavigations = new List<INavigation>();
                foreach (var navigation in entityType.GetNavigations())
                {
                    if (includedNavigations.Add(navigation))
                    {
                        entityNavigations.Add(navigation);
                    }
                }
                if (entityNavigations.Count == 0)
                {
                    if (stack.Count > 0)
                    {
                        yield return string.Join(".", stack.Reverse().Select(e => e.Current.Name));
                    }
                }
                else
                {
                    //foreach (var navigation in entityNavigations)
                    //{
                    //    var inverseNavigation = navigation.FindInverse();
                    //    if (inverseNavigation != null)
                    //        includedNavigations.Add(inverseNavigation);
                    //}
                    stack.Push(entityNavigations.GetEnumerator());
                }

                while (stack.Count > 0 && !stack.Peek().MoveNext())
                {
                    stack.Pop();
                }

                if (stack.Count == 0)
                {
                    break;
                }

                var x = stack.Peek();

                if (x?.Current == null)
                {
                    break;
                }

                var newType = x.Current.TargetEntityType;

                entityType = newType;

            }

            //Debug.WriteLine(entityType.Name);

        }


        /// <summary>
        /// Get the DbContext directly
        /// </summary>
        /// <returns>Current DbContext of the scope</returns>
        /// <remarks>It is not recommended to use the DbContext directly in business layer.</remarks>
        public DbContext GetDbContext()
        {
            var db = _contextLocator.GetContext<TContext>();
            return db;
        }
    }
}