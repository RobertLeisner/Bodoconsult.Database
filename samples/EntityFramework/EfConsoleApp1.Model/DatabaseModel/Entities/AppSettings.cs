// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.ComponentModel.DataAnnotations;
using Bodoconsult.Database.Ef.Interfaces;

namespace EfConsoleApp1.Model.DatabaseModel.Entities
{
    /// <summary>
    /// Represents an app setting item
    /// </summary>

    public class AppSettings : IEntityRequirements
    {
        public int ID { get; set; }

        /// <summary>
        /// Row version to solve concurrency issues
        /// </summary>
        [Timestamp]
        public byte[] RowVersion { get; set; }

        /// <summary>
        /// The key name of the app setting
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(255)]
        public string Key { get; set; }

        /// <summary>
        /// Current value of the app setting
        /// </summary>
        public string Value { get; set; }

    }
}
