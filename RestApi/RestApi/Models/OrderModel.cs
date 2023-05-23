using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace RestApi.Models;

public partial class OrderModel
{
	[Key]
	public int OrderId { get; set; }
	[DisplayName("Id người dùng")]
	[Required]
	public int? UserId { get; set; }
	[DisplayName("Thời gian đặt")]

	public DateTime? OrderDate { get; set; }
	[DisplayName("Tổng tiền")]

	public decimal? TotalPrice { get; set; }


}
