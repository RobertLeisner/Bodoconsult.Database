// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

/* 
 * Copyright (C) 2014 Mehdi El Gueddari
 * http://mehdi.me
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 */

using System.Data;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using Bodoconsult.Database.Ef.Enums;
using Bodoconsult.Database.Ef.Helpers;
using Bodoconsult.Database.Ef.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Bodoconsult.Database.Ef.Infrastructure
{
    /// <summary>
    /// As its name suggests, DbContextCollection maintains a collection of DbContext instances.
    /// 
    /// What it does in a nutshell:
    /// - Lazily instantiates DbContext instances when its Get Of TDbContext () method is called
    /// (and optionally starts an explicit database transaction).
    /// - Keeps track of the DbContext instances it created so that it can return the existing
    /// instance when asked for a DbContext of a specific type.
    /// - Takes care of committing / rolling back changes and transactions on all the DbContext
    /// instances it created when its Commit() or Rollback() method is called.
    /// 
    /// </summary>
    public class DbContextCollection<TContext> : IDbContextCollection<TContext> where TContext : DbContext
    {
        private readonly Dictionary<Type, DbContext> _initializedDbContexts = new();
        private readonly Dictionary<DbContext, IDbContextTransaction> _transactions = new();
        private readonly IsolationLevel? _isolationLevel;
        private readonly IDbContextFactory<TContext> _dbContextFactory;
        private bool _disposed;
        private bool _completed;
        private readonly bool _readOnly;

        private readonly bool _autoDetectChanges;

        internal Dictionary<Type, DbContext> InitializedDbContexts => _initializedDbContexts;

        /// <summary>
        /// Default ctor
        /// </summary>
        public DbContextCollection()
        {
            _autoDetectChanges = true;
            _disposed = false;
            _completed = false;

            //_initializedDbContexts = new Dictionary<Type, DbContext>();
            //_transactions = new Dictionary<DbContext, IDbContextTransaction>();

            _readOnly = false;
            _isolationLevel = null;
            _dbContextFactory = null;
        }

        public DbContextCollection(bool readOnly)
        {
            _autoDetectChanges = true;
            _disposed = false;
            _completed = false;

            _initializedDbContexts = new Dictionary<Type, DbContext>();
            _transactions = new Dictionary<DbContext, IDbContextTransaction>();

            _readOnly = readOnly;
            _isolationLevel = null;
            _dbContextFactory = null;
        }

        public DbContextCollection(bool readOnly, IsolationLevel? isolationLevel)
        {
            _autoDetectChanges = true;
            _disposed = false;
            _completed = false;

            _initializedDbContexts = new Dictionary<Type, DbContext>();
            _transactions = new Dictionary<DbContext, IDbContextTransaction>();

            _readOnly = readOnly;
            _isolationLevel = isolationLevel;
            _dbContextFactory = null;
        }

        public DbContextCollection(bool readOnly, IsolationLevel? isolationLevel, IDbContextFactory<TContext> dbContextFactory)
        {
            _autoDetectChanges = true;
            _disposed = false;
            _completed = false;

            _initializedDbContexts = new Dictionary<Type, DbContext>();
            _transactions = new Dictionary<DbContext, IDbContextTransaction>();

            _readOnly = readOnly;
            _isolationLevel = isolationLevel;
            _dbContextFactory = dbContextFactory;
        }

        public DbContextCollection(bool readOnly, IsolationLevel? isolationLevel, IDbContextFactory<TContext> dbContextFactory, bool autoDetectChanges)
        {
            _autoDetectChanges = autoDetectChanges;
            _disposed = false;
            _completed = false;

            _initializedDbContexts = new Dictionary<Type, DbContext>();
            _transactions = new Dictionary<DbContext, IDbContextTransaction>();

            _readOnly = readOnly;
            _isolationLevel = isolationLevel;
            _dbContextFactory = dbContextFactory;
        }

        public TContext GetContext()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("DbContextCollection");
            }

            var requestedType = typeof(TContext);

            if (_initializedDbContexts.ContainsKey(requestedType))
            {
                return _initializedDbContexts[requestedType] as TContext;
            }

            // First time we've been asked for this particular DbContext type.
            // Create one, cache it and start its database transaction if needed.
            var dbContext = _dbContextFactory != null
                ? _dbContextFactory.CreateDbContext()
                : Activator.CreateInstance<TContext>();

            dbContext.ChangeTracker.AutoDetectChangesEnabled = !_readOnly && _autoDetectChanges;

            _initializedDbContexts.Add(requestedType, dbContext);

            if (_readOnly)
            {
                dbContext.ChangeTracker.AutoDetectChangesEnabled = false;
            }

            if (!_isolationLevel.HasValue)
            {
                return _initializedDbContexts[requestedType] as TContext;
            }
            var tran = dbContext.Database.BeginTransaction(_isolationLevel.Value);
            _transactions.Add(dbContext, tran);

            return _initializedDbContexts[requestedType] as TContext;
        }

        public int Commit()
        {
            return Commit(ConcurrencyConflictSolvingType.DatabaseWins);
        }


        public int Commit(ConcurrencyConflictSolvingType concurrencyConflictSolvingType)
        {

            if (_disposed)
            {
                throw new ObjectDisposedException("DbContextCollection");
            }

            if (_completed)
            {
                throw new InvalidOperationException("You can't call Commit() or Rollback() more than once on a DbContextCollection. All the changes in the DbContext instances managed by this collection have already been saved or rollback and all database transactions have been completed and closed. If you wish to make more data changes, create a new DbContextCollection and make your changes there.");
            }

            // Best effort. You'll note that we're not actually implementing an atomic commit 
            // here. It entirely possible that one DbContext instance will be committed successfully
            // and another will fail. Implementing an atomic commit would require us to wrap
            // all of this in a TransactionScope. The problem with TransactionScope is that 
            // the database transaction it creates may be automatically promoted to a 
            // distributed transaction if our DbContext instances happen to be using different 
            // databases. And that would require the DTC service (Distributed Transaction Coordinator)
            // to be enabled on all of our live and dev servers as well as on all of our dev workstations.
            // Otherwise the whole thing would blow up at runtime. 

            // In practice, if our services are implemented following a reasonably DDD approach,
            // a business transaction (i.e. a service method) should only modify entities in a single
            // DbContext. So we should never find ourselves in a situation where two DbContext instances
            // contain uncommitted changes here. We should therefore never be in a situation where the below
            // would result in a partial commit. 

            ExceptionDispatchInfo lastError = null;

            var c = 0;

            foreach (var dbContext in _initializedDbContexts.Values)
            {
                try
                {
                    if (!_readOnly)
                    {
                        if (!dbContext.ChangeTracker.AutoDetectChangesEnabled)
                        {
                            dbContext.ChangeTracker.DetectChanges();
                        }
                        c += dbContext.SaveChanges();
                    }

                    // If we've started an explicit database transaction, time to commit it now.
                    var tran = GetValueOrDefault(_transactions, dbContext);
                    if (tran == null)
                    {
                        continue;
                    }
                    tran.Commit();
                    tran.Dispose();
                }
                catch (DbUpdateConcurrencyException e)
                {

                    // Entity wins
                    if (concurrencyConflictSolvingType == ConcurrencyConflictSolvingType.EntityWins)
                    {

                        // see https://docs.microsoft.com/en-us/ef/core/saving/concurrency
                        foreach (var entry in e.Entries)
                        {

                            //var proposedValues = entry.CurrentValues;
                            var databaseValues = entry.GetDatabaseValues();

                            // Refresh original values to bypass next concurrency check
                            entry.OriginalValues.SetValues(databaseValues);
                        }

                        return Commit(concurrencyConflictSolvingType);
                    }
                    // Database wins

                    foreach (var entry in dbContext.ChangeTracker.Entries().ToList())
                    {
                        entry.State = EntityState.Unchanged;
                    }

                    lastError = ExceptionDispatchInfo.Capture(e);
                }



                catch (Exception e)
                {

                    foreach (var entry in dbContext.ChangeTracker.Entries().ToList())
                    {
                        Debug.Print(JsonHelper.JsonSerializeNice(entry.Entity));
                    }

                    lastError = ExceptionDispatchInfo.Capture(e);
                }

            }

            _transactions.Clear();
            _completed = true;

            lastError?.Throw(); // Re-throw while maintaining the exception's original stack track

            return c;
        }

        public Task<int> CommitAsync()
        {
            return CommitAsync(ConcurrencyConflictSolvingType.DatabaseWins, CancellationToken.None);
        }


        public Task<int> CommitAsync(ConcurrencyConflictSolvingType concurrencyConflictSolvingType)
        {
            return CommitAsync(concurrencyConflictSolvingType, CancellationToken.None);
        }

        public async Task<int> CommitAsync(ConcurrencyConflictSolvingType concurrencyConflictSolvingType, CancellationToken cancelToken)
        {

            //if (cancelToken == null)
            //{
            //    throw new ArgumentNullException(nameof(cancelToken));
            //}

            if (_disposed)
            {
                throw new ObjectDisposedException("DbContextCollection");
            }

            if (_completed)
            {
                throw new InvalidOperationException("You can't call Commit() or Rollback() more than once on a DbContextCollection. All the changes in the DbContext instances managed by this collection have already been saved or rollback and all database transactions have been completed and closed. If you wish to make more data changes, create a new DbContextCollection and make your changes there.");
            }


            // See comments in the sync version of this method for more details.

            ExceptionDispatchInfo lastError = null;

            var c = 0;

            foreach (var dbContext in _initializedDbContexts.Values)
            {
                try
                {
                    if (!_readOnly)
                    {
                        if (!dbContext.ChangeTracker.AutoDetectChangesEnabled)
                        {
                            await Task.Run(() =>
                            {
                                dbContext.ChangeTracker?.DetectChanges();
                            }, cancelToken).ConfigureAwait(false);
                        }


                        c += await dbContext.SaveChangesAsync(cancelToken).ConfigureAwait(false);
                    }

                    // If we've started an explicit database transaction, time to commit it now.
                    var tran = GetValueOrDefault(_transactions, dbContext);
                    if (tran == null)
                    {
                        continue;
                    }
                    tran.Commit();
                    tran.Dispose();
                }
                catch (DbUpdateConcurrencyException e)
                {

                    // Entity wins
                    if (concurrencyConflictSolvingType == ConcurrencyConflictSolvingType.EntityWins)
                    {

                        // see https://docs.microsoft.com/en-us/ef/core/saving/concurrency
                        foreach (var entry in e.Entries)
                        {

                            //var proposedValues = entry.CurrentValues;
                            var databaseValues = entry.GetDatabaseValues();

                            // Refresh original values to bypass next concurrency check
                            entry.OriginalValues.SetValues(databaseValues);
                        }

                        return await CommitAsync(concurrencyConflictSolvingType, cancelToken).ConfigureAwait(false);
                    }


                    // Database wins
                    foreach (var entry in dbContext.ChangeTracker.Entries().ToList())
                    {
                        entry.State = EntityState.Unchanged;
                    }

                    lastError = ExceptionDispatchInfo.Capture(e);
                }


                catch (Exception e)
                {
                    lastError = ExceptionDispatchInfo.Capture(e);
                }

            }

            _transactions.Clear();
            _completed = true;

            lastError?.Throw(); // Re-throw while maintaining the exception's original stack track

            return c;
        }

        private void HandleEx(DbUpdateConcurrencyException dbUpdateConcurrencyException)
        {
            throw new NotImplementedException();
        }

        public void Rollback()
        {

            if (_disposed)
            {
                throw new ObjectDisposedException("DbContextCollection");
            }

            if (_completed)
            {
                throw new InvalidOperationException("You can't call Commit() or Rollback() more than once on a DbContextCollection. All the changes in the DbContext instances managed by this collection have already been saved or rollback and all database transactions have been completed and closed. If you wish to make more data changes, create a new DbContextCollection and make your changes there.");
            }


            ExceptionDispatchInfo lastError = null;

            foreach (var dbContext in _initializedDbContexts.Values)
            {
                // There's no need to explicitly rollback changes in a DbContext as
                // DbContext doesn't save any changes until its SaveChanges() method is called.
                // So "rolling back" for a DbContext simply means not calling its SaveChanges()
                // method. 

                // But if we've started an explicit database transaction, then we must roll it back.
                var tran = GetValueOrDefault(_transactions, dbContext);
                if (tran == null)
                {
                    continue;
                }
                try
                {
                    tran.Rollback();
                    tran.Dispose();
                }

                catch (Exception e)
                {
                    lastError = ExceptionDispatchInfo.Capture(e);
                }

            }

            _transactions.Clear();
            _completed = true;

            lastError?.Throw(); // Re-throw while maintaining the exception's original stack track
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }



            if (_disposed)
            {
                return;
            }

            // Do our best here to dispose as much as we can even if we get errors along the way.
            // Now is not the time to throw. Correctly implemented applications will have called
            // either Commit() or Rollback() first and would have got the error there.

            if (!_completed)
            {
                try
                {
                    if (_readOnly)
                    {
                        Commit();
                    }
                    else
                    {
                        Rollback();
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }

            try
            {

                foreach (var dbContext in _initializedDbContexts.Values)
                {
                    try
                    {
                        dbContext.Dispose();
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                    }
                }

                _initializedDbContexts.Clear();
                _disposed = true;
            }
            catch //(Exception e)
            {
                // ignored
            }


        }

        /// <summary>
        /// Returns the value associated with the specified key or the default 
        /// value for the TValue  type.
        /// </summary>
        private static IDbContextTransaction GetValueOrDefault(Dictionary<DbContext, IDbContextTransaction> dictionary, DbContext key)
        {
            return dictionary.TryGetValue(key, out var value) ? value : null;
        }

        
    }
}