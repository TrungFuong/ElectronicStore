using System;
using System.Collections.Generic;

namespace Application.DTOs.Responses
{
    public class OrderItemResponse
    {
        public string VariationId { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal Total { get; set; }
    }

    public class OrderResponse
    {
        public string OrderId { get; set; } = null!;
        public int OrderStatus { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal Total { get; set; }
        public string ShippingAddress { get; set; } = null!;
        public string? Note { get; set; }
        public DateOnly CreatedAt { get; set; }
        public DateOnly UpdatedAt { get; set; }
        public string CustomerId { get; set; } = null!;
        public IEnumerable<OrderItemResponse> Items { get; set; } = Array.Empty<OrderItemResponse>();
    }
}
