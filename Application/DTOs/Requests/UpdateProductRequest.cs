using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests
{
    public class UpdateProductRequest
    {
        public string ProductId { get; set; } = default!;
        public string ProductName { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public string? ProductDescription { get; set; }
        public decimal ProductPrice { get; set; }
        public string? CategoryId { get; set; }
        public string? BrandId { get; set; }
        public bool IsActive { get; set; }
    }
}
