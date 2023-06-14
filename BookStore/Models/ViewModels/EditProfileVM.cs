using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace BookStore.Models.ViewModels
{
	public class EditProfileVM
	{
		public int Id { get; set; }
		[Required]
		[StringLength(256)]
		[EmailAddress(ErrorMessage = "Email 格式有誤")]
		public string Email { get; set; }

		[Required]
		[StringLength(30)]
		[Display(Name = "姓名")]
		public string Name { get; set; }

		[StringLength(10)]
		[Display(Name = "手機")]
		public string Mobile { get; set; }
	}
}