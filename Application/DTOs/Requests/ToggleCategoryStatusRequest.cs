using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Requests
{
    public class ToggleCategoryStatusRequest
    {
        [Required]
        public string CategoryId { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}
