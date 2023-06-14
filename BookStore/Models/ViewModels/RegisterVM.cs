using BookStore.Models.EFModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookStore.Models.ViewModels
{
	public class RegisterVM
	{
		public int Id { get; set; }

		[Required]
		[StringLength(20)]
		[Display(Name="帳號")]
		public string Account { get; set; }

		[Required]
		[StringLength(20)]
		[Display(Name = "密碼")]
		[DataType(DataType.Password)]
		public string Password { get; set; }
		
		[Required]
		[StringLength(20)]
		[Display(Name = "確認密碼")]
		[DataType(DataType.Password)]
		[Compare("Password",ErrorMessage ="必須與 '密碼' 欄位值相同")]
		public string ComfirmPassword { get; set; }

		[Required]
		[StringLength(256)]
		[EmailAddress(ErrorMessage ="Email 格式有誤")]
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