using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Product
    {
        [Key]
        public string ProductId { get; set; } = default;
        [Required]
        [Unicode]
        [MaxLength(50)]
        public string ProductName { get; set; } = string.Empty;
        [NotMapped]
        public int TotalStock =>Variations?.Sum(v => v.StockQuantity) ?? 0;
        [Unicode]
        [MaxLength(200)]
        public string? ProductDescription { get; set; }
        [Column(TypeName = "decimal(18,2)")]

        public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public DateOnly UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
        public string? CategoryId { get; set; }
        public Category? Category { get; set; }
        public string? BrandId { get; set; }
        public Brand? Brand { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        public ICollection<ProductVariation>? Variations { get; set; }

        public ICollection<ProductImage>? Images { get; set; } = new List<ProductImage>();
        public ICollection<ProductSpecification>? Specifications { get; set; } = new List<ProductSpecification>();

    }
}
