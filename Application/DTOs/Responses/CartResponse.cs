using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Responses
{
    public class CartResponse
    {
        public string CartId { get; set; } = default!;
        public string AccountId { get; set; } = default!;
        public int TotalQuantity { get; set; }
        public decimal SubTotal { get; set; }
        public List<CartItemResponse> Items { get; set; } = new();
    }
}

