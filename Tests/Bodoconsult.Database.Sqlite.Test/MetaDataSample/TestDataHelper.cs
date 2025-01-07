using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace Bodoconsult.Database.Sqlite.Test.MetaDataSample
{
    public static class TestDataHelper
    {

        /// <summary>
        /// Get a filled new object from DTO class Customer object
        /// </summary>
        public static Customer NewCustomer()
        {

            var item = new Customer
            {
                CustomerId = 1,
                FirstName = "Hansi",
                LastName = "Mustermann",
                Company = "Test mit ü und ö",
                Address = "Test mit ü und ö",
                City = "Test mit ü und ö",
                State = "Test mit ü und ö",
                Country = "Test mit ü und ö",
                PostalCode = "T",
                Phone = "Test mit ü und ö",
                Fax = "Test mit ü und ö",
                Email = "Test mit ü und ö",
                SupportRepId = 1,
            };

            return item;

        }


        /// <summary>
        /// Insert a data row into table Customer from entity class Customer object
        /// </summary>
        /// <param name="count1"></param>
        public static DbCommand AddNewCommand(int count)
        {

            var item = NewCustomer();

			const string sql = "INSERT INTO \"Customer\"(\"CustomerId\", \"FirstName\", \"LastName\", \"Company\", \"Address\", \"City\", \"State\", \"Country\", \"PostalCode\", \"Phone\", \"Fax\", \"Email\", \"SupportRepId\") " +
					"VALUES (@CustomerId, @FirstName, @LastName, @Company, @Address, @City, @State, @Country, @PostalCode, @Phone, @Fax, @Email, @SupportRepId)";

			var cmd = new SqliteCommand(sql);

			SqliteParameter p;

			// Parameter @CustomerId
			p = new SqliteParameter("@CustomerId", SqliteType.Integer) { Value = count+1 };
			cmd.Parameters.Add(p);

			// Parameter @FirstName
			p = new SqliteParameter("@FirstName", SqliteType.Text) { Value = item.FirstName };
			cmd.Parameters.Add(p);

			// Parameter @LastName
			p = new SqliteParameter("@LastName", SqliteType.Text) { Value = item.LastName };
			cmd.Parameters.Add(p);

			// Parameter @Company
			p = new SqliteParameter("@Company", SqliteType.Text) { Value = item.Company };
			cmd.Parameters.Add(p);

			// Parameter @Address
			p = new SqliteParameter("@Address", SqliteType.Text) { Value = item.Address };
			cmd.Parameters.Add(p);

			// Parameter @City
			p = new SqliteParameter("@City", SqliteType.Text) { Value = item.City };
			cmd.Parameters.Add(p);

			// Parameter @State
			p = new SqliteParameter("@State", SqliteType.Text) { Value = item.State };
			cmd.Parameters.Add(p);

			// Parameter @Country
			p = new SqliteParameter("@Country", SqliteType.Text) { Value = item.Country };
			cmd.Parameters.Add(p);

			// Parameter @PostalCode
			p = new SqliteParameter("@PostalCode", SqliteType.Text) { Value = item.PostalCode };
			cmd.Parameters.Add(p);

			// Parameter @Phone
			p = new SqliteParameter("@Phone", SqliteType.Text) { Value = item.Phone };
			cmd.Parameters.Add(p);

			// Parameter @Fax
			p = new SqliteParameter("@Fax", SqliteType.Text) { Value = item.Fax };
			cmd.Parameters.Add(p);

			// Parameter @Email
			p = new SqliteParameter("@Email", SqliteType.Text) { Value = item.Email };
			cmd.Parameters.Add(p);

			// Parameter @SupportRepId
			p = new SqliteParameter("@SupportRepId", SqliteType.Integer) { Value = item.SupportRepId };
			cmd.Parameters.Add(p);

            return cmd;

        }


	}
}