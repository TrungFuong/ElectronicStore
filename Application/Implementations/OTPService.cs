using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Application.Implementations
{
    public class OTPService : IOTPService
    {
        private readonly IOTPRepository _otpRepository;
        private readonly IEmailService _emailService;
        private readonly IAccountRepository _accountRepository;

        public OTPService(
           IAccountRepository accountRepository,
           IOTPRepository otpRepository,
           IEmailService emailService
           )
        {
            _otpRepository = otpRepository;
            _emailService = emailService;
            _accountRepository = accountRepository;
        }

        public static string Hash(string otp)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(otp));
            return Convert.ToBase64String(bytes);
        }

        public async Task<string> GenerateAndSendOTPAsync(string email, EnumOTPPurpose purpose)
        {
            var acc = await _accountRepository.GetByEmailAsync(email);
            await _otpRepository.DeleteInvalidOTPAsync(acc.AccountId);

            var otp = Random.Shared.Next(100000, 999999).ToString();

            var hashedOtp = Hash(otp);


            var otpEntity = new OTP
            {
                AccountId = acc.AccountId,
                Purpose = purpose,
                HashedOTP = hashedOtp,
                ExpiredAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false
            };

            await _otpRepository.CreateAsync(otpEntity);

            await _emailService.SendAsync(
                to: acc.Email,
                subject: "OTP",
                body: $"OTP: {otp}. Hết hạn sau 5 phút"
            );

            return otp;
        }

        public async Task ValidateOTPAsync(string accountId, EnumOTPPurpose purpose, string code)
        {
            var otpEntity = await _otpRepository.GetValidOTPAsync(accountId, purpose)
                ?? throw new Exception("OTP hết hạn hoặc chim cút rồi");

            var hashedInput = Hash(code);

            if (hashedInput != otpEntity.HashedOTP)
                throw new Exception("OTP sai");

            await _otpRepository.InvalidateOTPAsync(otpEntity.Id);
        }

    }
}
