// Copyright (c) Mycronic. All rights reserved.

using Bodoconsult.Database.Ef.Interfaces;

namespace Bodoconsult.Database.Ef.Faking
{
    /// <summary>
    /// Fake implemenation of <see cref="IDirectAccessRepository"/>
    /// </summary>
    public class FakeDirectAccessRepository : IDirectAccessRepository
    {

        public FakeDirectAccessRepository(IAmbientDbContextLocator contextLocator)
        {
            ContextLocator  = contextLocator;
        }


        /// <summary>
        /// Current context locator
        /// </summary>
        public IAmbientDbContextLocator ContextLocator { get; set; }
    }
}