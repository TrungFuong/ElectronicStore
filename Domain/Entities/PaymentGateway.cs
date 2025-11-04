using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class PaymentGateway
    {
        [Key]
        public string PaymentGatewayId { get; set; }
        [Required]
        [MaxLength(100)]
        public string PaymentGatewayName { get; set; }
        [Required]
        [MaxLength(255)]
        public string PaymentGatewayDescription { get; set; }
        [Required]
        public bool IsActive { get; set; } = true;
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();

    }
}
