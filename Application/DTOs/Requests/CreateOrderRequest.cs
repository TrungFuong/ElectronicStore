using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Requests
{
    public class CreateOrderRequest
    {

        [Required]
        public string CustomerId { get; set; } = null!;


        [Required]
        [MaxLength(255)]
        public string ShippingAddress { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string ReceiverName { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string ReceiverPhone { get; set; } = null!;


        [MaxLength(255)]
        public string? Note { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Đơn hàng phải có ít nhất 1 sản phẩm")]
        public List<OrderItemRequest> Items { get; set; } = new();

        public string? DiscountId { get; set; }
    }
}
