using Domain;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Brand
    {
        [Key]
        public string BrandId { get; set; }
        [Required]
        [Unicode]
        [MaxLength(100)]
        public string BrandName { get; set; }
        [StringLength(200)]
        [Unicode]
        public string? BrandDescription { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
