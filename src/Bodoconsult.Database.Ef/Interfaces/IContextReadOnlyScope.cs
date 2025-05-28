// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

/* 
 * Copyright (C) 2014 Mehdi El Gueddari
 * http://mehdi.me
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 */

using Bodoconsult.App.Interfaces;

namespace Bodoconsult.Database.Ef.Interfaces
{
    /// <summary>
    /// A read-only DbContextScope. Refer to the comments for IDbContextScope
    /// for more details.
    /// </summary>
    public interface IContextReadOnlyScope: IDisposable

    {
        /// <summary>
        /// Current context config
        /// </summary>
        IContextConfig ContextConfig { get; }
    }
}