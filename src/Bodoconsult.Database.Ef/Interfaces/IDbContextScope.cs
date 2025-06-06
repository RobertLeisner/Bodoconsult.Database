﻿// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

/* 
 * Copyright (C) 2014 Mehdi El Gueddari
 * http://mehdi.me
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 */

using Microsoft.EntityFrameworkCore;

namespace Bodoconsult.Database.Ef.Interfaces
{
    /// <summary>
    /// Creates and manages the DbContext instances used by this code block. 
    /// 
    /// You typically use a DbContextScope at the business logic service level. Each 
    /// business transaction (i.e. each service method) that uses Entity Framework must
    /// be wrapped in a DbContextScope, ensuring that the same DbContext instances 
    /// are used throughout the business transaction and are committed or rolled 
    /// back atomically.
    /// 
    /// Think of it as TransactionScope but for managing DbContext instances instead 
    /// of database transactions. Just like a TransactionScope, a DbContextScope is 
    /// ambient, can be nested and supports async execution flows.
    /// 
    /// And just like TransactionScope, it does not support parallel execution flows. 
    /// You therefore MUST suppress the ambient DbContextScope before kicking off parallel 
    /// tasks or you will end up with multiple threads attempting to use the same DbContext
    /// instances (use IDbContextScopeFactory.SuppressAmbientContext() for this).
    /// 
    /// You can access the DbContext instances that this scopes manages via either:
    /// - its DbContexts property, or
    /// - an IAmbientDbContextLocator
    /// 
    /// (you would typically use the later in the repository / query layer to allow your repository
    /// or query classes to access the ambient DbContext instances without giving them access to the actual
    /// DbContextScope).
    /// 
    /// </summary>
    public interface IDbContextScope<T> : IContextScope where T : DbContext

    {
        /// <summary>
        /// The DbContext instances that this DbContextScope manages. Don't call SaveChanges() on the DbContext themselves!
        /// Save the scope instead.
        /// </summary>
        IDbContextCollection<T> DbContexts { get; }


    }
}
