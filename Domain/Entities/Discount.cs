using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Discount
    {
        [Key]
        public string DiscountId { get; set; }
        [Required]
        [MaxLength(100)]
        public string DiscountName { get; set; }
        [Required]
        [MaxLength(50)]
        public string DiscountCode { get; set; }
        [MaxLength(255)]
        public string DiscountDescription { get; set; }
        [Required]
        public EnumDiscountType DiscountType { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountValue { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal MinOrderValue { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal MaxDiscountAmount { get; set; }
        [Required]
        public DateTime StartDate { get; set; } = DateTime.Now;
        [Required]
        public DateTime ExpireDate { get; set; }
        public int UsageLimit { get; set; }
        public int UsageCount { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<OrderDetail>? OrderDetails { get; set; } = new List<OrderDetail>();

        public ICollection<DiscountUsage>? DiscountUsages { get; set; } = new List<DiscountUsage>();
    }
}
