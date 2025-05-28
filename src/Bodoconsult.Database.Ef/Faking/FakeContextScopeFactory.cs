// Copyright (c) Mycronic. All rights reserved.

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

namespace Bodoconsult.Database.Ef.Faking
{

    /// <summary>
    /// Mock scope factory for testing
    /// </summary>
    public class FakeContextScopeFactory : IContextScopeFactory<FakeDbContext>
    {

        /// <summary>
        /// Default ctor
        /// </summary>
        public FakeContextScopeFactory(IAppGlobalsWithDatabase appGlobals)
        {
            AppGlobals = appGlobals;
        }

        /// <summary>
        /// Current app globals with database settings
        /// </summary>
        public IAppGlobalsWithDatabase AppGlobals { get; }

        /// <summary>
        /// Creates a new DbContextScope.
        /// 
        /// By default, the new scope will join the existing ambient scope. This
        /// is what you want in most cases. This ensures that the same DbContext instances
        /// are used by all services methods called within the scope of a business transaction.
        /// 
        /// Set 'joiningOption' to 'ForceCreateNew' if you want to ignore the ambient scope
        /// and force the creation of new DbContext instances within that scope. Using 'ForceCreateNew'
        /// is an advanced feature that should be used with great care and only if you fully understand the
        /// implications of doing this.
        /// </summary>
        public IContextScope Create()
        {
            return new FakeContextScope<FakeDbContext>(AppGlobals.ContextConfig);
        }


        /// <summary>
        /// Creates a new DbContextScope.
        /// 
        /// By default, the new scope will join the existing ambient scope. This
        /// is what you want in most cases. This ensures that the same DbContext instances
        /// are used by all services methods called within the scope of a business transaction.
        /// 
        /// Set 'joiningOption' to 'ForceCreateNew' if you want to ignore the ambient scope
        /// and force the creation of new DbContext instances within that scope. Using 'ForceCreateNew'
        /// is an advanced feature that should be used with great care and only if you fully understand the
        /// implications of doing this.
        /// </summary>
        /// <param name="joiningOption">Scope joining option</param>
        /// <param name="autoDetectChanges">Enable AutoDtectChanges for the scope</param>
        public IContextScope Create(DbContextScopeOption joiningOption, bool autoDetectChanges)
        {
            return new FakeContextScope<FakeDbContext>(AppGlobals.ContextConfig);
        }


        /// <summary>
        /// Creates a new DbContextScope for read-only queries.
        /// 
        /// By default, the new scope will join the existing ambient scope. This
        /// is what you want in most cases. This ensures that the same DbContext instances
        /// are used by all services methods called within the scope of a business transaction.
        /// 
        /// Set 'joiningOption' to 'ForceCreateNew' if you want to ignore the ambient scope
        /// and force the creation of new DbContext instances within that scope. Using 'ForceCreateNew'
        /// is an advanced feature that should be used with great care and only if you fully understand the
        /// implications of doing this.
        /// </summary>
        public IContextReadOnlyScope CreateReadOnly()
        {
            return new FakeContextReadOnlyScope<FakeDbContext>(AppGlobals.ContextConfig);
        }

        /// <summary>
        /// Creates a new DbContextScope for read-only queries.
        /// 
        /// By default, the new scope will join the existing ambient scope. This
        /// is what you want in most cases. This ensures that the same DbContext instances
        /// are used by all services methods called within the scope of a business transaction.
        /// 
        /// Set 'joiningOption' to 'ForceCreateNew' if you want to ignore the ambient scope
        /// and force the creation of new DbContext instances within that scope. Using 'ForceCreateNew'
        /// is an advanced feature that should be used with great care and only if you fully understand the
        /// implications of doing this.
        /// </summary>
        public IContextReadOnlyScope CreateReadOnly(DbContextScopeOption joiningOption)
        {
            return new FakeContextReadOnlyScope<FakeDbContext>(AppGlobals.ContextConfig);
        }

        /// <summary>
        /// Forces the creation of a new ambient DbContextScope (i.e. does not
        /// join the ambient scope if there is one) and wraps all DbContext instances
        /// created within that scope in an explicit database transaction with 
        /// the provided isolation level. 
        /// 
        /// WARNING: the database transaction will remain open for the whole 
        /// duration of the scope! So keep the scope as short-lived as possible.
        /// Don't make any remote API calls or perform any long running computation 
        /// within that scope.
        /// 
        /// This is an advanced feature that you should use very carefully
        /// and only if you fully understand the implications of doing this.
        /// </summary>
        /// <param name="isolationLevel">Transaction isolation level</param>
        /// <param name="autoDetectChanges">Enable AutoDtectChanges for the scope</param>
        public IContextScope CreateWithTransaction(IsolationLevel isolationLevel)
        {
            return new FakeContextScope<FakeDbContext>(AppGlobals.ContextConfig);
        }

        /// <summary>
        /// Forces the creation of a new ambient DbContextScope (i.e. does not
        /// join the ambient scope if there is one) and wraps all DbContext instances
        /// created within that scope in an explicit database transaction with 
        /// the provided isolation level. 
        /// 
        /// WARNING: the database transaction will remain open for the whole 
        /// duration of the scope! So keep the scope as short-lived as possible.
        /// Don't make any remote API calls or perform any long running computation 
        /// within that scope.
        /// 
        /// This is an advanced feature that you should use very carefully
        /// and only if you fully understand the implications of doing this.
        /// </summary>
        /// <param name="isolationLevel">Transaction isolation level</param>
        /// <param name="autoDetectChanges">Enable AutoDtectChanges for the scope</param>
        public IContextScope CreateWithTransaction(IsolationLevel isolationLevel, bool autoDetectChanges)
        {
            return new FakeContextScope<FakeDbContext>(AppGlobals.ContextConfig);
        }

        /// <summary>
        /// Forces the creation of a new ambient read-only DbContextScope (i.e. does not
        /// join the ambient scope if there is one) and wraps all DbContext instances
        /// created within that scope in an explicit database transaction with 
        /// the provided isolation level. 
        /// 
        /// WARNING: the database transaction will remain open for the whole 
        /// duration of the scope! So keep the scope as short-lived as possible.
        /// Don't make any remote API calls or perform any long running computation 
        /// within that scope.
        /// 
        /// This is an advanced feature that you should use very carefully
        /// and only if you fully understand the implications of doing this.
        /// </summary>
        public IContextReadOnlyScope CreateReadOnlyWithTransaction()
        {
            return new FakeContextReadOnlyScope<FakeDbContext>(AppGlobals.ContextConfig);
        }

        /// <summary>
        /// Forces the creation of a new ambient read-only DbContextScope (i.e. does not
        /// join the ambient scope if there is one) and wraps all DbContext instances
        /// created within that scope in an explicit database transaction with 
        /// the provided isolation level. 
        /// 
        /// WARNING: the database transaction will remain open for the whole 
        /// duration of the scope! So keep the scope as short-lived as possible.
        /// Don't make any remote API calls or perform any long running computation 
        /// within that scope.
        /// 
        /// This is an advanced feature that you should use very carefully
        /// and only if you fully understand the implications of doing this.
        /// </summary>
        public IContextReadOnlyScope CreateReadOnlyWithTransaction(IsolationLevel isolationLevel)
        {
            return new FakeContextReadOnlyScope<FakeDbContext>(AppGlobals.ContextConfig);
        }

        /// <summary>
        /// Temporarily suppresses the ambient DbContextScope. 
        /// 
        /// Always use this if you need to  kick off parallel tasks within a DbContextScope. 
        /// This will prevent the parallel tasks from using the current ambient scope. If you
        /// were to kick off parallel tasks within a DbContextScope without suppressing the ambient
        /// context first, all the parallel tasks would end up using the same ambient DbContextScope, which 
        /// would result in multiple threads accesssing the same DbContext instances at the same 
        /// time.
        /// </summary>
        public IDisposable SuppressAmbientContext()
        {
            throw new NotImplementedException("SuppressAmbientContext not implemented");
        }
    }
}