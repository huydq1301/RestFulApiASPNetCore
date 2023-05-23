using System;
using System.Collections.Generic;

namespace RestApi.Entities;

public partial class Category
{
    public int CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public string? Description { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
