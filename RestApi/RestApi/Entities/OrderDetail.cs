using System;
using System.Collections.Generic;

namespace RestApi.Entities;

public partial class OrderDetail
{
    public int OrderDetailId { get; set; }

    public int? OrderId { get; set; }

    public int? ProductSizeId { get; set; }

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual Order? Order { get; set; }

    public virtual ProductSize? ProductSize { get; set; }
}
