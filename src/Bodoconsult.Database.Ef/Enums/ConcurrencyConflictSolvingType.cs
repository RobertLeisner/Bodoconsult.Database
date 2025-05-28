// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.Database.Ef.Enums
{
    /// <summary>
    /// How should concurrency conflict solving happen?
    /// </summary>
    public enum ConcurrencyConflictSolvingType
    {
        /// <summary>
        /// The data in the database will be kept
        /// </summary>
        DatabaseWins,

        /// <summary>
        /// The entity will override the data in the database
        /// </summary>
        EntityWins
    }
}