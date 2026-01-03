using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IOTPRepository
    {
        Task<OTP> CreateAsync(OTP otp);
        Task<OTP?> GetValidOTPAsync(String accountId, EnumOTPPurpose purpose);
        Task InvalidateOTPAsync(int otpId);
        Task DeleteInvalidOTPAsync(String accountId);
    }
}