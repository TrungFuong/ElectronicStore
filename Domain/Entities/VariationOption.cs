using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class VariationOption
    {
        [Key]
        public string OptionId { get; set; }
        [Required]
        public string VariationId { get; set; }
        public ProductVariation Variation { get; set; }
        [Required]
        public int AttributeId { get; set; }
        public VariationAttribute Attribute { get; set; }

        [Required, MaxLength(50)]
        public string Value { get; set; } // Ví dụ: 16GB, Black, 512GB
    }
}
