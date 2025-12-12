using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.DataAccess
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {
        }

        public DBContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var ConnectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(ConnectionString, builder =>
                {
                    builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                });
            }
        }

        public DbSet<Account> Accounts { get; set; } = null!;
        public DbSet<Brand> Brands { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Discount> Discounts { get; set; } = null!;
        public DbSet<DiscountUsage> DiscountUsages { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public DbSet<Payment> Payments { get; set; } = null!;
        public DbSet<PaymentGateway> PaymentGateways { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public DbSet<Staff> Staffs { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Account - Customer / Staff (1-1)
            modelBuilder.Entity<Account>()
                .HasOne(a => a.Customer)
                .WithOne(c => c.Account)
                .HasForeignKey<Customer>(c => c.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasOne(a => a.Staff)
                .WithOne(s => s.Account)
                .HasForeignKey<Staff>(s => s.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // Account - RefreshToken (1-n)
            modelBuilder.Entity<Account>()
                .HasMany(a => a.RefreshTokens)
                .WithOne(rt => rt.Account)
                .HasForeignKey(rt => rt.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            // Brand - Product (1-n)
            modelBuilder.Entity<Brand>()
                .HasMany(b => b.Products)
                .WithOne(p => p.Brand)
                .HasForeignKey(p => p.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            // Category - Product (1-n)
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Customer - Account
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Orders)
                .WithOne(o => o.Customer)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Customer>()
                .HasMany(c => c.DiscountUsages)
                .WithOne(du => du.Customer)
                .HasForeignKey(du => du.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Discount - DiscountUsage (1-n)
            modelBuilder.Entity<Discount>()
                .HasMany(d => d.DiscountUsages)
                .WithOne(du => du.Discount)
                .HasForeignKey(du => du.DiscountId)
                .OnDelete(DeleteBehavior.Cascade);

            // DiscountUsage (thực thể yếu, composite key)
            modelBuilder.Entity<DiscountUsage>()
                .HasKey(du => new { du.DiscountId, du.CustomerId, du.OrderId });

            modelBuilder.Entity<DiscountUsage>()
                .HasOne(du => du.Order)
                .WithMany(o => o.DiscountUsages)
                .HasForeignKey(du => du.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DiscountUsage>()
                .HasOne(du => du.Customer)
                .WithMany(c => c.DiscountUsages)
                .HasForeignKey(du => du.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Order - OrderDetail / Payment / DiscountUsage
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderDetails)
                .WithOne(od => od.Order)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.Payments)
                .WithOne(p => p.Order)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.DiscountUsages)
                .WithOne(du => du.Order)
                .HasForeignKey(du => du.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // OrderDetail
            modelBuilder.Entity<OrderDetail>()
                .HasKey(od => new { od.OrderId, od.ProductId });

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Payment - PaymentGateway / Order
            modelBuilder.Entity<Payment>()
                .HasKey(p => new { p.PaymentGatewayId, p.OrderId });

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.PaymentGateway)
                .WithMany(pg => pg.Payments)
                .HasForeignKey(p => p.PaymentGatewayId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Product
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Brand)
                .WithMany(b => b.Products)
                .HasForeignKey(p => p.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.OrderDetails)
                .WithOne(od => od.Product)
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // RefreshToken
            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.Account)
                .WithMany(a => a.RefreshTokens)
                .HasForeignKey(rt => rt.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            // Staff (1-1 Account)
            modelBuilder.Entity<Staff>()
                .HasOne(s => s.Account)
                .WithOne(a => a.Staff)
                .HasForeignKey<Account>(a => a.StaffId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}