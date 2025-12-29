using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Responses
{
    public class CartItemResponse
    {
        public string CartItemId { get; set; } = default!;
        public string VariationId { get; set; } = default!;
        public string ProductId { get; set; } = default!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }
    }
}

