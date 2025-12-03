using Application.DTOs.Auth;
using Application.DTOs.Requests;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models.Requests;
using Domain.Models.Responses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Implementations
{
    public class AuthService : IAuthService
    {
        //tự đọc về dependency injection
        private readonly IAccountRepository _accountRepo;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(
            IAccountRepository accountRepo,
            ITokenService tokenService,
            IPasswordHasher passwordHasher,
            IConfiguration config,
            IUnitOfWork unitOfWork)

        {
            _accountRepo = accountRepo;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
            _config = config;
            _unitOfWork = unitOfWork;
        }

        public Task<bool> ChangePasswordAsync(ChangePasswordRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var account = await _accountRepo.GetByPhoneAsync(request.Phone);

            //check tài khoản có tồn tại và đang hoạt động
            if (account == null || !account.IsActive)
            {
                throw new Exception("Tài khoản không tồn tại hoặc đã bị vô hiệu hóa");
            }

            //kiểm tra mật khẩu
            var validPassword = _passwordHasher.VerifyPassword(
               request.Password,
               account.Salt,
               account.HashPassword
           );

            if (!validPassword)
                throw new Exception("Mật khẩu không hợp lệ");

            //tạo token
            var accessToken = _tokenService.GenerateAccessToken(account);

            //tạo refresh token
            var refreshToken = _tokenService.GenerateRefreshToken(account.AccountId);

            await _unitOfWork.RefreshTokenRepository.AddAsync(refreshToken);
            await _unitOfWork.CommitAsync();

            string name = account.Role switch
            {
                EnumRole.Customer => account.Customer?.CustomerName ?? "Customer",
                EnumRole.Staff => account.Staff?.StaffName ?? "Staff",
                EnumRole.Admin => "Admin",
                _ => "User"
            };

            var expireMinutes = int.Parse(_config["JwtSettings:AccessTokenExpiryMinutes"]);

            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                AccountId = account.AccountId,
                Name = name,
                Role = account.Role.ToString(),
                AccessTokenExpireAt = DateTime.UtcNow.AddMinutes(expireMinutes)
            };
        }

        public Task<bool> LogoutAsync(string refreshToken)
        {
            throw new NotImplementedException();
        }

        public Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            // 1. Check phone tồn tại hay chưa
            var exists = await _accountRepo.GetByPhoneAsync(request.Phone);
            if (exists != null)
                throw new Exception("Số điện thoại đã được sử dụng.");

            // 2. Generate salt
            var salt = _passwordHasher.GenerateSalt();

            // 3. Hash password
            var hash = _passwordHasher.HashPassword(request.Password, salt);

            // 4. Tạo Account object
            var account = new Account
            {
                AccountId = Guid.NewGuid().ToString(),
                Phone = request.Phone,
                Salt = salt,
                HashPassword = hash,
                Role = request.Role,
                IsActive = true
            };

            // 5. Save using UnitOfWork
            await _unitOfWork.AccountRepository.AddAsync(account);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
