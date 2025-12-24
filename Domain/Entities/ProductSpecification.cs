using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ProductSpecification
    {
        [Key]
        public string SpecificationId { get; set; }

       
        [Required]
        public string ProductId { get; set; } = null!;
        public Product Product { get; set; } = null!;

        // Tên thuộc tính (ScreenSize, Chipset, CameraRear...)
        [Required]
        [MaxLength(100)]
        public string SpecKey { get; set; } = null!;

        // Giá trị thuộc tính
        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string SpecValue { get; set; } = null!;

        public bool IsActive { get; set; } = true;
    }
}
