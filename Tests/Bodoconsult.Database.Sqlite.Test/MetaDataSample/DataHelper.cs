using System.Data;

namespace Bodoconsult.Database.Sqlite.Test.MetaDataSample
{
    public static class DataHelper
    {

        /// <summary>
        /// Mapping datareader to entity class Customer
        /// </summary>
        public static Customer MapFromDbToCustomer(IDataReader reader)
        {

            var dto = new Customer();
            dto.CustomerId = reader.GetInt64(0);
            dto.FirstName = reader[1].ToString();
            dto.LastName = reader[2].ToString();
            dto.Company = reader[3].ToString();
            dto.Address = reader[4].ToString();
            dto.City = reader[5].ToString();
            dto.State = reader[6].ToString();
            dto.Country = reader[7].ToString();
            dto.PostalCode = reader[8].ToString();
            dto.Phone = reader[9].ToString();
            dto.Fax = reader[10].ToString();
            dto.Email = reader[11].ToString();
            dto.SupportRepId = reader.GetInt64(0);

            return dto;

        }


	}
}
