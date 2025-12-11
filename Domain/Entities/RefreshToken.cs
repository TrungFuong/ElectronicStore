using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class RefreshToken
    {
        [Key]
        public int TokenId { get; set; }
        [Required]
        [MaxLength(500)]
        public string Token { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpireAt { get; set; }
        public bool IsRevoked { get; set; } = false;
        public DateTime? RevokedAt { get; set; }
        public string? ReplacedByToken { get; set; }
        public string AccountId { get; set; }
        public Account Account { get; set; }
    }
}
