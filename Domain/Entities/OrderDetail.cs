using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class OrderDetail
    {
        [Key]
        public string OrderDetailId { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }   // Giá tại thời điểm mua

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        // FK

        [Required]
        public string OrderId { get; set; } = null!;

        [Required]
        public string VariationId { get; set; } = null!;


        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; } = null!;

        [ForeignKey(nameof(VariationId))]
        public ProductVariation Variation { get; set; } = null!;
    }
}
