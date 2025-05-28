// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

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
    /// Maintains a list of lazily-created DbContext instances.
    /// </summary>
    public interface IDbContextCollection<TContext> : IDisposable where TContext : DbContext
    {
        /// <summary>
        /// Get or create a DbContext instance of the specified type. 
        /// </summary>
        TContext GetContext();
    }
}