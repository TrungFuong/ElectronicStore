using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Requests
{
    public class IdRequest
    {
        [Required]
        public string Id { get; set; } = null!;
    }
}
