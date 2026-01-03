using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IOTPService
    {
        Task<string> GenerateAndSendOTPAsync(string accountId, EnumOTPPurpose purpose);
        Task ValidateOTPAsync(string accountId, EnumOTPPurpose purpose, string code);
    }
}
