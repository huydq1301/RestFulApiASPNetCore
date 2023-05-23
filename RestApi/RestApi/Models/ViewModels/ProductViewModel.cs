using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace RestApi.Models;

public partial class ProductViewModel
{

	[Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
	[MaxLength(50)]
	public string? ProductName { get; set; }
	[Required]
	[Range(0, int.MaxValue, ErrorMessage = "Giá sản phẩm không hợp lệ")]
	public decimal? Price { get; set; }
	[StringLength(1000, MinimumLength = 10, ErrorMessage = "Mô tả sản phẩm từ 10 đến 1000 ký tự")]
	public string? Description { get; set; }
	[Required]
	[DisplayName("Thuộc danh mục")]
	public int? CategoryId { get; set; }
	[DisplayName("Đường dẫn ảnh")]
	public string? ImageUrl { get; set; }



   
}
