using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Customer
    {
        [Key]
        public string CustomerId { get; set; }
        [Required]
        [Unicode]
        [MaxLength(50)]
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        [Required]
        [MaxLength(20)]
        public string CustomerPhone { get; set; }
        [Required]
        [MaxLength(255)]
        public string CustomerAddress { get; set; }
        public DateOnly CustomerDOB { get; set; }
        public string? AccountId { get; set; }
        public Account Account { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<DiscountUsage> DiscountUsages { get; set; } = new List<DiscountUsage>();
    }
}
