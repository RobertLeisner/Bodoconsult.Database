// Copyright (c) Mycronic. All rights reserved.

using System.Collections;
using Bodoconsult.App.Interfaces;
using Bodoconsult.Database.Ef.Enums;
using Bodoconsult.Database.Ef.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Bodoconsult.Database.Ef.Faking
{
    /// <summary>
    /// Context scope for faking
    /// </summary>
    public class FakeContextScope<T> : IDbContextScope<T> where T : DbContext
    {

        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="config">Current config</param>
        public FakeContextScope(IContextConfig config)
        {
            ContextConfig = config;
        }

        private readonly bool _first = false;

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        ~FakeContextScope()
        {
            Dispose(false);
        }


        protected virtual void Dispose(bool disposing)
        {

            if (disposing)
            {

                try
                {

                }
                catch //(Exception e)
                {
                    // ignored
                }

            }

        }

        public IContextConfig ContextConfig { get; }

        /// <summary>
        /// Saves the changes in all the DbContext instances that were created within this scope.
        /// This method can only be called once per scope.
        /// </summary>
        public int SaveChanges()
        {
            return _first ? 0 : 99;
        }


        /// <summary>
        /// Saves the changes in all the DbContext instances that were created within this scope.
        /// This method can only be called once per scope.
        /// </summary>
        public int SaveChanges(ConcurrencyConflictSolvingType concurrencyConflictSolvingType)
        {
            return _first ? 0 : 99;

            // do nothing
        }


        /// <summary>
        /// Saves the changes in all the DbContext instances that were created within this scope.
        /// This method can only be called once per scope.
        /// </summary>
        public Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Saves the changes in all the DbContext instances that were created within this scope.
        /// This method can only be called once per scope.
        /// </summary>
        public Task<int> SaveChangesAsync(ConcurrencyConflictSolvingType concurrencyConflictSolvingType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Saves the changes in all the DbContext instances that were created within this scope.
        /// This method can only be called once per scope.
        /// </summary>
        public Task<int> SaveChangesAsync(CancellationToken cancelToken)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Saves the changes in all the DbContext instances that were created within this scope.
        /// This method can only be called once per scope.
        /// </summary>
        public Task<int> SaveChangesAsync(CancellationToken cancelToken,
            ConcurrencyConflictSolvingType concurrencyConflictSolvingType)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Reloads the provided persistent entities from the data store
        /// in the DbContext instances managed by the parent scope. 
        /// 
        /// If there is no parent scope (i.e. if this DbContextScope
        /// if the top-level scope), does nothing.
        /// 
        /// This is useful when you have forced the creation of a new
        /// DbContextScope and want to make sure that the parent scope
        /// (if any) is aware of the entities you've modified in the 
        /// inner scope.
        /// 
        /// (this is a pretty advanced feature that should be used 
        /// with parsimony). 
        /// </summary>
        public void RefreshEntitiesInParentScope(IEnumerable entities)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the current <see cref="DbContext"/> instance
        /// </summary>
        /// <typeparam name="T">Type of database context</typeparam>
        /// <returns>Current <see cref="DbContext"/> instance</returns>
        public DbContext GetContext()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// The DbContext instances that this DbContextScope manages. Don't call SaveChanges() on the DbContext themselves!
        /// Save the scope instead.
        /// </summary>
        public IDbContextCollection<T> DbContexts { get; set; }
    }
}