using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Application.DTOs.Requests
{
    public class CreateProductRequest
    {
        [Required]
        public string ProductName { get; set; } = null!;

        public string? ProductDescription { get; set; }

        [Required]
        public string CategoryId { get; set; } = null!;

        [Required]
        public string BrandId { get; set; } = null!;
    }
}
