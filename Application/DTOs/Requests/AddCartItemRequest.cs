using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Requests
{
    public class AddCartItemRequest
    {
        [Required]
        public string VariationId { get; set; } = default!;

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; } = 1;
    }
}
