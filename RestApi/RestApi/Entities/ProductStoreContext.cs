using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RestApi.Entities;

public partial class ProductStoreContext : DbContext
{
    public ProductStoreContext()
    {
    }

    public ProductStoreContext(DbContextOptions<ProductStoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductSize> ProductSizes { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Wishlist> Wishlists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-HU50TNN\\SQLEXPRESS;Initial Catalog=ProductStore;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK__Carts__2EF52A27D40BA24E");

            entity.Property(e => e.CartId)
                .ValueGeneratedNever()
                .HasColumnName("cart_id");
            entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Product).WithMany(p => p.Carts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Carts__product_i__68487DD7");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Carts__user_id__693CA210");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__D54EE9B48ACB835A");

            entity.Property(e => e.CategoryId)
                .ValueGeneratedNever()
                .HasColumnName("category_id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("category_name");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.ToTable("Contact");

            entity.Property(e => e.ContactId)
                .ValueGeneratedNever()
                .HasColumnName("contact_id");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .HasColumnName("phone");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__4659622934AE7CD8");

            entity.Property(e => e.OrderId)
                .ValueGeneratedNever()
                .HasColumnName("order_id");
            entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            entity.Property(e => e.OrderDate)
                .HasColumnType("datetime")
                .HasColumnName("order_date");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_price");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Orders__user_id__6C190EBB");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__3C5A4080EC873FE0");

            entity.ToTable(tb =>
                {
                    tb.HasTrigger("TotalPrice");
                    tb.HasTrigger("updateQuantity");
                    tb.HasTrigger("updatetienban");
                });

            entity.Property(e => e.OrderDetailId)
                .ValueGeneratedNever()
                .HasColumnName("order_detail_id");
            entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.ProductSizeId).HasColumnName("product_size_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__OrderDeta__order__6A30C649");

            entity.HasOne(d => d.ProductSize).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductSizeId)
                .HasConstraintName("FK__OrderDeta__produ__6B24EA82");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__47027DF5887A7F9B");

            entity.Property(e => e.ProductId)
                .ValueGeneratedNever()
                .HasColumnName("product_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("image_url");
            entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.ProductName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("product_name");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Products__catego__6D0D32F4");
        });

        modelBuilder.Entity<ProductSize>(entity =>
        {
            entity.HasKey(e => e.ProductSizeId).HasName("PK__ProductS__062A9A682CA7BAEC");

            entity.ToTable("ProductSize");

            entity.Property(e => e.ProductSizeId)
                .ValueGeneratedNever()
                .HasColumnName("product_size_id");
            entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Size)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("size");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductSizes)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__ProductSi__produ__6E01572D");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.TokenId);

            entity.ToTable("RefreshToken");

            entity.Property(e => e.TokenId).HasColumnName("token_id");
            entity.Property(e => e.ExpiryDate)
                .HasColumnType("datetime")
                .HasColumnName("expiry_date");
            entity.Property(e => e.Token)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("token");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RefreshTo__user___74AE54BC");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Reviews__60883D9031F8A504");

            entity.Property(e => e.ReviewId)
                .ValueGeneratedNever()
                .HasColumnName("review_id");
            entity.Property(e => e.Comment)
                .HasColumnType("text")
                .HasColumnName("comment");
            entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.ReviewDate)
                .HasColumnType("datetime")
                .HasColumnName("review_date");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Product).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Reviews__product__6EF57B66");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Reviews__user_id__6FE99F9F");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__760965CCD75C8B4B");

            entity.Property(e => e.RoleId)
                .ValueGeneratedNever()
                .HasColumnName("role_id");
            entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__B9BE370FC4D27043");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.Address)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("username");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__Users__role_id__70DDC3D8");
        });

        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity.HasKey(e => e.WishlistId).HasName("PK__Wishlist__6151514E80308238");

            entity.ToTable("Wishlist");

            entity.Property(e => e.WishlistId)
                .ValueGeneratedNever()
                .HasColumnName("wishlist_id");
            entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Product).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Wishlist__produc__71D1E811");

            entity.HasOne(d => d.User).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Wishlist__user_i__72C60C4A");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
