using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.DataAccess
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options) { }

        public DBContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var ConnectionString = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build()
                    .GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(ConnectionString, builder =>
                {
                    builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                });
            }
        }

        // Các bảng cũ
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

        // Các bảng mới cho Product Variation
        public DbSet<ProductVariation> ProductVariations { get; set; } = null!;
        public DbSet<VariationAttribute> VariationAttributes { get; set; } = null!;
        public DbSet<VariationOption> VariationOptions { get; set; } = null!;
        public DbSet<ProductImage> ProductImages { get; set; } = null!;


        //Specification 
        public DbSet<ProductSpecification> ProductSpecifications { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Cấu hình quan hệ cũ ---
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

            // Customer - Order / DiscountUsage
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

            // Discount - DiscountUsage
            modelBuilder.Entity<Discount>()
                .HasMany(d => d.DiscountUsages)
                .WithOne(du => du.Discount)
                .HasForeignKey(du => du.DiscountId)
                .OnDelete(DeleteBehavior.Cascade);

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

            // Product - Brand / Category / OrderDetails
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

            // --- Cấu hình ProductVariation ---
            modelBuilder.Entity<ProductVariation>()
                .HasOne(v => v.Product)
                .WithMany(p => p.Variations)
                .HasForeignKey(v => v.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductVariation>()
                .HasMany(v => v.Options)
                .WithOne(o => o.Variation)
                .HasForeignKey(o => o.VariationId)
                .OnDelete(DeleteBehavior.Cascade);

            // VariationOption - VariationAttribute
            modelBuilder.Entity<VariationOption>()
                .HasOne(o => o.Attribute)
                .WithMany()
                .HasForeignKey(o => o.AttributeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Product - ProductImage
            modelBuilder.Entity<Product>()
                .HasMany(p => p.Images)
                .WithOne(i => i.Product)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // ProductImage: 1 ảnh chính + 3 ảnh phụ
            modelBuilder.Entity<ProductImage>()
                .Property(i => i.IsMain)
                .HasDefaultValue(false);
            modelBuilder.Entity<ProductSpecification>()
                .HasOne(ps => ps.Product)
                .WithMany(p => p.Specifications)
                .HasForeignKey(ps => ps.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductSpecification>()
                .HasIndex(ps => new { ps.ProductId, ps.SpecKey });

            modelBuilder.Entity<VariationAttribute>().HasData(
                new VariationAttribute { AttributeId = 1, Name = "Màu sắc" },
                new VariationAttribute { AttributeId = 2, Name = "Dung lượng" },
                new VariationAttribute { AttributeId = 3, Name = "RAM" },
                new VariationAttribute { AttributeId = 4, Name = "Kích thước" },
                new VariationAttribute { AttributeId = 5, Name = "Phiên bản" }
            );
        }
    }

}
