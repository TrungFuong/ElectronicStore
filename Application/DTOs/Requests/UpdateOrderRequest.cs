using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Requests
{
    public class UpdateOrderRequest
    {
        [Required]
        public string OrderId { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string ShippingAddress { get; set; } = null!;

        [MaxLength(255)]
        public string? Note { get; set; }

        public List<OrderItemRequest>? Items { get; set; }
    }
}
