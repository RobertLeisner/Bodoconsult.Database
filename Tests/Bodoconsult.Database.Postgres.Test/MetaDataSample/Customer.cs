using System;
using System.ComponentModel.DataAnnotations;

namespace Bodoconsult.Database.Postgres.Test.MetaDataSample
{
	public class Customer
	{

		/// <summary>
		/// CustomerId 
		/// </summary>
		public Int32 CustomerId { get; set; }


		/// <summary>
		/// FirstName  Maximum length: 40 chars
		/// </summary>
		[MaxLength(40, ErrorMessage = "Maximum number of characters that can be entered for FirstName is 40!")]
		public String FirstName { get; set; }


		/// <summary>
		/// LastName  Maximum length: 20 chars
		/// </summary>
		[MaxLength(20, ErrorMessage = "Maximum number of characters that can be entered for LastName is 20!")]
		public String LastName { get; set; }


		/// <summary>
		/// Company  Maximum length: 80 chars
		/// </summary>
		[MaxLength(80, ErrorMessage = "Maximum number of characters that can be entered for Company is 80!")]
		public String Company { get; set; }


		/// <summary>
		/// Address  Maximum length: 70 chars
		/// </summary>
		[MaxLength(70, ErrorMessage = "Maximum number of characters that can be entered for Address is 70!")]
		public String Address { get; set; }


		/// <summary>
		/// City  Maximum length: 40 chars
		/// </summary>
		[MaxLength(40, ErrorMessage = "Maximum number of characters that can be entered for City is 40!")]
		public String City { get; set; }


		/// <summary>
		/// State  Maximum length: 40 chars
		/// </summary>
		[MaxLength(40, ErrorMessage = "Maximum number of characters that can be entered for State is 40!")]
		public String State { get; set; }


		/// <summary>
		/// Country  Maximum length: 40 chars
		/// </summary>
		[MaxLength(40, ErrorMessage = "Maximum number of characters that can be entered for Country is 40!")]
		public String Country { get; set; }


		/// <summary>
		/// PostalCode  Maximum length: 10 chars
		/// </summary>
		[MaxLength(10, ErrorMessage = "Maximum number of characters that can be entered for PostalCode is 10!")]
		public String PostalCode { get; set; }


		/// <summary>
		/// Phone  Maximum length: 24 chars
		/// </summary>
		[MaxLength(24, ErrorMessage = "Maximum number of characters that can be entered for Phone is 24!")]
		public String Phone { get; set; }


		/// <summary>
		/// Fax  Maximum length: 24 chars
		/// </summary>
		[MaxLength(24, ErrorMessage = "Maximum number of characters that can be entered for Fax is 24!")]
		public String Fax { get; set; }


		/// <summary>
		/// Email  Maximum length: 60 chars
		/// </summary>
		[MaxLength(60, ErrorMessage = "Maximum number of characters that can be entered for Email is 60!")]
		public String Email { get; set; }


		/// <summary>
		/// SupportRepId 
		/// </summary>
		public Int32 SupportRepId { get; set; }


	}
}
