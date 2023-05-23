using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace RestApi.Models;

public partial class ReviewModel
{
	[Key]
	public int ReviewId { get; set; }

	[DisplayName("Id người dùng ")]
	[Required(ErrorMessage = "Vui lòng nhập Id người dùng.")]
	public int UserId { get; set; }

	[DisplayName("Id sản phẩm ")]
	[Required(ErrorMessage = "Vui lòng nhập Id sản phẩm.")]
	public int ProductId { get; set; }

	[DisplayName("Số sao ")]
	[Required(ErrorMessage = "Vui lòng chọn số sao.")]
	[Range(1, 5, ErrorMessage = "Số sao phải từ 1 đến 5.")]
	public int? Rating { get; set; }

	[DisplayName("Bình luận ")]
	[StringLength(1000, ErrorMessage = "Bình luận không được quá 1000 ký tự.")]
	public string? Comment { get; set; }

	[DisplayName("Ngày review ")]
	public DateTime? ReviewDate { get; set; }
}
