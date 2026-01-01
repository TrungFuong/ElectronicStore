using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Requests
{
    public class CreateDiscountRequest
    {
        [Required]
        public string DiscountName { get; set; }
        [Required]
        public string DiscountCode { get; set; }
        public string? DiscountDescription { get; set; }
        [Required]
        public EnumDiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal MinOrderValue { get; set; }
        public decimal MaxDiscountAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public int UsageLimit { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
