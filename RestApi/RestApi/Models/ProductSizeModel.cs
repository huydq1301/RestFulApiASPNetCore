using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace RestApi.Models;

public partial class ProductSizeModel
{
	[Key]
	public int ProductSizeId { get; set; }
	[DisplayName("Id sản phẩm ")]
	[Required]
	public int? ProductId { get; set; }
	[DisplayName("Giá sản phẩm ")]
	[Required]
	public decimal? Price { get; set; }
	[DisplayName("Size sản phẩm ")]
	[Required]
	public string? Size { get; set; }
	[DisplayName("Số lượng sản phẩm ")]
	[Required]
	public int? Quantity { get; set; }


}
