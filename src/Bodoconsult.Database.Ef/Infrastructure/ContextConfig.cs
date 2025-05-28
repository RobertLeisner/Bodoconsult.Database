// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.App.Interfaces;

namespace Bodoconsult.Database.Ef.Infrastructure;

/// <summary>
/// Current context config
/// </summary>
public class ContextConfig : IContextConfig
{
    public string ConnectionString { get; set; }
    public bool TurnOffMigrations { get; set; }
    public bool TurnOffConverters { get; set; }
    public int CommandTimeout { get; set; } = 600;
    public bool TurnOffBackup { get; set; }
}