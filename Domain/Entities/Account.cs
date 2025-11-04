using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Account
    {
        [Key]
        public string AccountId { get; set; }
        [Required]
        [MaxLength(200)]
        //pw lưu thì phải mã hóa
        public string HashPassword { get; set; }
        [Required]
        [MaxLength(200)]
        public string Salt { get; set; }
        [Required]
        public EnumRole Role { get; set; } = 0;
        [Required]
        public bool IsActive { get; set; } = true;
        public string CustomerId { get; set; }
        public Customer Customer { get; set; }
        public string StaffId { get; set; }
        public Staff Staff { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
