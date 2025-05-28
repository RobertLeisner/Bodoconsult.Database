// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

/* 
 * Copyright (C) 2014 Mehdi El Gueddari
 * http://mehdi.me
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 */

using System.Data;
using Bodoconsult.App.Interfaces;
using Bodoconsult.Database.Ef.Enums;
using Bodoconsult.Database.Ef.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Bodoconsult.Database.Ef.Infrastructure
{
    public class DbContextReadOnlyScope<T> : IDbContextReadOnlyScope<T> where T : DbContext
    {
        private readonly DbContextScope<T> _internalScope;


        public IDbContextCollection<T> DbContexts => _internalScope.DbContexts;

        /// <summary>
        /// Current context config
        /// </summary>
        public IContextConfig ContextConfig { get; }

        public DbContextReadOnlyScope()
            : this(joiningOption: DbContextScopeOption.JoinExisting, isolationLevel: null, dbContextFactory: null)
        { }

        public DbContextReadOnlyScope(IDbContextWithConfigFactory<T> dbContextFactory)
            : this(joiningOption: DbContextScopeOption.JoinExisting, isolationLevel: null, dbContextFactory: dbContextFactory)
        { }

        public DbContextReadOnlyScope(IsolationLevel isolationLevel)
            : this(joiningOption: DbContextScopeOption.ForceCreateNew, isolationLevel: isolationLevel, dbContextFactory: null)
        { }

        public DbContextReadOnlyScope(IsolationLevel isolationLevel, IDbContextWithConfigFactory<T> dbContextFactory)
            : this(joiningOption: DbContextScopeOption.ForceCreateNew, isolationLevel: isolationLevel, dbContextFactory: dbContextFactory)
        { }

        public DbContextReadOnlyScope(DbContextScopeOption joiningOption, IsolationLevel? isolationLevel, IDbContextWithConfigFactory<T> dbContextFactory)
        {
            if (dbContextFactory == null)
            {
                throw new ArgumentNullException(nameof(dbContextFactory));
            }
            ContextConfig = dbContextFactory.AppGlobals.ContextConfig;
            _internalScope = new DbContextScope<T>(joiningOption: joiningOption, readOnly: true, isolationLevel: isolationLevel, dbContextFactory: dbContextFactory);
        }



        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        ~DbContextReadOnlyScope()
        {
            Dispose(false);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }


            try
            {
                _internalScope.Dispose();
            }
            catch //(Exception e)
            {
                // ignored
            }


        }

    }
}