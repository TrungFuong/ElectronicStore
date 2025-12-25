using Domain.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests
{
    public class AddStaffRequest
    {
        public string Phone { get; set; }
        [RegularExpression(PasswordRegex.PASSWORD, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự, gồm chữ hoa, chữ thường, số và ký tự đặc biệt")]
        public string Password { get; set; }
        public string StaffName { get; set; }
        public DateOnly StaffDOB { get; set; }
    }
}
