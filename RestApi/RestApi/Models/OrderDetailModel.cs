using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RestApi.Models;

public partial class OrderDetailModel
{
	[Key]
	public int OrderDetailId { get; set; }
	[DisplayName("Id đặt")]
	[Required(ErrorMessage = "Yêu cầu nhập Id đặt hàng")]
	public int? OrderId { get; set; }

	[DisplayName("Id sản phẩm theo size")]
	[Required(ErrorMessage = "Yêu cầu nhập Id sản phẩm theo size")]
	public int? ProductSizeId { get; set; }

	[DisplayName("Số lượng sản phẩm")]
	[Required(ErrorMessage = "Yêu cầu nhập số lượng sản phẩm")]
	[Range(1, int.MaxValue, ErrorMessage = "Số lượng sản phẩm phải lớn hơn 0")]
	public int? Quantity { get; set; }

	[DisplayName("Thành tiền")]
	[Range(0, double.MaxValue, ErrorMessage = "Giá trị không hợp lệ")]
	public decimal? Price { get; set; }



}
