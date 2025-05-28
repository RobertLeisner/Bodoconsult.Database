// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Data;
using System.Diagnostics;
using System.Reflection;
using Bodoconsult.App.Benchmarking;
using Bodoconsult.App.ClientNotifications;
using Bodoconsult.App.EventCounters;
using Bodoconsult.App.Factories;
using Bodoconsult.App.Interfaces;
using Bodoconsult.App.Logging;
using Bodoconsult.Database.Test.Utilities.App;
using Bodoconsult.Database.Test.Utilities.TestData;
using NUnit.Framework;

namespace Bodoconsult.Database.Test.Utilities.Helpers;

public static class TestHelper
{
    public const string DbName = "TestDbSqlClient";

    /// <summary>
    /// Connection string for a LocalDB instance. Database names for LocalDb, SqlExpress and SqlServer should not be equal. Otherwise, there will be errors running the tests due to blocked database files.
    /// </summary>
    public static string LocalDbConnectionString { get; set; } =
        $@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog={DbName};Integrated Security=true;MultipleActiveResultSets=True;App=EFCore";

    /// <summary>
    /// Connection string
    /// </summary>
    public static string ConnectionString => Globals.Instance.AppStartParameter.DefaultConnectionString;

    /// <summary>
    /// Create a <see cref="IAppEventSource"/> instance
    /// </summary>
    /// <returns><see cref="IAppEventSource"/> instance based on <see cref="AppApmEventSource"/></returns>
    internal static AppApmEventSourceFactory CreateAppEventSourceFactory()
    {
        var logger = GetFakeAppLoggerProxy();

        var aes = new AppApmEventSourceFactory(logger);

        return aes;
    }

    public static IClientNotificationLicenseManager GetFakeLicenceManager()
    {
        return new FakeClientNotificationLicenseManager();
    }

    private static string _testDataPath;

    public static string TempPath => Path.GetTempPath();

    public static string TestDataPath
    {
        get
        {

            if (!string.IsNullOrEmpty(_testDataPath))
            {
                return _testDataPath;
            }

            var path = new DirectoryInfo(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName).Parent.Parent.Parent.FullName;

            _testDataPath = Path.Combine(path, "TestData");

            if (!Directory.Exists(_testDataPath))
            {
                Directory.CreateDirectory(_testDataPath);
            }

            return _testDataPath;
        }
    }

    /// <summary>
    /// Start an app by file name
    /// </summary>
    /// <param name="fileName"></param>
    public static void StartFile(string fileName)
    {

        if (!Debugger.IsAttached)
        {
            return;
        }

        Assert.That(File.Exists(fileName));

        var p = new Process { StartInfo = new ProcessStartInfo { UseShellExecute = true, FileName = fileName } };

        p.Start();

    }

    /// <summary>
    /// Get a fully set up fake logger
    /// </summary>
    /// <returns>Logger instance</returns>
    public static AppLoggerProxy GetFakeAppLoggerProxy()
    {
        if (_logger != null)
        {
            return _logger;
        }
        _logger = new AppLoggerProxy(new FakeLoggerFactory(), Globals.Instance.LogDataFactory);
        return _logger;
    }

    private static AppLoggerProxy _logger;

    /// <summary>
    /// Get a fully set up fake bench logger
    /// </summary>
    /// <returns>Bench logger instance</returns>
    public static AppBenchProxy GetFakeAppBenchProxy()
    {
        if (_bench != null)
        {
            return _bench;
        }
        _bench = new AppBenchProxy(new FakeLoggerFactory(), Globals.Instance.LogDataFactory);
        return _bench;
    }

    private static AppBenchProxy _bench;


    public static void GetDataForService(ICollection<DemoEntity> result, DateTime from, DateTime to)
    {
        var entity = new DemoEntity
        {
            Id = 1,
            Name = "Test1",
            Date = from
        };

        result.Add(entity);

        entity = new DemoEntity
        {
            Id = 2,
            Name = "Test2",
            Date = from.AddDays(1)
        };

        result.Add(entity);

        entity = new DemoEntity
        {
            Id = 3,
            Name = "Test3",
            Date = to.AddDays(1)
        };

        result.Add(entity);

        entity = new DemoEntity
        {
            Id = 4,
            Name = "Test4",
            Date = from
        };

        result.Add(entity);
    }

    public static DataTable CreateEmptyDataTable()
    {
        var table = new DataTable("TTrace");
        table.Columns.Add("ID", typeof(Guid));
        table.Columns.Add("Date", typeof(DateTime));
        table.Columns.Add("TowerSn", typeof(string));
        table.Columns.Add("TraceCode", typeof(int));

        return table;
    }

    public static DataTable GetDataTableForTests()
    {
        var table = CreateEmptyDataTable();

        var dr = table.NewRow();
        dr["ID"] = Guid.NewGuid();
        dr["Date"] = DateTime.Now;
        dr["TowerSn"] = "001234";
        dr["TraceCode"] = 999;
        table.Rows.Add(dr);

        dr = table.NewRow();
        dr["ID"] = Guid.NewGuid();
        dr["Date"] = DateTime.Now.AddYears(-1);
        dr["TowerSn"] = "001234";
        dr["TraceCode"] = 999;
        table.Rows.Add(dr);

        dr = table.NewRow();
        dr["ID"] = Guid.NewGuid();
        dr["Date"] = DateTime.Now.AddMonths(-6);
        dr["TowerSn"] = "001234";
        dr["TraceCode"] = 999;
        table.Rows.Add(dr);

        dr = table.NewRow();
        dr["ID"] = Guid.NewGuid();
        dr["Date"] = DateTime.Now.AddYears(-2);
        dr["TowerSn"] = "001234";
        dr["TraceCode"] = 999;
        table.Rows.Add(dr);

        return table;
    }


    public static void GetDataTableForService(DataTable result, DateTime from, DateTime to)
    {
        var dr = result.NewRow();
        dr["ID"] = Guid.NewGuid();
        dr["Date"] = from;
        dr["TowerSn"] = "001234";
        dr["TraceCode"] = 999;
        result.Rows.Add(dr);

        dr = result.NewRow();
        dr["ID"] = Guid.NewGuid();
        dr["Date"] = from.AddDays(1);
        dr["TowerSn"] = "001234";
        dr["TraceCode"] = 999;
        result.Rows.Add(dr);

        dr = result.NewRow();
        dr["ID"] = Guid.NewGuid();
        dr["Date"] = from.AddDays(1);
        dr["TowerSn"] = "001234";
        dr["TraceCode"] = 999;
        result.Rows.Add(dr);

        dr = result.NewRow();
        dr["ID"] = Guid.NewGuid();
        dr["Date"] = from;
        dr["TowerSn"] = "001234";
        dr["TraceCode"] = 999;
        result.Rows.Add(dr);
    }
}