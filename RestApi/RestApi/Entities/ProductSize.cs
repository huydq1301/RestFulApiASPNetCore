using System;
using System.Collections.Generic;

namespace RestApi.Entities;

public partial class ProductSize
{
    public int ProductSizeId { get; set; }

    public int? ProductId { get; set; }

    public decimal? Price { get; set; }

    public string? Size { get; set; }

    public int? Quantity { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Product? Product { get; set; }
}
