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
                entity.Property(e => e.RegisterDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Price).HasColumnType("decimal(10,2)");
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.ToTable(table =>
                {
                    table.HasCheckConstraint("CK_Product_Discount", "Discount >= 0 AND Discount <= 1");
                    table.HasCheckConstraint("CK_Product_StockQuantity", "StockQuantity >= 0");
                });
            });

            modelBuilder.Entity<Admin>(entity =>
            {
                entity.HasIndex(e => e.Username).IsUnique();
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(10,2)");
                entity.Property(e => e.OrderDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Status).HasMaxLength(50).HasDefaultValue("Pending");
                entity.HasOne(o => o.Customer)
                    .WithMany(c => c.Orders)
                    .HasForeignKey(o => o.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade);
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
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.Property(e => e.Price).HasColumnType("decimal(10,2)");
                entity.HasIndex(e => new { e.OrderId, e.ProductId }).IsUnique();
                entity.HasOne(od => od.Order)
                    .WithMany(o => o.OrderDetails)
                    .HasForeignKey(od => od.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(od => od.Product)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(od => od.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
