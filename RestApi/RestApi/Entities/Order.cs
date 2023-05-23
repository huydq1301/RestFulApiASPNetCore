using System;
using System.Collections.Generic;

namespace RestApi.Entities;

public partial class Order
{
    public int OrderId { get; set; }

    public int? UserId { get; set; }

    public DateTime? OrderDate { get; set; }

    public decimal? TotalPrice { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual User? User { get; set; }
}
