using System;
using System.ComponentModel.DataAnnotations;

namespace Bodoconsult.Database.Sqlite.Test.MetaDataSample
{
    public class Customer
    {

        /// <summary>
        /// CustomerId 
        /// </summary>
        public Int64 CustomerId { get; set; }


        /// <summary>
        /// FirstName  Maximum length: -1 chars
        /// </summary>
        [MaxLength(-1, ErrorMessage = "Maximum number of characters that can be entered for FirstName is -1!")]
        public String FirstName { get; set; }


        /// <summary>
        /// LastName  Maximum length: -1 chars
        /// </summary>
        [MaxLength(-1, ErrorMessage = "Maximum number of characters that can be entered for LastName is -1!")]
        public String LastName { get; set; }


        /// <summary>
        /// Company  Maximum length: -1 chars
        /// </summary>
        [MaxLength(-1, ErrorMessage = "Maximum number of characters that can be entered for Company is -1!")]
        public String Company { get; set; }


        /// <summary>
        /// Address  Maximum length: -1 chars
        /// </summary>
        [MaxLength(-1, ErrorMessage = "Maximum number of characters that can be entered for Address is -1!")]
        public String Address { get; set; }


        /// <summary>
        /// City  Maximum length: -1 chars
        /// </summary>
        [MaxLength(-1, ErrorMessage = "Maximum number of characters that can be entered for City is -1!")]
        public String City { get; set; }


        /// <summary>
        /// State  Maximum length: -1 chars
        /// </summary>
        [MaxLength(-1, ErrorMessage = "Maximum number of characters that can be entered for State is -1!")]
        public String State { get; set; }


        /// <summary>
        /// Country  Maximum length: -1 chars
        /// </summary>
        [MaxLength(-1, ErrorMessage = "Maximum number of characters that can be entered for Country is -1!")]
        public String Country { get; set; }


        /// <summary>
        /// PostalCode  Maximum length: -1 chars
        /// </summary>
        [MaxLength(-1, ErrorMessage = "Maximum number of characters that can be entered for PostalCode is -1!")]
        public String PostalCode { get; set; }


        /// <summary>
        /// Phone  Maximum length: -1 chars
        /// </summary>
        [MaxLength(-1, ErrorMessage = "Maximum number of characters that can be entered for Phone is -1!")]
        public String Phone { get; set; }


        /// <summary>
        /// Fax  Maximum length: -1 chars
        /// </summary>
        [MaxLength(-1, ErrorMessage = "Maximum number of characters that can be entered for Fax is -1!")]
        public String Fax { get; set; }


        /// <summary>
        /// Email  Maximum length: -1 chars
        /// </summary>
        [MaxLength(-1, ErrorMessage = "Maximum number of characters that can be entered for Email is -1!")]
        public String Email { get; set; }


        /// <summary>
        /// SupportRepId 
        /// </summary>
        public Int64 SupportRepId { get; set; }


    }
}