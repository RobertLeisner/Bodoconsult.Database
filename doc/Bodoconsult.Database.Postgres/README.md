# Bodoconsult.Database.Postgres 

This is the predecessor project of Bodoconsult.Core.Database.Postgres

## What does the library

Bodoconsult.Database.Postgres library simplifies the access to Postgres based databases. 
It handles the most common database actions like getting datatables, running SQL statement or fetch scalar values from the database.

Additionally the library contains some tools for developers making the usage of the database layers derived from Bodoconsult.Database as easy as possible. See below.

## How to use the library

The source code contain a NUnit test classes, the following source code is extracted from. The samples below show the most helpful use cases for the library.

The database used for unit tests is the Chinook sample database from <https://github.com/lerocha/chinook-database>.

### How to use the database layer

#### Connection string to Postgres

To connect to a Postgres database with this library you need to provide connection string. Here a sample for such a connection string:

Host=192.168.10.125;Port=5432;Username=postgres;Password=mypassword;Database=dvdrental;SslMode=Disable;

For more information on connection strings see <https://www.connectionstrings.com/postgresql/>.

#### Class overview and main methods

The most important class in the library is the the class PostgresConnManager. PostgresConnManager exposes the following main methods:

##### Connection related and general methods

* TestConnection()
* GetCommand()
* GetParameter()

##### Methods for running SQL statements or commands

* Exec()
* ExecMultiple()

##### Methods for getting data

* GetDataTable()
* GetDataReader()
* GetDataAdapter()

#### Samples

``` csharp

        [SetUp]
        public void Setup()
        {
            var conn = TestHelper.PostgresConnectionString;

            _db = PostgresConnManager.GetConnManager(conn);

        }
		
		
        [Test]
        public void TestTestConnection()
        {
            // Assert

            // Act
            var result = _db.TestConnection();

            // Assert
            Assert.That(result);

        }

        [Test]
        public void TestGetDataTableSql()
        {
            // Assert
            var sql =
                "SELECT * " +
                "FROM Film; ";

            // Act
            var result = _db.GetDataTable(sql);

            // Assert
            Assert.That(result);

        }


        [Test]
        public void TestGetDataTableCommand()
        {
            // Act
            var sql =
                "SELECT * " +
                "FROM Film; ";

            var cmd = new NpgsqlCommand(sql);

            // Act
            var result = _db.GetDataTable(cmd);

            // Assert
            Assert.That(result);

        }


        /// <summary>
        /// Get a datatable from the database from a (parameterized) SqlCommand object (choose this option to avoid SQL injection)
        /// </summary>
        [Test]
        public void TestGetDataTableFromCommandWithGetCommand()
        {
            const string sql = "SELECT * FROM Film";

            var cmd = _db.GetCommand();
            cmd.CommandText = sql;

            // Add parameters here if required
            var erg = _db.GetDataTable(cmd);


            Assert.That(erg.Rows.Count > 0);
        }


        /// <summary>
        /// Get a datatable from the database from a plain SQL string (avoid this option due to SQL injection)
        /// </summary>
        [Test]
        public void TestGetDataReaderFromSql()
        {

            const string sql = "SELECT * FROM Film";

            var erg = _db.GetDataReader(sql);

            Assert.That(erg.FieldCount > 0);
        }



        /// <summary>
        /// Get a datatable from the database from a (parameterized) SqlCommand object (choose this option to avoid SQL injection)
        /// </summary>
        [Test]
        public void TestGetDataReaderFromCommand()
        {

            const string sql = "SELECT * FROM Film";


            var cmd = new NpgsqlCommand
            {
                CommandText = sql
            };

            // Add parameters here if required

            var erg = _db.GetDataReader(cmd);

            Assert.That(erg.FieldCount > 0);
        }


        /// <summary>
        /// Get a datatable from the database from a (parameterized) SqlCommand object (choose this option to avoid SQL injection)
        /// </summary>
        [Test]
        public void TestGetDataReaderFromCommandWidthGetCommand()
        {
            const string sql = "SELECT * FROM Film";

            var cmd = _db.GetCommand();
            cmd.CommandText = sql;

            // Add parameters here if required
            var erg = _db.GetDataReader(cmd);

            Assert.That(erg.FieldCount > 0);

        }


        /// <summary>
        /// Get a datatable from the database from a (parameterized) SqlCommand object (choose this option to avoid SQL injection)
        /// </summary>
        [Test]
        public void TestGetDataReaderFromCommandWidthGetCommandAndParameter()
        {
            const string sql = "SELECT * FROM Film WHERE film_id=@ID;";

            var cmd = _db.GetCommand();
            cmd.CommandText = sql;

            var p = _db.GetParameter(cmd, "@ID", GeneralDbType.Int);
            p.Value = 8;

            // Add parameters here if required
            var erg = _db.GetDataReader(cmd);

            Assert.That(erg.FieldCount > 0);
            Assert.That(erg.HasRows);

        }

        /// <summary>
        /// Execute a plain SQL string (avoid this option due to SQL injection)
        /// </summary>
        [Test]
        public void TestExecFromSql()
        {

            const string sql = "DELETE FROM Film WHERE film_id=-99";

            Assert.DoesNotThrow(() => _db.Exec(sql));
        }


        /// <summary>
        /// Execute a SqlCommand object (choose this option to avoid SQL injection)
        /// </summary>
        [Test]
        public void TestExecFromCommand()
        {

            const string sql = "DELETE FROM Film WHERE film_id=@Key";

            // Create command
            var cmd = new NpgsqlCommand()
            {
                CommandText = sql
            };

            // Add a parameter to the command
            var para = cmd.Parameters.Add("@Key", NpgsqlDbType.Integer);
            para.Value = -99;

            Assert.DoesNotThrow(() => _db.Exec(cmd));
        }


        /// <summary>
        /// Get a scalar value from database from a plain SQL string (avoid this option due to SQL injection)
        /// </summary>
        [Test]
        public void TestExecWithResultFromSql()
        {

            const string sql = "SELECT film_id FROM film WHERE film_id=8";

            var result = _db.ExecWithResult(sql);

            Assert.That(result);
            Assert.IsFalse(string.IsNullOrEmpty(result));
        }


        /// <summary>
        /// Get a scalar value from database from SqlCommand object (choose this option to avoid SQL injection)
        /// </summary>
        [Test]
        public void TestExecWithResultFromCommand()
        {

            const string sql = "SELECT film_id FROM Film WHERE film_id=@Key";

            // Create command
            var cmd = new NpgsqlCommand
            {
                CommandText = sql
            };

            // Add a parameter to the command
            var para = cmd.Parameters.Add("@Key", NpgsqlDbType.Integer);
            para.Value = 8;

            var result = _db.ExecWithResult(cmd);

            Assert.That(result);
            Assert.IsFalse(string.IsNullOrEmpty(result));
        }


        [Test]
        public void TestExecMultiple()
        {
            const string sql = "DELETE FROM Film WHERE film_id=-99";

            var commands = new List<DbCommand>();

            var cmd = new NpgsqlCommand(sql);
            commands.Add(cmd);

            cmd = new NpgsqlCommand(sql);
            commands.Add(cmd);

            cmd = new NpgsqlCommand(sql);
            commands.Add(cmd);

            var result = _db.ExecMultiple(commands);

            Assert.That(result == 0);
        }

```

### Developer tools for making best usage of Bodoconsult.Database.Postgres based data layers

There is as basic metadata infrastructure implemented to help developers to make best usage of Bodoconsult.Database. This metadata infrastructure derived from 
IMetaDataService / BaseMetaDataService creates C# code files based on a certain SQL statement OR DBCommand object. 

1. It first creates an adjusted entity class for the SQL statement OR DBCommand object.
2. Then it creates a mapping method to map a DataReader row object to the entity class.
3. At last it creates an service class for adding new rows, updating rows and getting rows from the database table related to SQL statement OR DBCommand object. 
This feature works only well for statements absed on a single table. If there are more tables targeted in the statement, only the GetAll and GetById may make sense in the service class. 
The other methods ma be removed in that case.

Here a sample how to use the IMetaDataService infrastructure:

``` csharp

			const string conn = "Valid ADO.NET provider connection string";

            const string sql = "SELECT * FROM \"Customer\";";

            const string entityName = "Customer";

            const string primaryKeyField = "CustomerId";

            const string targetPath = @"D:\temp";

			// Instanciate the metadata service
			var service = PostgresMetaDataService();

			// Get the metadata for the SQL statement
            service.GetMetaData(conn, entityName, sql, primaryKeyField);

            // Export the code files
            var result = service.ExportAll(targetPath);

```

As a result you will find four files with code in the folder D:\temp:

* D:\temp\Customer_EntityClass_Code.txt
* D:\temp\Customer_DataHelperMethod_Code.txt
* D:\temp\Customer_ServiceClass_Code.txt
* D:\temp\Customer_TestDataHelperMethod_Code.txt

## About us

Bodoconsult (<http://www.bodoconsult.de>) is a Munich based software development company from Germany.

Robert Leisner is senior software developer at Bodoconsult. See his profile on <http://www.bodoconsult.de/Curriculum_vitae_Robert_Leisner.pdf>.

