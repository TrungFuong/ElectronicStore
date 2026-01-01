using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class CartItem
    {
        [Key]
        public string CartItemId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string CartId { get; set; } = default!;
        public Cart Cart { get; set; } = default!;

        [Required]
        public string VariationId { get; set; } = default!;
        public ProductVariation Variation { get; set; } = default!;

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; } = 1;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
