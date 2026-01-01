using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Requests
{
    public class UpdateCustomerRequest
    {
        [Required]
        public string CustomerId { get; set; } = null!;
        [Required]
        [MaxLength(50)]
        public string CustomerName { get; set; } = null!;
        [Required]
        [MaxLength(100)]
        public string CustomerEmail { get; set; } = null!;
        [Required]
        [MaxLength(20)]
        public string CustomerPhone { get; set; } = null!;
        [Required]
        [MaxLength(255)]
        public string CustomerAddress { get; set; } = null!;
        public DateOnly CustomerDOB { get; set; }
        public string? AccountId { get; set; }
    }
}
