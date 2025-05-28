// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

/* 
 * Copyright (C) 2014 Mehdi El Gueddari
 * http://mehdi.me
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 */

using Bodoconsult.Database.Ef.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Bodoconsult.Database.Ef.Infrastructure
{
    public class AmbientDbContextLocator : IAmbientDbContextLocator
    {
        public T GetContext<T>() where T : DbContext
        {
            var ambientDbContextScope = DbContextScope<T>.GetAmbientScope();
            return ambientDbContextScope?.DbContexts.GetContext();
        }
    }
}