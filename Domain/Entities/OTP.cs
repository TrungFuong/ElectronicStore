using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class OTP
    {
        public int Id { get; set; }
        public string HashedOTP { get; set; }
        public EnumOTPPurpose Purpose { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiredAt { get; set; }
        public bool IsUsed { get; set; } = false;
        public string AccountId { get; set; }
        public Account Account { get; set; }
    }
}
