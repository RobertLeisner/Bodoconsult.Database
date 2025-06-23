// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using EfConsoleApp1.Model.DatabaseModel.Entities;
using EfConsoleApp1.Model.DatabaseModel.EntityConfiguration;
using Microsoft.EntityFrameworkCore;

namespace EfConsoleApp1.Model.DatabaseModel.DbContext;

public class ExampleDbContext : Microsoft.EntityFrameworkCore.DbContext
{

    /// <summary>
    /// Ctor for setting up DbContext with non SqLserver database i.e. for in-memory testing
    /// </summary>
    /// <param name="options">DbContext options</param>
    public ExampleDbContext(DbContextOptions options)
        : base(options)
    { }

    #region Database sets

    public DbSet<AppSettings> AppSettings { get; set; }

    public DbSet<Users> Users { get; set; }

    public DbSet<UserType> UserType { get; set; }


    #endregion

    #region Query sets

    ///// <summary>
    ///// Items for the component tree in the UI (material handling view)
    ///// </summary>
    //public DbQuery<QComponentTreeItem> QComponentTreeItems { get; set; }

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (modelBuilder == null)
        {
            throw new ArgumentNullException(nameof(modelBuilder));
        }

        LoadConfig(modelBuilder);
    }


    private static readonly IEntityTypeConfiguration<AppSettings> AppSettingsConfig = new AppSettingsConfig();
    private static readonly IEntityTypeConfiguration<Users> UsersConfig = new UsersConfig();
    private static readonly IEntityTypeConfiguration<UserType> UserTypeConfig = new UserTypeConfig();

    private static void LoadConfig(ModelBuilder modelBuilder)
    {
        // Config model
        modelBuilder.ApplyConfiguration(AppSettingsConfig);
        modelBuilder.ApplyConfiguration(UsersConfig);
        modelBuilder.ApplyConfiguration(UserTypeConfig);

        //**********************
        // Add relations
        //
        // Build only one-to-many relationships for being compatible with EFCore 2.2
        //**********************

        //// TArticle
        //modelBuilder.Entity<TArticle>()
        //    .HasOne(x => x.ArticleGroup)
        //    .WithMany(x => x.Articles)
        //    .HasForeignKey(s => s.ArticleGroupId);

    }
}