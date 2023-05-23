using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace RestApi.Entities;

public partial class User 
{
    public int UserId { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public int RoleId { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual Role? Role { get; set; }

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
