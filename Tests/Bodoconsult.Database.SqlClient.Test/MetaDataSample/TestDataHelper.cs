namespace Bodoconsult.Database.SqlClient.Test.MetaDataSample
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


    }
}