using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace RestApi.Models.ViewModels
{
	public class UserVM
	{
		[Key]
		public int UserId { get; set; }

		[Required(ErrorMessage = "Vui lòng nhập tên tài khoản.")]
		[MaxLength(20, ErrorMessage = "Tên tài khoản không được vượt quá 20 ký tự.")]
		[DisplayName("Tên tài khoản")]
		public string Username { get; set; }

		[Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
		[MaxLength(20, ErrorMessage = "Mật khẩu không được vượt quá 20 ký tự.")]
		[DisplayName("Mật khẩu")]
		public string Password { get; set; }

		[EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
		[DisplayName("Địa chỉ Email")]
		public string Email { get; set; }
		[RegularExpression(@"^0\d{9,10}$", ErrorMessage = "Số điện thoại không đúng định dạng")]
		[DisplayName("Số điện thoại")]
		public string Phone { get; set; }

		[DisplayName("Địa chỉ")]
		public string Address { get; set; }
	}
}
