// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;
using System.Collections.Generic;
using Bodoconsult.Database.Interfaces;
using Microsoft.Data.Sqlite;

namespace Bodoconsult.Database.Sqlite.Test.MetaDataSample
{
	public class CustomerService
	{

		private readonly IConnManager _db;

		public CustomerService(string connectionString)
		{
			_db = new SqliteConnManager(connectionString);
		}

		/// <summary>
		/// Insert a data row into table Customer from entity class Customer object
		/// </summary>
		public void AddNew(Customer item)
		{

			const string sql = "INSERT INTO \"Customer\"(\"CustomerId\", \"FirstName\", \"LastName\", \"Company\", \"Address\", \"City\", \"State\", \"Country\", \"PostalCode\", \"Phone\", \"Fax\", \"Email\", \"SupportRepId\") " +
					"VALUES (@CustomerId, @FirstName, @LastName, @Company, @Address, @City, @State, @Country, @PostalCode, @Phone, @Fax, @Email, @SupportRepId)";

			var cmd = new SqliteCommand(sql);

			SqliteParameter p;

			// Parameter @CustomerId
			p = new SqliteParameter("@CustomerId", SqliteType.Integer) { Value = item.CustomerId };
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

			_db.Exec(cmd);

		}

		/// <summary>
		/// Update a data row in table Customer from an entity class Customer object
		/// </summary>
		public void Update(Customer item)
		{

			const string sql = "UPDATE \"Customer\" SET \"FirstName\"=@FirstName, \"LastName\"=@LastName, \"Company\"=@Company, \"Address\"=@Address, \"City\"=@City, \"State\"=@State, \"Country\"=@Country, \"PostalCode\"=@PostalCode, \"Phone\"=@Phone, \"Fax\"=@Fax, \"Email\"=@Email WHERE \"CustomerId\"=@CustomerId; ";

			var cmd = new SqliteCommand(sql);

			SqliteParameter p;

			// Parameter @CustomerId
			p = new SqliteParameter("@CustomerId", SqliteType.Integer) { Value = item.CustomerId };
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

			//// Parameter @SupportRepId
			//p = new SqliteParameter("@SupportRepId", SqliteType.Integer) { Value = item.SupportRepId };
			//cmd.Parameters.Add(p);

			_db.Exec(cmd);

		}

		/// <summary>
		/// Delete a row from table Customer 
		/// </summary>
		public void Delete(System.Int64 customerId)
		{

			var cmd = new SqliteCommand("DELETE FROM \"Customer\" WHERE \"CustomerId\" = @PK");

			var p = new SqliteParameter("@PK", SqliteType.Integer) { Value = customerId };
			cmd.Parameters.Add(p);

			_db.Exec(cmd);

		}

		/// <summary>
		/// Get all rows in table Customer
		/// </summary>
		public IList<Customer> GetAll()
		{

			var result = new List<Customer>();

			var reader = _db.GetDataReader("SELECT * FROM \"Customer\"");

			while (reader.Read())
			{
				var dto = DataHelper.MapFromDbToCustomer(reader);
				result.Add(dto);

			}

			reader.Dispose();

			return result;
		}

		/// <summary>
		/// Get all rows in table Customer
		/// </summary>
		public Customer GetById(System.Int64 pkCustomerId)
		{

			Customer dto = null;

			var reader = _db.GetDataReader($"SELECT * FROM \"Customer\" WHERE \"CustomerId\"={pkCustomerId};");

			while (reader.Read())
			{
				dto = DataHelper.MapFromDbToCustomer(reader);
				break;

			}

			reader.Dispose();

			return dto;
		}

		/// <summary>
		/// Count all rows in table Customer 
		/// </summary>
		public int Count()
		{

			var result = _db.ExecWithResult("SELECT COUNT(*) FROM \"Customer\"");

			return Convert.ToInt32(result);
		}


	}
}