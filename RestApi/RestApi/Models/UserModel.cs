using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Identity;

namespace RestApi.Models;

public partial class UserModel
{
	[Key]
	public int UserId { get; set; }

	[Required(ErrorMessage = "Tên tài khoản không được để trống")]
	[MaxLength(20, ErrorMessage = "Tên tài khoản không được quá 20 kí tự")]
	[DisplayName("Tên tài khoản")]
	public string? Username { get; set; }

	[Required(ErrorMessage = "Mật khẩu không được để trống")]
	[MaxLength(20, ErrorMessage = "Mật khẩu không được quá 20 kí tự")]
	[DisplayName("Mật khẩu")]
	public string? Password { get; set; }

	[EmailAddress(ErrorMessage = "Email không đúng định dạng")]
	[DisplayName("Địa chỉ Email")]
	public string? Email { get; set; }

	[RegularExpression(@"^0\d{9,10}$", ErrorMessage = "Số điện thoại không đúng định dạng")]
	[DisplayName("Số điện thoại")]
	public string? Phone { get; set; }

	[DisplayName("Địa chỉ")]
	public string? Address { get; set; }

	[Required(ErrorMessage = "RoleId không được để trống")]
	public int RoleId { get; set; }
}
