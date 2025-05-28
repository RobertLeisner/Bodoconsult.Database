//using NUnit.Framework;

//namespace Bodoconsult.Core.Database.Test
//{
//    public class Tests
//    {

//        private const string ConnectionString =
//            @"SERVER=.\sqlexpress;DATABASE=MediaDb;Trusted_Connection=True;MultipleActiveResultSets=true";


//        private AdapterConnManager _db;

//        [SetUp]
//        public void Setup()
//        {
//            _db = SqlClientConnManager.GetConnManager(ConnectionString);

//        }

//        [Test]
//        public void TestTestConnection()
//        {

//            var erg = _db.TestConnection();


//            Assert.That(erg);
//        }


//        [Test]
//        public void TestDataTable()
//        {

//            const string sql = "SELECT * FROM dbo.settings";

//            var erg = _db.GetDataTable(sql);


//            Assert.That(erg.Rows.Count>0);
//        }
//    }
//}