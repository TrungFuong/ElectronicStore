using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Responses
{
    public class ProductCardResponse
    {
        [Required]
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public int StockQuantity { get; set; }

        public string ProductDescription { get; set; }
        public string? BrandName { get; set; }
        public string? CategoryName { get; set; }

        public string? ImageUrl { get; set; }
    }
}
