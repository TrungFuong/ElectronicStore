using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class VariationAttribute
    {
        [Key]
        public int AttributeId { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; } // RAM, Color, Storage...
    }
}
