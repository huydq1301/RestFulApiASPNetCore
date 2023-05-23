using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace RestApi.Models.ViewModels
{
	public class OrderVM
	{
		[Key]
		public int OrderId { get; set; }
		[DisplayName("Id người dùng")]
		[Required(ErrorMessage = "Id người dùng không được để trống")]
		public int? UserId { get; set; }
		[DisplayName("Thời gian đặt")]
		[Required(ErrorMessage = "Thời gian đặt không được để trống")]
		public DateTime? OrderDate { get; set; }
	}
}
