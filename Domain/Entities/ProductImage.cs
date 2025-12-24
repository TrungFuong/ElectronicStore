using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ProductImage
    {
        [Key]
        public string ImageId { get; set; }

        [Required]
        public string ProductId { get; set; }
        public Product Product { get; set; }


        [Required, MaxLength(255)]
        public string ImageUrl { get; set; }

        public bool IsMain { get; set; } = false; // true = ảnh chính, false = ảnh phụ

        public bool IsActive { get; set; } = true;
    }
}
