using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestApi.Models;

public partial class WishlistModel
{
	[Key]
	public int WishlistId { get; set; }
	[Required]

	public int UserId { get; set; }
	[Required]

	public int ProductId { get; set; }

}
