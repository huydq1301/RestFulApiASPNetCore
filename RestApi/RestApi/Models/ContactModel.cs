using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace RestApi.Models;

public partial class ContactModel
{
	[Key]
	public int ContactId { get; set; }
	[DisplayName("Tên liên hệ")]
	public string? Name { get; set; }
	[DisplayName("Email liên hệ")]

	public string? Email { get; set; }
	[DisplayName("Số điện thoại liên hệ")]

	public string? Phone { get; set; }

}
