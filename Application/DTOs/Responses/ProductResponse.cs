using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Responses
{
    public class ProductResponse
    {
        public string ProductId { get; set; } = default!;
        public string ProductName { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public string? ProductDescription { get; set; }
        public decimal ProductPrice { get; set; }
        public bool IsActive { get; set; }

        public string? CategoryName { get; set; }
        public string? BrandName { get; set; }
    }
}
