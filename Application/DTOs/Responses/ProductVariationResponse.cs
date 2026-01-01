using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Responses
{
    public class ProductVariationResponse
    {
        public string VariationId { get; set; } = null!;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        public List<VariationOptionResponse> Options { get; set; } = new();
    }
}
