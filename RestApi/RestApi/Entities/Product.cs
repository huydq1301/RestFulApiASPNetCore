using System;
using System.Collections.Generic;

namespace RestApi.Entities;

public partial class Product
{
    public int ProductId { get; set; }

    public string? ProductName { get; set; }

    public decimal? Price { get; set; }

    public string? Description { get; set; }

    public int? CategoryId { get; set; }

    public string? ImageUrl { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual Category? Category { get; set; }

    public virtual ICollection<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
