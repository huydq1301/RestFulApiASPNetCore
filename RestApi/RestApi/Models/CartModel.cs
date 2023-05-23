using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RestApi.Models;

public partial class CartModel
{
	[Key]
	public int CartId { get; set; }

	[Required(ErrorMessage = "UserId không được để trống")]
	[DisplayName("Id người dùng")]
	public int? UserId { get; set; }

	[Required(ErrorMessage = "ProductId không được để trống")]
	[DisplayName("Id sản phẩm")]
	public int ProductId { get; set; }

	[Required(ErrorMessage = "Quantity không được để trống")]
	[Range(1, int.MaxValue, ErrorMessage = "Quantity phải lớn hơn 0")]
	[DisplayName("Số lượng sản phẩm")]
	public int? Quantity { get; set; }
}

