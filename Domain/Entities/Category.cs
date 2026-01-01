using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Category
    {
        [Key]
        public string CategoryId { get; set; }
        [MaxLength(50)]
        public string CategoryName { get; set; }
        [MaxLength(500)]
        public string CategoryDescription { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
