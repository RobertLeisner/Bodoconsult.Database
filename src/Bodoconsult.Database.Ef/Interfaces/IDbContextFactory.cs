// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Bodoconsult.Database.Ef.Interfaces;


public interface IDbContextWithConfigFactory<T> : IDbContextFactory<T> where T : DbContext
{
    /// <summary>
    /// Current app globals with database settings
    /// </summary>
    IAppGlobalsWithDatabase AppGlobals { get; }
}
