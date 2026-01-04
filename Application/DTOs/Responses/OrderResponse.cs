using Domain.Enums;
using System;
using System.Collections.Generic;

namespace Application.DTOs.Responses
{
    public class OrderItemResponse
    {

        public string VariationId { get; set; } = null!;

        public string ProductName { get; set; } = null!;

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public decimal DiscountAmount { get; set; }
        public decimal Total { get; set; }
    }

    public class OrderResponse
    {
        public string OrderId { get; set; } = null!;
        public EnumOrderStatus OrderStatus { get; set; }

        public string ShippingAddress { get; set; } = null!;
        public string ReceiverName { get; set; } = null!;
        public string ReceiverPhone { get; set; } = null!;

        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal Total { get; set; }

        public string? Note { get; set; }
        public DateOnly CreatedAt { get; set; }
        public DateOnly UpdatedAt { get; set; }

        public string CustomerId { get; set; } = null!;

        public List<OrderItemResponse> Items { get; set; } = new();

    }
}
