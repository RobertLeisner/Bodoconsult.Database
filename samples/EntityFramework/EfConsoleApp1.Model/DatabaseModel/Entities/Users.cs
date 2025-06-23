// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.Database.Ef.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace EfConsoleApp1.Model.DatabaseModel.Entities;

/// <summary>
/// UserManagement: Represents an app user
/// </summary>
public class Users : IEntityRequirements
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
    public string Username { get; set; }

    /// <summary>
    /// Real name of the person
    /// </summary>
    [StringLength(40)]
    public string Realname { get; set; }

    /// <summary>
    /// Encrypted password
    /// </summary>
    [StringLength(15)]
    public string Password { get; set; }

    /// <summary>
    /// User permissions
    /// </summary>
    [StringLength(10)]
    public string Rights { get; set; }

    /// <summary>
    /// Usertype of the person
    /// </summary>
    [StringLength(30)]
    public string UserType { get; set; }

    /// <summary>
    /// User type entity ID
    /// </summary>
    public int UserTypeId { get; set; }
}