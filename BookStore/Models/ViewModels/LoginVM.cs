using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace BookStore.Models.ViewModels
{
	public class LoginVM
	{
		[Required]
		[StringLength(20)]
		[Display(Name = "帳號")]
		public string Account { get; set; }

		[Required]
		[StringLength(20)]
		[Display(Name = "密碼")]
		[DataType(DataType.Password)]
		public string Password { get; set; }
	}
}