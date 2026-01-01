using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Requests
{
    public class UpdateCartItemRequest
    {
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
