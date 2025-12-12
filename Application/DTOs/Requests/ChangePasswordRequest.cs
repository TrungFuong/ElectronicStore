using Domain.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests
{
    public class ChangePasswordRequest
    {
        public string AccountId { get; set; } // lấy từ Claims
        public string OldPassword { get; set; }
        [Required]
        [RegularExpression(PasswordRegex.PASSWORD, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự, gồm chữ hoa, chữ thường, số và ký tự đặc biệt")]
        public string NewPassword { get; set; }
    }
}