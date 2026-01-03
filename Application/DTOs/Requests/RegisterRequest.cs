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
        [Compare("Phone", ErrorMessage = "Số điện thoại xác nhận không khớp")]
        public string ConfirmPhone { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [Compare("Email", ErrorMessage = "Email xác nhận không khớp")]
        public String ConfirmEmail { get; set; }
        [Required]
        [RegularExpression(PasswordRegex.PASSWORD, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự, gồm chữ hoa, chữ thường, số và ký tự đặc biệt")]
        public string Password { get; set; }
        [Required]
        [RegularExpression(PasswordRegex.PASSWORD, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự, gồm chữ hoa, chữ thường, số và ký tự đặc biệt")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; }
    }
}
