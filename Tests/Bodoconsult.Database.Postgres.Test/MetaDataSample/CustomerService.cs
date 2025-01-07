// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System;
using System.Collections.Generic;
using Bodoconsult.Database.Interfaces;
using Npgsql;
using NpgsqlTypes;

namespace Bodoconsult.Database.Postgres.Test.MetaDataSample
{
	public class CustomerService
	{

		private readonly IConnManager _db;

		public CustomerService(string connectionString)
		{
			_db = new PostgresConnManager(connectionString);
		}

		/// <summary>
		/// Insert a data row into table Customer from entity class Customer object
		/// </summary>
		public void AddNew(Customer item)
		{

			const string sql = "INSERT INTO \"Customer\"(\"CustomerId\", \"FirstName\", \"LastName\", \"Company\", \"Address\", \"City\", \"State\", \"Country\", \"PostalCode\", \"Phone\", \"Fax\", \"Email\", \"SupportRepId\") " +
					"VALUES (@CustomerId, @FirstName, @LastName, @Company, @Address, @City, @State, @Country, @PostalCode, @Phone, @Fax, @Email, @SupportRepId)";

			var cmd = new NpgsqlCommand(sql);

			NpgsqlParameter p;

			// Parameter @CustomerId
			p = new NpgsqlParameter("@CustomerId", NpgsqlDbType.Integer) { Value = item.CustomerId };
			cmd.Parameters.Add(p);

			// Parameter @FirstName
			p = new NpgsqlParameter("@FirstName", NpgsqlDbType.Varchar) { Value = item.FirstName };
			cmd.Parameters.Add(p);

			// Parameter @LastName
			p = new NpgsqlParameter("@LastName", NpgsqlDbType.Varchar) { Value = item.LastName };
			cmd.Parameters.Add(p);

			// Parameter @Company
			p = new NpgsqlParameter("@Company", NpgsqlDbType.Varchar) { Value = item.Company };
			cmd.Parameters.Add(p);

			// Parameter @Address
			p = new NpgsqlParameter("@Address", NpgsqlDbType.Varchar) { Value = item.Address };
			cmd.Parameters.Add(p);

			// Parameter @City
			p = new NpgsqlParameter("@City", NpgsqlDbType.Varchar) { Value = item.City };
			cmd.Parameters.Add(p);

			// Parameter @State
			p = new NpgsqlParameter("@State", NpgsqlDbType.Varchar) { Value = item.State };
			cmd.Parameters.Add(p);

			// Parameter @Country
			p = new NpgsqlParameter("@Country", NpgsqlDbType.Varchar) { Value = item.Country };
			cmd.Parameters.Add(p);

			// Parameter @PostalCode
			p = new NpgsqlParameter("@PostalCode", NpgsqlDbType.Varchar) { Value = item.PostalCode };
			cmd.Parameters.Add(p);

			// Parameter @Phone
			p = new NpgsqlParameter("@Phone", NpgsqlDbType.Varchar) { Value = item.Phone };
			cmd.Parameters.Add(p);

			// Parameter @Fax
			p = new NpgsqlParameter("@Fax", NpgsqlDbType.Varchar) { Value = item.Fax };
			cmd.Parameters.Add(p);

			// Parameter @Email
			p = new NpgsqlParameter("@Email", NpgsqlDbType.Varchar) { Value = item.Email };
			cmd.Parameters.Add(p);

			// Parameter @SupportRepId
			p = new NpgsqlParameter("@SupportRepId", NpgsqlDbType.Integer) { Value = item.SupportRepId };
			cmd.Parameters.Add(p);

			_db.Exec(cmd);

		}

		/// <summary>
		/// Update a data row in table Customer from an entity class Customer object
		/// </summary>
		public void Update(Customer item)
		{

			const string sql = "UPDATE \"Customer\" SET \"FirstName\"=@FirstName, \"LastName\"=@LastName, \"Company\"=@Company, \"Address\"=@Address, \"City\"=@City, \"State\"=@State, \"Country\"=@Country, \"PostalCode\"=@PostalCode, \"Phone\"=@Phone, \"Fax\"=@Fax, \"Email\"=@Email, \"SupportRepId\"=@SupportRepId WHERE \"CustomerId\"=@CustomerId; ";

			var cmd = new NpgsqlCommand(sql);

			NpgsqlParameter p;

			// Parameter @CustomerId
			p = new NpgsqlParameter("@CustomerId", NpgsqlDbType.Integer) { Value = item.CustomerId };
			cmd.Parameters.Add(p);

			// Parameter @FirstName
			p = new NpgsqlParameter("@FirstName", NpgsqlDbType.Varchar) { Value = item.FirstName };
			cmd.Parameters.Add(p);

			// Parameter @LastName
			p = new NpgsqlParameter("@LastName", NpgsqlDbType.Varchar) { Value = item.LastName };
			cmd.Parameters.Add(p);

			// Parameter @Company
			p = new NpgsqlParameter("@Company", NpgsqlDbType.Varchar) { Value = item.Company };
			cmd.Parameters.Add(p);

			// Parameter @Address
			p = new NpgsqlParameter("@Address", NpgsqlDbType.Varchar) { Value = item.Address };
			cmd.Parameters.Add(p);

			// Parameter @City
			p = new NpgsqlParameter("@City", NpgsqlDbType.Varchar) { Value = item.City };
			cmd.Parameters.Add(p);

			// Parameter @State
			p = new NpgsqlParameter("@State", NpgsqlDbType.Varchar) { Value = item.State };
			cmd.Parameters.Add(p);

			// Parameter @Country
			p = new NpgsqlParameter("@Country", NpgsqlDbType.Varchar) { Value = item.Country };
			cmd.Parameters.Add(p);

			// Parameter @PostalCode
			p = new NpgsqlParameter("@PostalCode", NpgsqlDbType.Varchar) { Value = item.PostalCode };
			cmd.Parameters.Add(p);

			// Parameter @Phone
			p = new NpgsqlParameter("@Phone", NpgsqlDbType.Varchar) { Value = item.Phone };
			cmd.Parameters.Add(p);

			// Parameter @Fax
			p = new NpgsqlParameter("@Fax", NpgsqlDbType.Varchar) { Value = item.Fax };
			cmd.Parameters.Add(p);

			// Parameter @Email
			p = new NpgsqlParameter("@Email", NpgsqlDbType.Varchar) { Value = item.Email };
			cmd.Parameters.Add(p);

			// Parameter @SupportRepId
			p = new NpgsqlParameter("@SupportRepId", NpgsqlDbType.Integer) { Value = item.SupportRepId };
			cmd.Parameters.Add(p);

			_db.Exec(cmd);

		}

		/// <summary>
		/// Delete a row from table Customer 
		/// </summary>
		public void Delete(System.Int32 customerId)
		{

			var cmd = new NpgsqlCommand("DELETE FROM \"Customer\" WHERE \"CustomerId\" = @PK");

			var p = new NpgsqlParameter("@PK", NpgsqlDbType.Integer) { Value = customerId };
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
		public Customer GetById(System.Int32 pkCustomerId)
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