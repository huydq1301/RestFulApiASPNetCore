using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace RestApi.Models.ViewModels
{
	public class OrderDetailVM
	{
		[Key]
		public int OrderDetailId { get; set; }
		[DisplayName("Id đặt")]
		[Required]
		public int? OrderId { get; set; }
		[DisplayName("Id sản phẩm theo size")]
		[Required]
		public int? ProductSizeId { get; set; }
		[DisplayName("Số lượng sản phẩm")]
		[Required]
		public int? Quantity { get; set; }
	}
}
