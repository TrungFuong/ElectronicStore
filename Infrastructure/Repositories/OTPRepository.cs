using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class OTPRepository : IOTPRepository
    {
        private readonly DBContext _context;

        public OTPRepository(DBContext context)
        {
            _context = context;
        }

        // Tạo OTP
        public async Task<OTP> CreateAsync(OTP otp)
        {
            _context.OTPs.Add(otp);
            await _context.SaveChangesAsync();
            return otp;
        }

        // Lấy OTP chưa hết hạn
        public async Task<OTP?> GetValidOTPAsync(
            string accountId,
            EnumOTPPurpose purpose
        )
        {
            var now = DateTime.UtcNow;

            return await _context.OTPs
                .Where(x =>
                    x.AccountId == accountId &&
                    x.Purpose == purpose &&
                    x.ExpiredAt > now
                )
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();
        }

        // Invalidate OTP
        public async Task InvalidateOTPAsync(int otpId)
        {
            var otp = await _context.OTPs
                .FirstOrDefaultAsync(x => x.Id == otpId);

            if (otp == null) return;

            otp.IsUsed = true;
            await _context.SaveChangesAsync();
        }

        // Xoá OTP không hợp lệ (hết hạn)
        public async Task DeleteInvalidOTPAsync(string accountId)
        {
            var now = DateTime.UtcNow;

            var invalidOtps = await _context.OTPs
                .Where(x =>
                    x.AccountId == accountId &&
                    x.ExpiredAt <= now
                )
                .ToListAsync();

            if (invalidOtps.Count == 0) return;

            _context.OTPs.RemoveRange(invalidOtps);
            await _context.SaveChangesAsync();
        }
    }
}
