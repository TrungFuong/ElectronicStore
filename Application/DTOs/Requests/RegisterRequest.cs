using Domain.Constants;
using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Application.DTOs.Requests
{
    public class RegisterRequest
    {
        [Required]
        public string Phone { get; set; }
        [Required]
        [RegularExpression(PasswordRegex.PASSWORD, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự, gồm chữ hoa, chữ thường, số và ký tự đặc biệt")]
        public string Password { get; set; }
        public EnumRole Role { get; set; } = EnumRole.Customer;
    }
}
