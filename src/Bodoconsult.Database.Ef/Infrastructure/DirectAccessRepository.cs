// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.Database.Ef.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Bodoconsult.Database.Ef.Infrastructure
{
    /// <summary>
    /// Base implementation of <see cref="IDirectAccessRepository"/>
    /// for high speed access to performance relevante data
    /// </summary>
    public class BaseDirectAccessRepository<TContext> : IDirectAccessRepository where TContext : DbContext
    {

        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="contextLocator">Current context locator</param>
        public BaseDirectAccessRepository(IAmbientDbContextLocator contextLocator)
        {
            ContextLocator = contextLocator;
        }


        /// <summary>
        /// Current context locator
        /// </summary>
        public IAmbientDbContextLocator ContextLocator { get; }
    }
}