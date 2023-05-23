using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace RestApi.Models.ViewModels
{
	public class CategoryVM
	{
		[Required]

		public string? CategoryName { get; set; }
		[DisplayName("Mô tả danh mục")]
		public string? Description { get; set; }
	}
}
