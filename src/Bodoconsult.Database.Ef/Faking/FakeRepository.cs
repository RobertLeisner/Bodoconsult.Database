// Copyright (c) Mycronic. All rights reserved.

using System.Collections;
using System.Diagnostics;
using System.Linq.Expressions;
using Bodoconsult.App.Helpers;
using Bodoconsult.Database.Ef.Interfaces;

namespace Bodoconsult.Database.Ef.Faking
{

    /// <summary>
    /// Fake repository
    /// </summary>
    /// <typeparam name="T">POCO entity type</typeparam>
    public class FakeRepository<T> : IRepository<T> where T : class, IEntityRequirements, new()
    {

        private readonly IList<T> _context;

        private readonly object _sharedLock = new();

        private readonly FakeUnitOfWork _unitOfWork;

        public FakeRepository(IList<T> context, FakeUnitOfWork unitOfWork)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            // Set the ID
            for (var i = 0; i < context.Count; i++)
            {
                var item = context[i];

                item.ID = i + 1;
            }

            _context = context;

            _unitOfWork = unitOfWork;
        }



        /// <summary>
        /// Marks an entity as new
        /// </summary>
        /// <param name="entity">entity to add</param>
        public T Add(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (_context.Any(x => x.ID == entity.ID))
            {
                return Update(entity);
            }

            entity = AddInternal(entity);

            CheckNestedObjects(entity);

            return entity;
        }

        /// <summary>
        /// Marks an entity as new
        /// </summary>
        /// <param name="entity">entity to add</param>
        private T AddInternal(T entity)
        {
            lock (_sharedLock)
            {
                if (entity.ID == 0)
                {
                    entity.ID = _context.Count + 1;
                }
                _context.Add(entity);
            }

            return entity;
        }

        /// <summary>
        /// Create a new entity if the entity is not existing. Otherwise update the entity.
        /// Identifying the entity is done by the ID property
        /// </summary>
        /// <param name="entity">entity to add or update</param>
        /// <remarks>. Does not commit the added or updated entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        /// <returns></returns>
        public T AddOrUpdate(T entity)
        {

            if (entity == null)
            {
                return null;
            }

            if (entity.ID == 0)
            {
                AddInternal(entity);
                return entity;
            }

            var isEntity = _context.Any(x => x.ID == entity.ID);

            return isEntity ? UpdateMs(entity) : AddInternal(entity);
        }

        /// <summary>
        /// Create a new entity (asyncron version)
        /// </summary>
        /// <param name="entity">entity to add</param>
        /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        /// <returns></returns>
        public async Task<T> AddAsync(T entity)
        {
            return await Task.Run(() => Add(entity)).ConfigureAwait(false);
        }

        /// <summary>
        /// Add a list of entities to the store
        /// </summary>
        /// <param name="entities">list of entities</param>
        public void Add(IEnumerable<T> entities)
        {
            lock (_sharedLock)
            {
                var list = entities.ToList();

                foreach (var entity in list)
                {
                    AddInternal(entity);
                }
            }
        }

        /// <summary>
        /// Add a list of entities to the store. Does not check nested entities
        /// </summary>
        /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        /// <param name="entities">list of entities</param>
        public void AddBatch(IEnumerable<T> entities)
        {
            lock (_sharedLock)
            {
                var list = entities.ToList();

                foreach (var entity in list)
                {
                    AddInternal(entity);
                }
            }
        }


        /// <summary>
        /// Add a list of entities to the store (asyncron version)
        /// </summary>
        /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        /// <param name="entities">list of entities</param>
        public async Task AddAsync(IEnumerable<T> entities)
        {
            await Task.Run(() => Add(entities)).ConfigureAwait(false);
        }

        /// <summary>
        /// Any items in set data set
        /// </summary>
        /// <returns>true if any items</returns>
        public bool Any()
        {
            return _context.Any();
        }

        /// <summary>
        /// Any items in set data set
        /// </summary>
        /// <param name="where">condition</param>
        /// <returns>true if any items</returns>
        public bool Any(Expression<Func<T, bool>> where)
        {
            return where == null ? _context.Any() : _context.AsQueryable().Any(where);
        }

        /// <summary>
        /// Any items in set data set (asyncron version)
        /// </summary>
        /// <returns>true if any items</returns>
        public async Task<bool> AnyAsync()
        {
            return await Task.Run(() => _context.Any())
                .ConfigureAwait(false);
        }


        /// <summary>
        /// Any items in set data set (asyncron version)
        /// </summary>
        /// <param name="where">condition</param>
        /// <returns>true if any items</returns>
        public async Task<bool> AnyAsync(Expression<Func<T, bool>> @where)
        {
            return await Task.Run(() => where == null ? _context.Any() : _context.AsQueryable().Any(where))
                .ConfigureAwait(false);
        }


        /// <summary>
        /// Number of items fulfilling a certain condition
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return _context.Count;
        }

        /// <summary>
        /// Number of items fulfilling a certain condition
        /// </summary>
        /// <param name="where">condition</param>
        /// <returns></returns>
        public int Count(Expression<Func<T, bool>> where)
        {
            return @where == null ? _context.Count : _context.AsQueryable().Count(@where);
        }

        /// <summary>
        /// Get number of entities (asyncron version)
        /// </summary>
        /// <returns>Number of entities found</returns>
        public async Task<int> CountAsync()
        {
            return await Task.Run(() => _context.Count)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get number of entities (asyncron version)
        /// </summary>
        /// <returns>Number of entities found</returns>
        public async Task<int> CountAsync(Expression<Func<T, bool>> @where)
        {
            return await Task.Run(() => where == null ?
                    _context.Count :
                    _context.AsQueryable().Count(where))
                .ConfigureAwait(false);
        }


        /// <summary>
        /// Attach an detached entity
        /// </summary>
        /// <param name="entity">entity to attach</param>
        /// <returns>returns th attached entity</returns>
        public T Attach(T entity)
        {
            // Do nothing special
            return entity;
        }

        /// <summary>
        /// Attach an detached entity if it is not attached
        /// </summary>
        /// <param name="entity">entity to attach</param>
        /// <returns>returns the attached entity</returns>
        public T AttachIfNot(T entity)
        {
            // Do nothing special
            return entity;
        }


        /// <summary>
        /// Marks an entity to be removed
        /// </summary>
        /// <param name="entity">entity to remove</param>
        public bool Delete(T entity)
        {

            try
            {
                lock (_sharedLock)
                {
                    _context.Remove(entity);
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error:Delete:" + ex.Message);
                return false;
            }

        }

        /// <summary>
        /// Deletes a entity  (asyncron version)
        /// </summary>
        /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        /// <param name="entity">Entity to delete</param>
        /// <returns>true on success</returns>
        public async Task<bool> DeleteAsync(T entity)
        {
            return await Task.Run(() => Delete(entity)).ConfigureAwait(false);
        }

        /// <summary>
        /// Marks an entity to be removed
        /// </summary>
        /// <param name="where">condition</param>
        public bool Delete(Expression<Func<T, bool>> where)
        {

            try
            {
                lock (_sharedLock)
                {
                    foreach (var entity in _context.AsQueryable().Where(where).ToList())
                    {
                        _context.Remove(entity);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error:Delete:" + ex.Message);
                return false;
            }

        }


        /// <summary>
        /// Delete entities (asyncron version)
        /// </summary>
        /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        /// <param name="where">selection expression</param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(Expression<Func<T, bool>> @where)
        {
            return await Task.Run(() => Delete(where)).ConfigureAwait(false);
        }

        /// <summary>
        /// Drop all entities
        /// </summary>
        public void DropAll()
        {
            _context.Clear();
        }

        /// <summary>
        /// Get an untracked entity using delegate
        /// </summary>
        /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        /// <param name="where">selection expression</param>
        /// <returns>Requested entity or null if entity not found</returns>
        public T GetOne(Expression<Func<T, bool>> where)
        {
            return _context.AsQueryable().FirstOrDefault(where);
        }


        /// <summary>
        /// Get an untracked entity using delegate
        /// </summary>
        /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        /// <param name="where">selection expression</param>
        /// <param name="loadNestedEntities">Load all nested entities</param>
        /// <returns>Requested entity or null if entity not found</returns>
        public T GetOne(Expression<Func<T, bool>> where, bool loadNestedEntities)
        {
            return _context.AsQueryable().FirstOrDefault(where);
        }


        /// <summary>
        /// Get an untracked entity using delegate with selected includes
        /// </summary>
        /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        /// <param name="where">selection expression</param>
        /// <param name="includePaths">Load all entities from the selected apths. Paths separated by semicolon.</param>
        /// <returns>Requested entity or null if entity not found</returns>
        public T GetOneInclude(Expression<Func<T, bool>> @where, string includePaths)
        {
            return _context.AsQueryable().FirstOrDefault(where);
        }


        /// <summary>
        /// Get an tracked entity using delegate
        /// </summary>
        /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        /// <param name="where">selection expression</param>
        /// <returns>Requested entity or null if entity not found</returns>
        public T GetOneTracked(Expression<Func<T, bool>> where)
        {
            return _context.AsQueryable().FirstOrDefault(where);
        }


        /// <summary>
        /// Get an tracked entity using delegate
        /// </summary>
        /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        /// <param name="where">selection expression</param>
        /// <param name="loadNestedEntities">Load all nested entities</param>
        /// <returns>Requested entity or null if entity not found</returns>
        public T GetOneTracked(Expression<Func<T, bool>> where, bool loadNestedEntities)
        {
            return _context.AsQueryable().FirstOrDefault(where);
        }

        /// <summary>
        /// Get an tracked entity using delegate with selected includes
        /// </summary>
        /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        /// <param name="where">selection expression</param>
        /// <param name="includePaths">Load all entities from the selected apths. Paths separated by semicolon.</param>
        /// <returns>Requested entity or null if entity not found</returns>
        public T GetOneTrackedInclude(Expression<Func<T, bool>> @where, string includePaths)
        {
            return _context.AsQueryable().FirstOrDefault(where);
        }

        /// <summary>
        /// Get an entity using delegate (asyncron version)
        /// </summary>
        /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        /// <param name="where">selection expression</param>
        /// <returns>Requested entity or null if entity not found</returns>
        public async Task<T> GetOneAsync(Expression<Func<T, bool>> @where)
        {
            return await Task.Run(() => _context.AsQueryable().FirstOrDefault(where))
                .ConfigureAwait(false);
        }


        /// <summary>
        /// Get an entity using delegate (asyncron version)
        /// </summary>
        /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        /// <param name="where">selection expression</param>
        /// <param name="loadNestedEntities">Load all nested entities</param>
        /// <returns>Requested entity or null if entity not found</returns>
        public async Task<T> GetOneAsync(Expression<Func<T, bool>> @where, bool loadNestedEntities)
        {
            return await Task.Run(() => _context.AsQueryable().FirstOrDefault(where))
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get all entities as IQueryable. Entities will be loaded untracked.
        /// </summary>
        /// <returns>Queryable list of all entities</returns>
        public IQueryable<T> GetAll()
        {
            return _context.AsQueryable();
        }

        /// <summary>
        /// Get all entities as IQueryable
        /// </summary>
        /// <returns>Queryable list of all entities</returns>
        public IQueryable<T> GetAll(params Expression<Func<T, object>>[] includeProperties)
        {
            return _context.AsQueryable();
        }

        /// <summary>
        /// Gets entities using selection delegate. Entities will be loaded tracked.
        /// </summary>
        /// <param name="where">selection delegate</param>
        /// <returns>List of entities</returns>
        public IQueryable<T> GetListTracked(Expression<Func<T, bool>> @where)
        {
            return _context.AsQueryable().Where(where);
        }

        /// <summary>
        /// Get all entities as IQueryable. Entities will be loaded tracked. Entities will be loaded tracked.
        /// </summary>
        /// <returns>Queryable list of all entities</returns>
        public IQueryable<T> GetAllTracked()
        {
            return _context.AsQueryable();
        }

        /// <summary>
        /// Get all entities as IQueryable
        /// </summary>
        /// <returns>Queryable list of all entities</returns>
        public IQueryable<T> GetAllTracked(params Expression<Func<T, object>>[] includeProperties)
        {
            return _context.AsQueryable();
        }

        /// <summary>
        /// Gets entities using selection delegate
        /// </summary>
        /// <param name="where">selection delegate</param>
        /// <returns>List of entities</returns>
        public IQueryable<T> GetList(Expression<Func<T, bool>> @where)
        {
            return _context.AsQueryable().Where(where);
        }


        /// <summary>
        /// Gets entities using selection delegate
        /// </summary>
        /// <param name="where">selection delegate</param>
        /// <param name="includeProperties">Related properties to include</param>
        /// <returns>List of entities</returns>
        public IQueryable<T> GetList(Expression<Func<T, bool>> @where, params Expression<Func<T, object>>[] includeProperties)
        {
            return _context.AsQueryable().Where(where);
        }


        /// <summary>
        /// Gets entities using selection delegate
        /// </summary>
        /// <param name="where">selection delegate</param>
        /// <param name="includeProperties">Related properties to include</param>
        /// <returns>List of entities</returns>
        public IQueryable<T> GetListTracked(Expression<Func<T, bool>> @where, params Expression<Func<T, object>>[] includeProperties)
        {
            return _context.AsQueryable().Where(where);
        }

        /// <summary>
        /// Update a entity (asyncron version)
        /// </summary>
        /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        /// <param name="entity">Entity to update</param>
        /// <returns>true on success</returns>
        public async Task<T> UpdateAsync(T entity)
        {
            return await Task.Run(() => Update(entity)).ConfigureAwait(false);
        }

        /// <summary>
        /// Update an existing object  with the same ID with another object
        /// </summary>
        /// <param name="entity"></param>
        public T UpdateMs(T entity)
        {
            var entry = _context.SingleOrDefault(s => s.ID == entity.ID);

            if (entry == null)
            {
                return Add(entity);
            }

            ObjectHelper.MapProperties(entity, entry);
            CheckNestedObjects(entity);

            return entry;
        }

        /// <summary>
        /// Update a entity and all nested enties
        /// </summary>
        /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        /// <param name="entity">Entity to update</param>
        /// <returns>true on success</returns>
        ///  <remarks>Uses own update implementation: detached entities will be attached and all nested entities in entity graph will be checked for changes.
        /// This behaviour produces much less update statements in the database.</remarks>
        public T Update(T entity)
        {
            return UpdateMs(entity);
        }

        /// <summary>
        /// Update a entity but no nested entities
        /// </summary>
        /// <remarks>. Does not commit the entity to database. Call <see cref="IContextScope.SaveChanges()"/> of the current UnitOfWork.</remarks>
        /// <param name="entity">Entity to update</param>
        /// <returns>true on success</returns>
        /// <remarks>Uses own update implementation: updates only the etity itself and no nested enties.</remarks>
        public T UpdateTopLevelEntityOnly(T entity)
        {
            var entry = _context.SingleOrDefault(s => s.ID == entity.ID);

            if (entry == null)
            {
                return Add(entity);
            }

            ObjectHelper.MapProperties(entity, entry);
            return entry;
        }

        /// <summary>
        /// Bulk insert data to the database
        /// </summary>
        /// <param name="entities"></param>
        public void BulkInsertAll(IEnumerable<T> entities)
        {
            Add(entities);
        }

        /// <summary>
        /// Async bulk insert data to the database
        /// </summary>
        /// <param name="entities"></param>
        public async Task BulkInsertAllAsync(IEnumerable<T> entities)
        {
            await Task.Run(() => { Add(entities); }).ConfigureAwait(false);
        }

        public IEnumerable<string> GetIncludePaths()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Load all entites for a query result
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IQueryable<T> LoadAllNestedEntities(IQueryable<T> query)
        {
            return query;
        }

        private void CheckNestedObjects(object entity)
        {

            var type = entity.GetType();

            var props = type.GetProperties();

            var list = props
                .Where(x => x.PropertyType.GetInterfaces().Any(t => t.IsGenericType
                                                                    && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                .Where(x => x.PropertyType.Name == "ICollection`1");


            //Debug.Print($"--> {type.Name}");

            foreach (var nestedType in list)
            {
                var x = (IEnumerable)nestedType.GetValue(entity, null);
                EnumerateCollection(x, nestedType.PropertyType.GenericTypeArguments[0]);
            }
        }

        private void EnumerateCollection(IEnumerable entities, Type type)
        {
            var method = typeof(FakeUnitOfWork)
                .GetMethod(typeof(IEntityRequirementsGuid).IsAssignableFrom(type)
                    ? "GetRepositoryGuid" :
                    "GetRepository");
            var generic = method.MakeGenericMethod(type);
            var repo = generic.Invoke(_unitOfWork, null);

            var addMethod = repo.GetType().GetMethod("Update");

            foreach (var entity in entities)
            {
                //Debug.Print(entity.ToString());

                addMethod.Invoke(repo, [entity]);

                CheckNestedObjects(entity);


            }
        }
    }
}
