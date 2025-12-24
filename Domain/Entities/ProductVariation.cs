using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ProductVariation
    {
        [Key]
        public string VariationId { get; set; }
        [Required]
        public string ProductId { get; set; }
        public Product Product { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public ICollection<VariationOption>? Options { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
