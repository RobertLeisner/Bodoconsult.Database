// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

namespace Bodoconsult.Database.Ef.Interfaces;

/// <summary>
/// Interface for direct access repositories to perform safe direct SQL transactions between EF calls
/// </summary>
public interface IDirectAccessRepository
{

    /// <summary>
    /// Current context locator
    /// </summary>
    public IAmbientDbContextLocator ContextLocator { get; }

}