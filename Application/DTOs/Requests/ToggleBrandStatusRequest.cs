using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Requests
{
    public class ToggleBrandStatusRequest
    {
        [Required]
        public string BrandId { get; set; } = null!;

        [Required]
        public bool IsActive { get; set; }
    }
}
