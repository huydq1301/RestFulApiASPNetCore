using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace RestApi.Models.ViewModels
{
	public class LoginVM
	{
		[Required]
		[MaxLength(20)]

		public string? Username { get; set; }
		[Required]
		[MaxLength(20)]

		public string? Password { get; set; }
	}
}
