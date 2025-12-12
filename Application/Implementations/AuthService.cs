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
            //không lưu access token nên không cần thu hồi, ở frontend chỉ cần xóa đi
            try
            {
                var token = _unitOfWork.RefreshTokenRepository
                    .GetByTokenAsync(refreshToken).Result;
                if (token == null)
                    throw new Exception("Refresh token không tồn tại.");
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
                _unitOfWork.RefreshTokenRepository.Update(token);
                _unitOfWork.CommitAsync().Wait();
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                throw new Exception("Đăng xuất thất bại: " + ex.Message);
            }
        }

        public async Task<RefreshTokenResponse> RefreshTokenAsync(string token)
        {
            var oldToken = await _unitOfWork.RefreshTokenRepository.GetByTokenAsync(token);

            if (oldToken == null)
                throw new Exception("Refresh token không tồn tại.");

            if (oldToken.IsRevoked)
                throw new Exception("Refresh token đã bị thu hồi.");

            if (oldToken.ExpireAt <= DateTime.UtcNow)
                throw new Exception("Refresh token đã hết hạn.");

            if (oldToken.Account == null || !oldToken.Account.IsActive)
                throw new Exception("Tài khoản không hợp lệ hoặc đã bị vô hiệu hóa.");

            //Tạo refresh token mới
            var newRefresh = _tokenService.GenerateRefreshToken(oldToken.AccountId);
            oldToken.IsRevoked = true;
            oldToken.RevokedAt = DateTime.UtcNow;
            oldToken.ReplacedByToken = newRefresh.Token;

            await _unitOfWork.RefreshTokenRepository.AddAsync(newRefresh);

            await _unitOfWork.CommitAsync();

            //Tạo access token mới
            var accessToken = _tokenService.GenerateAccessToken(oldToken.Account);

            return new RefreshTokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = newRefresh.Token,
                AccessTokenExpireAt = DateTime.UtcNow.AddMinutes(
                    int.Parse(_config["JwtSettings:AccessTokenExpiryMinutes"])
                )
            };
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            // 1. Check phone tồn tại hay chưa
            var exists = await _accountRepo.GetByPhoneAsync(request.Phone);
            if (exists != null)
                throw new Exception("Số điện thoại đã được sử dụng.");

            // 3. Hash password
            var hash = _passwordHasher.HashPassword(request.Password);

            // 4. Tạo Account object
            var account = new Account
            {
                AccountId = Guid.NewGuid().ToString(),
                Phone = request.Phone,
                HashPassword = hash,
                Role = request.Role,
                IsActive = true
            };

            await _unitOfWork.AccountRepository.AddAsync(account);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordRequest request)
        {
            try
            {
                //Lấy account
                var account = await _accountRepo.GetByIdAsync(request.AccountId);
                if (account == null || !account.IsActive)
                    throw new Exception("Tài khoản không tồn tại hoặc đã bị vô hiệu hóa");

                //Verify mật khẩu hiện tại 
                var valid = _passwordHasher.VerifyPassword(request.OldPassword, account.HashPassword);
                if (!valid)
                    throw new Exception("Mật khẩu hiện tại không đúng.");

                if (request.NewPassword == request.OldPassword)
                    throw new Exception("Mật khẩu mới phải khác mật khẩu hiện tại");

                // Hash mật khẩu mới
                var newHash = _passwordHasher.HashPassword(request.NewPassword);
                account.HashPassword = newHash;

                _unitOfWork.AccountRepository.Update(account);

                // Revoke tất cả refresh token liên quan 
                var tokens = await _unitOfWork.RefreshTokenRepository.GetByAccountIdAsync(account.AccountId);
                if (tokens != null)
                {
                    foreach (var t in tokens)
                    {
                        t.IsRevoked = true;
                        t.RevokedAt = DateTime.UtcNow;
                        _unitOfWork.RefreshTokenRepository.Update(t);
                    }
                }

                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Đổi mật khẩu thất bại: " + ex.Message);
            }
        }

        public Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
