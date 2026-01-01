using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Cart
    {
        [Key]
        public string CartId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string AccountId { get; set; } = default!;
        public Account Account { get; set; } = default!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
