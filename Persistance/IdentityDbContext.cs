using backend.Entities.Store;
using backend.Entities.UserInfo;
using Microsoft.EntityFrameworkCore;

namespace backend.Persistance
{
    public class IdentityDbContext : DbContext
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
        {
        }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.CategoryName).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Username).IsUnique();
                entity.Property(e => e.RegisterDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.ToTable(table =>
                {
                    table.HasCheckConstraint("CK_Product_Price", "Price >= 0");
                    table.HasCheckConstraint("CK_Product_Discount", "DiscountPrice IS NULL OR (DiscountPrice >= 0 AND DiscountPrice < Price)");
                    table.HasCheckConstraint("CK_Product_StockQuantity", "StockQuantity >= 0");
                });
            });

            modelBuilder.Entity<Admin>(entity =>
            {
                entity.HasIndex(e => e.Username).IsUnique();
                entity.ToTable(table =>
                {
                    table.HasCheckConstraint("CK_Admin_Role", "Role IN ('Owner', 'Manager', 'Staff')");
                });
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.OrderDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Status).HasMaxLength(50).HasDefaultValue("Pending");
                entity.HasOne(o => o.Customer)
                    .WithMany(c => c.Orders)
                    .HasForeignKey(o => o.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.ToTable(table =>
                {
                    table.HasCheckConstraint("CK_Order_TotalAmount", "TotalAmount >= 0");
                    table.HasCheckConstraint("CK_Order_Status", "Status IN ('Pending', 'Processing', 'Shipped', 'Delivered', 'Cancelled')");
                });
            });

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.Property(e => e.AddedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.HasIndex(e => new { e.CustomerId, e.ProductId }).IsUnique();
                entity.HasOne(c => c.Customer)
                    .WithMany(cu => cu.Carts)
                    .HasForeignKey(c => c.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(c => c.Product)
                    .WithMany(p => p.Carts)
                    .HasForeignKey(c => c.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.ToTable(table =>
                {
                    table.HasCheckConstraint("CK_Cart_Quantity", "Quantity > 0");
                });
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasIndex(e => new { e.OrderId, e.ProductId }).IsUnique();
                entity.HasOne(od => od.Order)
                    .WithMany(o => o.OrderDetails)
                    .HasForeignKey(od => od.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(od => od.Product)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(od => od.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.ToTable(table =>
                {
                    table.HasCheckConstraint("CK_OrderDetail_Quantity", "Quantity > 0");
                    table.HasCheckConstraint("CK_OrderDetail_Price", "Price >= 0");
                });
            });
        }
    }
}
