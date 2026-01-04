using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Requests
{
    public class OrderItemRequest
    {
        [Required]
        public string VariationId { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải >= 1")]
        public int Quantity { get; set; }
    }
}
