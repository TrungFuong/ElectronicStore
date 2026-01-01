using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Responses
{
    public class CategoryProductResponse
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public int StockQuantity { get; set; }
        public string? ProductDescription { get; set; }

        public string? BrandId { get; set; }
        public string? BrandName { get; set; }

        public string? CategoryId { get; set; }
        public string? CategoryName { get; set; }
    }
}
