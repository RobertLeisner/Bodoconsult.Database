// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.Database.Ef.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace EfConsoleApp1.Model.DatabaseModel.Entities;

/// <summary>
/// UserType: Represents an app user type like admin, normal user etc.
/// </summary>
public class UserType : IEntityRequirements
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
    /// Username of the person
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [StringLength(30)]
    public string UserTypeName { get; set; }

}