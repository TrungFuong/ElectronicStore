using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests
{
    public class ChangePasswordRequest
    {
        public string AccountId { get; set; } // lấy từ Claims
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}