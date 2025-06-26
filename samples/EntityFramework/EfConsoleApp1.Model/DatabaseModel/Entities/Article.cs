// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using System.ComponentModel.DataAnnotations;
using Bodoconsult.Database.Ef.Interfaces;

namespace EfConsoleApp1.Model.DatabaseModel.Entities;

/// <summary>
/// Article: Represents an article
/// </summary>
public class Article : IEntityRequirements
{

    /// <summary>
    /// Internal ID of the user
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Row version to solve concurrency issues
    /// </summary>
    [Timestamp]
    public byte[] RowVersion { get; set; }

    /// <summary>
    /// Articlename
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [StringLength(255)]
    public string ArticleName { get; set; }

}