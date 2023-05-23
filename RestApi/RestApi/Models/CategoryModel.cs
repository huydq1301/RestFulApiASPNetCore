using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RestApi.Models;

public partial class CategoryModel
{
	[Key]
	public int CategoryId { get; set; }

	[Required(ErrorMessage = "Tên danh mục không được để trống")]
	[DisplayName("Tên danh mục")]
	public string? CategoryName { get; set; }

	[DisplayName("Mô tả danh mục")]
	public string? Description { get; set; }
}
