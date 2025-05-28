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
    /// A read-only DbContextScope. Refer to the comments for IDbContextScope
    /// for more details.
    /// </summary>
    public interface IDbContextReadOnlyScope<T> : IContextReadOnlyScope where T: DbContext
    {
        /// <summary>
        /// The DbContext instances that this DbContextScope manages.
        /// </summary>
        IDbContextCollection<T> DbContexts { get; }
    }
}