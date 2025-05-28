Bodoconsult.Database.Ef
==============

# What does the library

Bodoconsult.Database.Ef is a library based on Microsoft Entity Framework. 



# How to use the library

The source code contains NUnit test classes the following source code is extracted from. The samples below show the most helpful use cases for the library.

# Overview

>   [Entities](#entities)

>   [Entity configuration](#entity-configuration)

>   [Database context](#database-context)

>   [Enhance database context for target database type](#enhance-database-context-for-target-database-type)

>   [Create a DbContext factory](#create-a-dbcontext-factory)

>   [Create a unit of work based on SqlServerUnitOfWork](#create-a-unit-of-work-based-on-sqlserverunitofwork)

>   [Handling evolving database schemas: Migrations](#migrations)

>   [Running schema and data migrations: IMigrationController](#running-schema-and-data-migrations-imigrationcontroller)

>   [Running all data migrations: IModelDataConvertersHandler](#running-all-data-migrations-imodeldataconvertershandler)

>   [Running a data migration for one entity type: IModelDataConverter](#running-a-data-migration-for-one-entity-type-imodeldataconverter)


# Entities 

If talking about Entity Framework an entity is a simple class representing a table in the database. Sometimes entities are named plain old code objects (POCO) too.

``` csharp

/// <summary>
/// Represents an app setting item
/// </summary>

public class AppSettings : IEntityRequirements
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
    [StringLength(255)]
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

# Entity configuration

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

# Database context

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

# Enhance database context for target database type

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

# Create a design time DbContext factory: IDesignTimeDbContextFactory<SqlServerExampleDbContext>

The design time factory is used by the EFCore tools. Locate this class in the same folder in the StartProject as defined in VS.

Before running EFCore tools (see below) write unit tests for design time DbContext factory. Otherwise you may get not very helpful error messages shown at Package-Manager-Console (PMC)

``` csharp
/SqlServerExampleDbContext IDesignTimeDbContextFactory<SqlServerExampleDbContext>.CreateDbContext(string[] args)
{
    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", false)
        .Build();


    var loggerfactory = new FakeLoggerFactory();

    var conn = configuration.GetConnectionString("DefaultConnection");

    if (string.IsNullOrEmpty(conn))
    {
        throw new ArgumentException($"{nameof(conn)} may not be null");
    }

    var builder = new DbContextOptionsBuilder<SqlServerExampleDbContext>();

    builder
        .UseLoggerFactory(loggerfactory)
        .UseSqlServer(conn, x => x
            .MigrationsAssembly(typeof(SqlServerExampleDbContext).Assembly.FullName)
            .EnableRetryOnFailure()
            .CommandTimeout((int)TimeSpan.FromSeconds(60).TotalSeconds));

    var dbContext = new SqlServerExampleDbContext(builder.Options);
    dbContext.Database.SetCommandTimeout(60);
    dbContext.ChangeTracker.AutoDetectChangesEnabled = true;

    return dbContext;
}
```

# Create a production or testing DbContext factory: IDbContextWithConfigFactory<SqlServerExampleDbContext>

Locate this class in the same folder as the DBContext.

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
            throw new ArgumentException(nameof(ContextConfig.ConnectionString));
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

# Create a unit of work based on SqlServerUnitOfWork

``` csharp
    /// <summary>
    /// Current unit of work for ExampleDb running on SqlServer
    /// </summary>
    public class SqlServerExampleDbUnitOfWork: SqlServerUnitOfWork<SqlServerExampleDbContext>
    {
        public SqlServerExampleDbUnitOfWork(IContextScopeFactory<SqlServerExampleDbContext> dbContextScopeFactory, IAppLoggerProxy logger, IAmbientDbContextLocator ambientDbContextLocator, IBackupEngine backupEngine, IMigrationController migrationController) : base(dbContextScopeFactory, logger, ambientDbContextLocator, backupEngine, migrationController)
        {
        }

        // Add additional functionality for your unit of work here if needed
    }
```


``` csharp

```

# Choose or implement a migration controller: IMigrationController

SqlServerMigrationController is a default migration controller for SqlServer based databases. It takes a backup of existing databases, runs the EFCore schema migrations and after that the data migration defined in your IModelDataConvertersHandlerFactory implementation.




# Handling evolving database schemas: Migrations

If talking about EFCore we should differ to kinds of migrations:

-   EFCore migrations (migrations): create a new database or bring an existing one to the current database schema version. Migrations change the database structure.

-   Data migrations (converters): seed a new database or convert existing data to the new database schema version,

## Overview

Migrations define how to reach the defined database (DB) schema in the DbContext instance with database tools. A migration is bound to a certain database schema version. 

A migration 20250401171019_V1_00 consists of two file 20250401171019_V1_00.cs and 20250401171019_V1_00.Designer.cs. The name of a migration always start with a timestamp the migration was created and an additional name (V1_00).

All migrations applied to a database already are stored in a table called _EFMigrationsHistory. The EF Core migration process checks this table before running existing migrations. If no entries are found all migrations are executed sorted by timestamp ascending. If there are entries in the table only the missing migrations are executed sorted by timestamp ascending.


The [20250401171019_V1_00.cs](../../samples/EntityFramework/EfConsoleApp1.Model/DatabaseModel/Migrations/20250401171019_V1_00.cs) file contains the migration commands to come to the next schema version of the DB schema. This file contains a method Up() for upward migration and Down() for downward migration. Downward migration have been evolved as tricky in real world scenarios with existing data. Bringing already migrated data to the old state may be demanding! Our recommendation is: do NOT use it at least in more complex database scenarios.

The [20250401171019_V1_00.Designer.cs](../../samples/EntityFramework/EfConsoleApp1.Model/DatabaseModel/Migrations/20250401171019_V1_00.Designer.cs) is containing a definition of the current schema as metadata for usage by EF Core.

There is another file important for the migrations: the model snapshot. [StSysDbEntitiesSqlServerModelSnapshot](../../samples/EntityFramework/EfConsoleApp1.Model/DatabaseModel/Migrations/StSysDbEntitiesSqlServerModelSnapshot.cs)  contains a snapshot after the last existing migration has been running. The content in the method BuildModel() is the same as in the method BuildTargetModel in the Designer file of the last migration. If you use EFCore tools the model snapshot is update after each add of a new migration. It is possible and maybe the only way of working in complex scenarios to edit this file manually.

## Entity Framework Core tools (EFCore tools)

The proposed way by MS is to let the migrations be created by the integrated EFCore tools. 

In more complex scenarios in real world i.e. coming from existing databases it is sometimes much easier to create or adjust the migration files manually.

To test migrations from VS use the EFCore tools from Package Manager Console window. Select the project with the migrations in. The connection string for the test is taken from appsettings.json.

Run the following command to create an empty database with the last available database schema (meaning running all migrations)

```
Update-Database -Context SqlServerExampleDbContext 
```

Run the following command to create an empty database with the request database schema (meaning running only migrations up to 20250401171019_V1_00 included)

```
Update-Database 20250401171019_V1_00 -Context SqlServerExampleDbContext
```

See https://learn.microsoft.com/en-us/ef/core/cli/powershell for more details.


## Existing databases

If a database is already existing and filled with production data migrations may get difficile. Think about a scenario the existing database has a database schema no fitting to your new required database schema.

In such a case you have first to update the existing database schema to the new database schema and then bring the data in the tables to the new schema. 

The experience has shown in bigger DB project based on EFCore 3.1 with three main database schemas that including data migration into EFCore migrations is not a good idea. Debugging is complicated and implementation difficult to make it unit testable. So the better idea is to perform the migrations as intended by MS and after that run data migrations separately. In the following you will see how we solved the task bringing databases from schema version 1 to target schema version 3. Version 1 was weakly normalized. So compared to the target schema version 3 there were a lot of tables and fields missing. On the other side some fields from schema version 1 could be removed after normalization. Some old tables were replaced by new tables too. So version 2 is an intermediate state adding the new tables and fields as required. Version 3 then cleans the database schema to the finally required state by removing unused tables and unused fields.

The first approach was to that all in one solution. Theoretically this should be possible. In reality you need to define three sets of entities, entity configs and DBContexts for each of the schema version. This makes configuration for example of the entites more demanding as entity XYZ_V1 should be mapped to table XYZ and XYZ_V2 and XYZ_V3 too. Yes, you can do that. We failed with this approach due to the huge complexity of this approach.

Spending a lot of time with experiments we decided to handle each schema version in a separate solution with a separate busienss logic, data model and UI. This kept at least the complexity added by using EFCore at an acceptable level. In each solution you have the model related to the current schema version included. The migrations from version 1 were then taken to version 2 and a new migration added to reach version 2. In version 3 project the model was as required in version 3 and the migrations from version 2 were added completely and a new migration was added to reach version 3.

In each solution for the three schema versions always the schema migrations run first and then follow the data migrations.

In such a real world project the EFCore tools proofed to be less helpful. Migrations work was done mainly manually.

As long as you only add new entities or new properties to your entities migrations are easy. But if you need to change the datatype of an existing property to a new type it gets much more tricky. 

## Example for EfCore migrations followed by a data migration

The example shows how to migrate an existing V1_00 database to the final V2_00 schema version bringing the database in a better normalized state. In V1_00 the user type is a string property in table Users. In V2_00 will be added a table UserType and the field Users.UserType will be replace by Users.UserTypeId.

The repo contains a EfConsoleApp1 sample app. In the EfConsoleApp1.Model project there are 2 migrations defined. The first of them - 20250401171019_V1_00- (Migration1) is creating table AppSettings and Users.

The second migration 20250522151025_V2_00 (Migration2) adds RowVersion fields to both tables. And it adds the new table UserType.


## Create model data converters: BaseModelDataConverter

Create converters for data migration for existing database by creating classes inheriting from BaseModelDataConverter. Normally it is a good idea to have a separate converter per table.

Here a sample implementation using EfCore for AppSettings entity:

``` csharp
    /// <summary>
    /// This converter checks at every app start if all expected settings (LastUpdate and Company) are available. If not they are created with default values
    /// </summary>
    public class AppSettingsConverterEf: BaseModelDataConverter
    {
        private readonly IRepository<AppSettings> _repo;

        public AppSettingsConverterEf(IUnitOfWork unitOfWork, IAppLoggerProxy appLogger) : base(unitOfWork, appLogger)
        {
            RequiredAppVersion = new Version(1, 0, 0);
            UnitOfWork = unitOfWork;
            _repo = UnitOfWork.GetRepository<AppSettings>();
        }

        /// <summary>
        /// Check if the premises are fulfilled to run the converter
        /// </summary>
        public override bool CheckPremisesToRunConverter()
        {
            // Run it always
            return true;
        }


        /// <summary>
        /// Run the converter
        /// </summary>
        public override void Run()
        {
            // Check setting LastUpdate
            CheckLastUpdateSetting();

            // Check setting Company
            CheckCompanySetting();
        }

        public void CheckLastUpdateSetting()
        {
            using (var scope = UnitOfWork.GetContextScope())
            {
                if (_repo.Any(x => x.Key == "LastUpdate"))
                {
                    return;
                }

                var setting = new AppSettings
                {
                    Key = "LastUpdate"
                };

                _repo.Add(setting);

                scope.SaveChanges();
            }
        }

        public void CheckCompanySetting()
        {
            using (var scope = UnitOfWork.GetContextScope())
            {
                if (_repo.Any(x => x.Key == "Company"))
                {
                    return;
                }

                var setting = new AppSettings
                {
                    Key = "Company",
                    Value = "TestCompany Ltd"
                };

                _repo.Add(setting);

                scope.SaveChanges();
            }
        }
    }
```

If you have performance issues you can run data conversions with plain SQL too. The disadvantage of working with SQL is that the converters using SQL are not unit testable but only integration testable. Example:

``` csharp
/// <summary>
/// This converter checks at every app start if all expected settings (LastUpdate and Company) are available. If not they are created with default values
/// </summary>
public class AppSettingsConverterSql : BaseModelDataConverter
{
    public AppSettingsConverterSql(IUnitOfWork unitOfWork, IAppLoggerProxy appLogger) : base(unitOfWork, appLogger)
    {
        RequiredAppVersion = new Version(1, 0, 0);
    }

    /// <summary>
    /// Check if the premises are fulfilled to run the converter
    /// </summary>
    public override bool CheckPremisesToRunConverter()
    {
        // Run it always
        return true;
    }


    /// <summary>
    /// Run the converter
    /// </summary>
    public override void Run()
    {

        // Check setting LastUpdate
        var sql = "INSERT INTO [dbo].[AppSettings] ([Key],[Value]) SELECT 'LastUpdate', null WHERE NOT EXISTS (SELECT * FROM [dbo].[AppSettings] WHERE [key]='LastUpdate')";
        UnitOfWork.RunSql(sql);

        // Check setting Company
        sql = "INSERT INTO [dbo].[AppSettings] ([Key] ,[Value]) SELECT 'Company', 'TestCompany Ltd' WHERE NOT EXISTS (SELECT * FROM [dbo].[AppSettings] WHERE [key]='Company')";
        UnitOfWork.RunSql(sql);
    }
}
```

## Create a model data conversion handler factory: IModelDataConvertersHandlerFactory

If you do not need data migration or seeding choose DoNothingDataModelConvertersHandlerFactory:

``` csharp
    /// <summary>
    /// Do NOT seed the database or migrate any existing data
    /// </summary>
    public class DoNothingDataModelConvertersHandlerFactory : IModelDataConvertersHandlerFactory
    {
        private readonly IAppLoggerProxy _logger;

        /// <summary>
        /// Default ctor
        /// </summary>
        public DoNothingDataModelConvertersHandlerFactory(IAppLoggerProxy logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Create the instance of <see cref="IModelDataConvertersHandler"/> with all loaded converters for data migration
        /// </summary>
        /// <returns></returns>
        public IModelDataConvertersHandler CreateInstance()
        {

            var h = new ModelDataConvertersHandler(_logger);
            // Do NOT load converters here
            return h;

        }
    }
```

If you need migrations implement your own factory based on IModelDataConvertersHandlerFactory to load all the required converters based on IModelDataComverter to do the data migrations:

``` csharp
    public class SqlServerExampleDbEfModelDataConvertersHandlerFactory: IModelDataConvertersHandlerFactory
    {
        private readonly IAppLoggerProxy _logger;

        /// <summary>
        /// Default ctor
        /// </summary>
        public SqlServerExampleDbEfModelDataConvertersHandlerFactory(IAppLoggerProxy logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Create the instance of <see cref="IModelDataConvertersHandler"/> with all loaded converters for data migration
        /// </summary>
        /// <returns></returns>
        public IModelDataConvertersHandler CreateInstance()
        {
            var h = new ModelDataConvertersHandler(_logger);

            // Load converters now in the order you need!
            h.AddConverter<AppSettingsConverterEf>();
            h.AddConverter<UserTypeConverterEf>(); // must be done before user migration to have the UserType.ID available
            h.AddConverter<UsersConverterEf>();

            return h;
        }
    }
```



















## Running schema and data migrations: IMigrationController

Running the migrations and migrate the data afterwards will be handled by an implementation of IMigrationController. For SqlServer there is a default implementation SqlServerMigrationController. This default implementation takes a backup of an existing database, then migrates to the new database schema and runs data migrations afterwards.

## 



## Running all data migrations: IModelDataConvertersHandler



## Running a data migration for one entity type: IModelDataConverter



# About us

Bodoconsult <http://www.bodoconsult.de> is a Munich based software company from Germany.

Robert Leisner is senior software developer at Bodoconsult. See his profile on <http://www.bodoconsult.de/Curriculum_vitae_Robert_Leisner.pdf>.



