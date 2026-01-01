using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Requests
{
    public class OrderItemRequest
    {
        [Required]
        public string VariationId { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }
    }
}
