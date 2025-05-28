Bodoconsult.Database.Ef
==============

# What does the library

Bodoconsult.Database.Ef is a library based on Microsoft Entity Framework. 



# How to use the library

The source code contains NUnit test classes the following source code is extracted from. The samples below show the most helpful use cases for the library.

# Overview

-   Entities
-   Entity configuration
-   Database context
-   Enhance database context for target database type
-   DbContext factory

#   Implementation details

## Entities 

If talking about Entity Framework an entity is a simple class representing a table in the database. Sometimes entities are named plain old code objects (POCO) too.

``` csharp

    /// <summary>
    /// Represents a app setting item
    /// </summary>
    
    public class AppSettings: IEntityRequirements
    {
        public int ID { get; set; }

        /// <summary>
        /// Row version to solve concurrency issues
        /// </summary>
        public byte[] RowVersion { get; set; }

        /// <summary>
        /// The key name of the app setting
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Key { get; set; }

        /// <summary>
        /// Current value of the app setting
        /// </summary>
        public string Value { get; set; }

    }

```

Do not use prefixes for entities. It is not recommend by MS anymore. In the above example name the class AppSettings instead of TAppSettings

Entities used with Entity Framework should contain simple get-set properties only. There should not be implemented any kind of logic in an entity. 
If you want to enhance an entity with logic used extension methods for implementing the logic:

``` csharp

    /// <summary>
    /// Extension method for <see cref="AppSettings"/> entity
    /// </summary>
    public static class AppSettingsExtensions
    {
        /// <summary>
        /// Returns the entity data as a friendly readable string
        /// </summary>
        /// <param name="appSettings">Current entity instance</param>
        /// <returns>Entity data as a friendly readable string</returns>
        public static string ToFormattedString(this AppSettings appSettings)
        {

            return $"{appSettings.Key}: {appSettings.RowVersion}";

        }
    }

```

If logic is implemented in the entity class itself, the logic is loaded to memory per instance created of the entity type. The extension method is loaded only once. Espacially for heavy-usage entities is may reduce the memory pressure of your app.

## Entity configuration

Entity configuration for entity properties may be done in the entity class itself via attributes like [Required(AllowEmptyStrings = false)] in the above sample.

More and more complex configuration can be done in a separate configuration file implementing IEntityTypeConfiguration\<T\>. Use this file to configure table details, database indexes and other more sophisticated settings.

``` csharp

/// <summary>
/// Entity configuration for <see cref="AppSettings"/> entity
/// </summary>
public class AppSettingsConfig : IEntityTypeConfiguration<AppSettings>
{

    public void Configure(EntityTypeBuilder<AppSettings> builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }


        builder.HasKey(x => x.ID);
        builder.ToTable("AppSettings");
        builder.Property(x => x.ID).ValueGeneratedOnAdd();

        builder.Property(e => e.Key)
                .HasMaxLength(255)
                .IsRequired();

        builder.Property(p => p.RowVersion).IsRowVersion().IsConcurrencyToken().IsRequired();

        builder.HasIndex(u => u.Key)
            //.HasName("AppSettingsKeyUnique") // Old
            .HasDatabaseName("AppSettingsKeyUnique") // New
            .IsUnique();
    }
}

```

## Database context

Implement a base DbContext based class:

``` csharp

    public class ExampleDbContext : Microsoft.EntityFrameworkCore.DbContext
    {

        /// <summary>
        /// Default ctor
        /// </summary>
        public ExampleDbContext() : base(GetDefaultOptions())
        { }

        /// <summary>
        /// Ctor for setting up DbContext with a specific connection string
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        public ExampleDbContext(string connectionString) : base(GetDefaultOptions(connectionString))
        { }

        /// <summary>
        /// Ctor for setting up DbContext with non SqLserver database i.e. for in-memory testing
        /// </summary>
        /// <param name="options">DbContext options</param>
        public ExampleDbContext(DbContextOptions options)
            : base(options)
        { }


        /// <summary>
        /// Get default options for the DbContext
        /// </summary>
        /// <param name="connectionString">Connection string to use</param>
        /// <returns>DbContext options</returns>
        protected static DbContextOptions GetDefaultOptions(string connectionString = null)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            var builder = new DbContextOptionsBuilder<ExampleDbContext>();

            builder.UseSqlServer(connectionString, x => x
                .MigrationsAssembly("EntityFrameworkSample")
                .EnableRetryOnFailure()
                .CommandTimeout((int)TimeSpan.FromSeconds(600).TotalSeconds));
            return builder.Options;
        }

        #region Database sets

        public DbSet<AppSettings> AppSettings { get; set; }

        public DbSet<Users> Users { get; set; }


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

        private static void LoadConfig(ModelBuilder modelBuilder)
        {
            // Config model
            modelBuilder.ApplyConfiguration(AppSettingsConfig);
            modelBuilder.ApplyConfiguration(UsersConfig);


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

```

## Enhance database context for target database type

In case your DBContext instance requires implementations specific for a certain type of database, implement a class derived from your base DbContextclass and add or adjust the required features.

``` csharp

/// <summary>
/// Implement SQLServer specific features for the DbContext
/// </summary>
public class SqlServerExampleDbContext : ExampleDbContext
{
    /// <summary>
    /// Default ctor
    /// </summary>
    public SqlServerExampleDbContext() : base(GetDefaultOptions())
    { }

    /// <summary>
    /// Ctor for setting up DbContext with a specific connection string
    /// </summary>
    /// <param name="connectionString">Connection string</param>
    public SqlServerExampleDbContext(string connectionString) : base(GetDefaultOptions(connectionString))
    { }

    /// <summary>
    /// Ctor with context options
    /// </summary>
    /// <param name="options">Current DB options</param>
    public SqlServerExampleDbContext(DbContextOptions options) : base(options)
    { }

    // Add or adjust DB specific logic

}

```

## DbContext factory

``` csharp

public class SqlServerExampleDbContextFactory: IDbContextFactory<SqlServerExampleDbContext>
{
    /// <summary>
    /// Current DB context configuration
    /// </summary>

    public IContextConfig ContextConfig { get;  }

    /// <summary>
    /// Current app logger instance
    /// </summary>
    public IAppLoggerProxy Logger { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="config">Current context configuration</param>
    /// <param name="logger">Current logger to use</param>
    public SqlServerExampleDbContextFactory(IContextConfig config, IAppLoggerProxy logger)
    {
        
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));

        ContextConfig = config ?? throw new ArgumentNullException(nameof(config));
    }

    public SqlServerExampleDbContext CreateDbContext()
    {
        if (string.IsNullOrEmpty(ContextConfig.ConnectionString))
        {
            throw new ArgumentNullException(nameof(ContextConfig.ConnectionString));
        }

        var builder = new DbContextOptionsBuilder<ExampleDbContext>();

        builder
            .UseLoggerFactory(Logger.LoggerFactory)
            .UseSqlServer(ContextConfig.ConnectionString, x => x
                .MigrationsAssembly("EntityFrameworkSample")
                .EnableRetryOnFailure()
                .CommandTimeout((int)TimeSpan.FromSeconds(ContextConfig.CommandTimeout).TotalSeconds));

        var dbContext = new SqlServerExampleDbContext(builder.Options);
        dbContext.Database.SetCommandTimeout(ContextConfig.CommandTimeout);
        dbContext.ChangeTracker.AutoDetectChangesEnabled = true;

        return dbContext;
    }

}

```

# About us

Bodoconsult <http://www.bodoconsult.de> is a Munich based software company from Germany.

Robert Leisner is senior software developer at Bodoconsult. See his profile on <http://www.bodoconsult.de/Curriculum_vitae_Robert_Leisner.pdf>.



