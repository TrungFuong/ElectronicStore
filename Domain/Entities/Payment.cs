using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Payment
    {
        [Key]
        public string PaymentId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        //do vnpay hoặc momopay trả về
        [MaxLength(255)]
        public string? TransactionId { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        public string? RequestData { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        public string? ResponseData { get; set; }
        public string? Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [Required]
        public string PaymentGatewayId { get; set; }
        public PaymentGateway PaymentGateway { get; set; }
        [Required]
        public string OrderId { get; set; }
        public Order Order { get; set; }
    }
}
