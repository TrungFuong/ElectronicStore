using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class DiscountUsage
    {
        public DateTime UsedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public string DiscountId { get; set; }
        [ForeignKey(nameof(DiscountId))]
        public Discount Discount { get; set; }

        [Required]
        public string OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; }

        [Required]
        public string CustomerId { get; set; }
        [ForeignKey(nameof(CustomerId))]
        public Customer Customer { get; set; }

    }
}
