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
        public string ProductName { get; set; } = string.Empty;

        public int StockQuantity { get; set; }

        public string? ProductDescription { get; set; }

        [Required]
        public decimal ProductPrice { get; set; }

        public string? CategoryId { get; set; }

        public string? BrandId { get; set; }
    }
}
